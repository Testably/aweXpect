using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Equality options for <see langword="string" />s.
/// </summary>
public partial class StringEqualityOptions : IOptionsEquality<string?>
{
	private const int DefaultMaxLength = 30;

	private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(1000);
	private readonly string _parameterName;
	private IEqualityComparer<string>? _comparer;
	private bool _ignoreCase;
	private bool _ignoreIndentation;
	private bool _ignoreLeadingWhiteSpace;
	private bool _ignoreNewlineStyle;
	private bool _ignoreTrailingWhiteSpace;
	private bool _isIgnoreCaseSpecified;
	private bool _isIgnoreIndentationSpecified;
	private bool _isIgnoreLeadingWhiteSpaceSpecified;
	private bool _isIgnoreNewlineStyleSpecified;
	private bool _isIgnoreTrailingWhiteSpaceSpecified;

	/// <summary>
	///     The first specified option that changes the lines, which <see cref="AsBlock()" /> cannot honour.
	/// </summary>
	private string? _lineOption;

	private IStringMatchType _matchType = ExactMatch;
	private string? _matchTypeOption;

	/// <summary>
	///     The pattern that was parsed last, because a collection expectation compares every item with the same one.
	/// </summary>
	private ParsedPattern? _parsedPattern;

	/// <summary>
	///     Initializes the options for a pattern that the caller received as <paramref name="parameterName" />, which
	///     an unusable pattern is reported against.
	/// </summary>
	public StringEqualityOptions(string parameterName)
	{
		_parameterName = parameterName;
	}

	/// <summary>
	///     Indicates whether the current match type inspects the content of the subject instead of comparing it as a value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> subject has no content to inspect, so an inspecting match type must fail for it in both
	///     polarities, exactly like the dedicated <c>StartsWith</c> / <c>DoesNotStartWith</c> expectations do.
	/// </remarks>
	public bool InspectsSubject => _matchType.InspectsSubject;

	/// <summary>
	///     Indicates whether the options compare two <see langword="string" />s with plain ordinal equality, i.e. whether
	///     neither the match type nor any option that changes the comparison was set.
	/// </summary>
	/// <remarks>
	///     This lets an expectation hand the comparison to a collection that has its own comparer, as long as nothing
	///     was configured that the collection could not honour.
	/// </remarks>
	public bool ComparesByOrdinalEquality
		=> _matchType is ExactMatchType && _comparer is null && !_ignoreCase && !_ignoreLeadingWhiteSpace &&
		   !_ignoreTrailingWhiteSpace && !_ignoreNewlineStyle && !_ignoreIndentation;

	/// <inheritdoc />
	/// <remarks>
	///     The pattern is validated outside the asynchronous part, so that an unusable pattern throws at the call
	///     instead of only when the returned task is awaited.
	/// </remarks>
	public ValueTask<bool> AreConsideredEqual<TExpected>(string? actual, TExpected expected)
	{
		if (expected is not string expectedString)
		{
			ValidatePattern(null);
			return _matchType.AreConsideredEqual(actual, null, _ignoreCase, _comparer);
		}

		(bool IsStartAnchored, bool IsEndAnchored) anchoredEdges = GetAnchoredEdges();
		expectedString = NormalizeExpected(expectedString, anchoredEdges);
		Regex? regex = ValidatePattern(expectedString);
		return AreConsideredEqualToPattern(
			PadWithInnerWhiteSpace(Normalize(actual), expectedString, anchoredEdges), expectedString, regex);
	}

	/// <summary>
	///     Compares the already normalized <paramref name="actual" /> value with the already normalized and validated
	///     <paramref name="expected" /> pattern, which was parsed as <paramref name="regex" /> for a regex match type.
	/// </summary>
	/// <remarks>
	///     Without a pattern, a comparison that completes synchronously is returned without a state machine, because
	///     every item of a string collection is compared this way.
	/// </remarks>
	private ValueTask<bool> AreConsideredEqualToPattern(string? actual, string expected, Regex? regex)
	{
		if (regex is not null || _matchType is WildcardMatchType)
		{
			return AreConsideredEqualToPatternAsync(actual, expected, regex);
		}

		try
		{
			return CompletedOrAwaited(_matchType.AreConsideredEqual(actual, expected, _ignoreCase, _comparer),
				expected);
		}
		catch (RegexMatchTimeoutException exception)
		{
			throw CreateTimeoutException(expected, exception);
		}
	}

	private ValueTask<bool> CompletedOrAwaited(ValueTask<bool> isEqual, string expected)
		=> isEqual.IsCompletedSuccessfully ? isEqual : AwaitTheComparison(isEqual, expected);

	private async ValueTask<bool> AwaitTheComparison(ValueTask<bool> isEqual, string expected)
	{
		try
		{
			return await isEqual;
		}
		catch (RegexMatchTimeoutException exception)
		{
			throw CreateTimeoutException(expected, exception);
		}
	}

	private async ValueTask<bool> AreConsideredEqualToPatternAsync(string? actual, string expected, Regex? regex)
	{
		try
		{
			if (regex is not null)
			{
				return actual is not null && regex.IsMatch(actual);
			}

			if (_matchType is WildcardMatchType)
			{
				return actual is not null && WildcardMatchType.IsMatch(
					GetParsedPattern(expected, static (_, pattern, ignoreCase)
						=> WildcardMatchType.CreateRegex(pattern, ignoreCase)), actual, _ignoreCase);
			}

			return await _matchType.AreConsideredEqual(actual, expected, _ignoreCase, _comparer);
		}
		catch (RegexMatchTimeoutException exception)
		{
			throw CreateTimeoutException(expected, exception);
		}
	}

	/// <summary>
	///     Counts how often the <paramref name="expected" /> <see langword="string" /> occurs
	///     in the <paramref name="actual" /> <see langword="string" />.
	/// </summary>
	/// <remarks>
	///     Both strings are normalized once before the comparison, so that the options which change the length of the
	///     strings are applied to the complete strings and not to the individual substrings that are compared.<br />
	///     The leading or trailing whitespace of the <paramref name="expected" /> <see langword="string" /> is only ignored
	///     where an occurrence reaches the start or the end of the <paramref name="actual" /> <see langword="string" />, so
	///     that whitespace inside it still has to match.<br />
	///     An <paramref name="expected" /> <see langword="string" /> that is empty after the normalization is rejected
	///     for every match type, because it never occurs, so that a negated expectation could never fail.<br />
	///     The pattern is validated outside the asynchronous part, so that an unusable pattern throws at the call
	///     instead of only when the returned task is awaited.
	/// </remarks>
	/// <exception cref="ArgumentException">
	///     The <paramref name="expected" /> <see langword="string" /> is empty after the normalization.
	/// </exception>
	public ValueTask<int> CountOccurrences(string actual, string expected)
	{
		expected = NormalizeExpected(expected, (false, false));
		Regex? regex = ValidatePattern(expected);
		if (expected.Length == 0)
		{
			throw CreateEmptyPatternException(GetPatternKind() ?? "string");
		}

		actual = PadWithInnerWhiteSpace(Normalize(actual), expected, (false, false));

		int? count = CountOccurrencesWithoutWindow(actual, expected, regex);
		if (count is not null)
		{
			return new ValueTask<int>(count.Value);
		}

		return CountOccurrencesWithWindow(actual, expected);
	}

	/// <summary>
	///     Counts the occurrences of the already normalized and validated <paramref name="expected" /> pattern for
	///     match types that cannot be counted with a window of the expected length, or returns <see langword="null" />.
	/// </summary>
	private int? CountOccurrencesWithoutWindow(string actual, string expected, Regex? regex)
	{
		// A block spans whole lines, so its occurrences cannot be found with a window of the expected length.
		if (_matchType is BlockMatchType)
		{
			return BlockMatchType.CountOccurrences(actual, expected, _comparer ?? UseDefaultComparer(_ignoreCase));
		}

		try
		{
			// A pattern can match a different number of characters than it is long, so its occurrences cannot be found
			// with a window of the expected length.
			if (regex is not null)
			{
				return RegexMatchType.CountOccurrences(actual, regex);
			}

			if (_matchType is WildcardMatchType)
			{
				return WildcardMatchType.CountOccurrences(actual, expected, _ignoreCase);
			}
		}
		catch (RegexMatchTimeoutException exception)
		{
			throw CreateTimeoutException(expected, exception);
		}

		return null;
	}

	/// <summary>
	///     Counts the occurrences of the already normalized and validated <paramref name="expected" /> string by
	///     comparing it with a window of the same length.
	/// </summary>
	/// <remarks>
	///     For the default match type without a comparer, a window equals the <paramref name="expected" /> string
	///     exactly where an ordinal search finds it, so the windows are not copied.
	/// </remarks>
	private async ValueTask<int> CountOccurrencesWithWindow(string actual, string expected)
	{
		if (_matchType is ExactMatchType && _comparer is null)
		{
			return CountOrdinalOccurrences(actual, expected,
				_ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		int count = 0;
		int index = 0;
		while (index < actual.Length)
		{
			if (await _matchType.AreConsideredEqual(
				    actual.Substring(index, Math.Min(expected.Length, actual.Length - index)),
				    expected, _ignoreCase, _comparer))
			{
				count++;
				index += expected.Length;
			}
			else
			{
				index++;
			}
		}

		return count;
	}

	private static int CountOrdinalOccurrences(string actual, string expected, StringComparison comparison)
	{
		int count = 0;
		int index = actual.IndexOf(expected, 0, comparison);
		while (index >= 0)
		{
			count++;
			index = actual.IndexOf(expected, index + expected.Length, comparison);
		}

		return count;
	}

	/// <summary>
	///     Specifies a new <see cref="IStringMatchType" /> to use for matching two strings, named
	///     <paramref name="optionName" /> in the exception when another match type is already specified.
	/// </summary>
	/// <exception cref="InvalidOperationException">A match type is already specified.</exception>
	public void SetMatchType(IStringMatchType matchType, string optionName)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_matchTypeOption, optionName);
		_matchTypeOption = optionName;
		_matchType = matchType;
	}

	/// <summary>
	///     Rejects a second call of the line <paramref name="option" />, and any line option in combination with
	///     <see cref="AsBlock()" />, which compares the lines on its own.
	/// </summary>
	private void SpecifyLineOption(ref bool isSpecified, string option)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(isSpecified, option);
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_matchType is BlockMatchType ? nameof(AsBlock) : null, option);
		_lineOption ??= option;
		isSpecified = true;
	}

	/// <summary>
	///     Get the expectations text.
	/// </summary>
	public string GetExpectation(string? expected, ExpectationGrammars grammars)
		=> _matchType.GetExpectation(expected, grammars) + GetOptionString();

	/// <summary>
	///     Formats the <paramref name="expected" /> item of an expected collection.
	/// </summary>
	/// <remarks>
	///     A pattern is preceded by its kind, e.g. <c>prefix "foo"</c>, so that it is not mistaken for the value the item
	///     had to be equal to.
	/// </remarks>
	public string FormatExpectedItem(string? expected)
		=> WithPatternKind(Formatter.Format(expected));

	/// <summary>
	///     Precedes the already <paramref name="formattedExpected" /> item with the kind of the pattern.
	/// </summary>
	internal string WithPatternKind(string formattedExpected)
		=> _matchType switch
		{
			PrefixMatchType => "prefix ",
			SuffixMatchType => "suffix ",
			RegexMatchType => "regex ",
			WildcardMatchType => "wildcard ",
			_ => "",
		} + formattedExpected;

	/// <summary>
	///     Get an extended failure text.
	/// </summary>
	public string GetExtendedFailure(string it, ExpectationGrammars grammars, string? actual, string? expected)
	{
		if (grammars.HasFlag(ExpectationGrammars.Negated))
		{
			return $"{it} was {Formatter.Format(actual)}";
		}

		int[]? ignoredColumnsPerLine = GetIgnoredColumnsPerLine(actual);
		actual = NormalizeLines(actual);

		int ignoredLineCount = 0;
		if (_ignoreLeadingWhiteSpace && actual is not null && ignoredColumnsPerLine is not null)
		{
			(ignoredLineCount, int ignoredColumnCount) = CountLeadingWhiteSpace(actual);
			ignoredColumnsPerLine[ignoredLineCount] += ignoredColumnCount;
		}

		StringDifferenceSettings? settings = ignoredColumnsPerLine is null
			? null
			: new StringDifferenceSettings(ignoredLineCount, 0, ignoredColumnsPerLine);

		actual = TrimWhiteSpace(actual);
		expected = NormalizeExpected(expected, GetAnchoredEdges());

		return _matchType.GetExtendedFailure(it, actual, expected, _ignoreCase,
			_comparer ?? UseDefaultComparer(_ignoreCase), settings);
	}

	/// <summary>
	///     Get an extended failure text that states which value <paramref name="it" /> had in its
	///     <paramref name="member" />.
	/// </summary>
	/// <remarks>
	///     The match types phrase their failure with the member as the subject (<c>message was …</c>, <c>message did
	///     not match…</c>), so it is rephrased here; a failure of a custom match type is kept as it is.
	/// </remarks>
	internal string GetExtendedMemberFailure(string it, string member, ExpectationGrammars grammars,
		string? actual, string? expected)
	{
		string failure = GetExtendedFailure(member, grammars, actual, expected);
		string wasPrefix = member + " was ";
		if (failure.StartsWith(wasPrefix, StringComparison.Ordinal))
		{
			return $"{it} had {member} {failure.Substring(wasPrefix.Length)}";
		}

		string didNotMatchPrefix = member + " did not match";
		if (failure.StartsWith(didNotMatchPrefix, StringComparison.Ordinal))
		{
			return
				$"{it} had {member} {Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength))}, which {failure.Substring(member.Length + 1)}";
		}

		return failure;
	}

	/// <summary>
	///     Ignores casing when comparing the <see langword="string" />s.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     The casing is already specified, or a custom comparer is already set via
	///     <see cref="Using(IEqualityComparer{string})" />.
	/// </exception>
	public StringEqualityOptions IgnoringCase(bool ignoreCase = true)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_isIgnoreCaseSpecified, nameof(IgnoringCase));
		if (ignoreCase && _comparer is not null)
		{
			throw CaseAndComparerConflict();
		}

		_isIgnoreCaseSpecified = true;
		_ignoreCase = ignoreCase;
		return this;
	}

	/// <summary>
	///     Ignores the indentation when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreIndentation" /> parameter.
	/// </summary>
	/// <remarks>
	///     Enabling this option will remove the leading whitespace from every line and replace all occurrences of
	///     <c>\r\n</c> and <c>\r</c> with <c>\n</c> in the strings before comparing them, which makes
	///     <see cref="IgnoringNewlineStyle(bool)" /> redundant.<br />
	///     Any Unicode whitespace counts as indentation, e.g. also a non-breaking space.<br />
	///     Trailing whitespace within a line is kept, but a line that consists only of whitespace becomes empty.<br />
	///     The expected value is transformed as well, which also applies to wildcard and regex patterns.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The indentation is already specified, or the expected value is matched as a block via <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringIndentation(bool ignoreIndentation = true)
	{
		SpecifyLineOption(ref _isIgnoreIndentationSpecified, nameof(IgnoringIndentation));
		_ignoreIndentation = ignoreIndentation;
		return this;
	}

	/// <summary>
	///     Ignores the newline style when comparing <see langword="string" />s.
	/// </summary>
	/// <remarks>
	///     Enabling this option will replace all occurrences of <c>\r\n</c> and <c>\r</c> with <c>\n</c> in the strings before
	///     comparing them.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The newline style is already specified, or the expected value is matched as a block via
	///     <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringNewlineStyle(bool ignoreNewlineStyle = true)
	{
		SpecifyLineOption(ref _isIgnoreNewlineStyleSpecified, nameof(IgnoringNewlineStyle));
		_ignoreNewlineStyle = ignoreNewlineStyle;
		return this;
	}

	/// <summary>
	///     Ignores leading whitespace when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreLeadingWhiteSpace" /> parameter.
	/// </summary>
	/// <remarks>
	///     Note:<br />
	///     The position of the first mismatch in the failure message (its index, or its line and column) refers to the
	///     original subject, so it also counts the removed whitespace.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The leading whitespace is already specified, or the expected value is matched as a block via
	///     <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringLeadingWhiteSpace(bool ignoreLeadingWhiteSpace = true)
	{
		SpecifyLineOption(ref _isIgnoreLeadingWhiteSpaceSpecified, nameof(IgnoringLeadingWhiteSpace));
		_ignoreLeadingWhiteSpace = ignoreLeadingWhiteSpace;
		return this;
	}

	/// <summary>
	///     Ignores trailing whitespace when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreTrailingWhiteSpace" /> parameter.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     The trailing whitespace is already specified, or the expected value is matched as a block via
	///     <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringTrailingWhiteSpace(bool ignoreTrailingWhiteSpace = true)
	{
		SpecifyLineOption(ref _isIgnoreTrailingWhiteSpaceSpecified, nameof(IgnoringTrailingWhiteSpace));
		_ignoreTrailingWhiteSpace = ignoreTrailingWhiteSpace;
		return this;
	}

	/// <inheritdoc />
	public override string ToString() => _matchType.GetTypeString() + GetOptionString();

	/// <summary>
	///     Specifies a specific <see cref="IEqualityComparer{T}" /> to use for comparing <see langword="string" />s.
	/// </summary>
	/// <remarks>
	///     Without a comparer, the <see cref="StringComparer.Ordinal" /> or <see cref="StringComparer.OrdinalIgnoreCase" />
	///     is used depending on whether the casing is ignored.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     A comparer is already set, the casing is already ignored via <see cref="IgnoringCase(bool)" />, or the
	///     expected value is matched as a regex or wildcard pattern.
	/// </exception>
	public StringEqualityOptions Using(IEqualityComparer<string> comparer)
	{
		comparer.ThrowIfNull();
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_comparer is not null, nameof(Using));
		if (_ignoreCase)
		{
			throw CaseAndComparerConflict();
		}

		if (_matchType is RegexMatchType or WildcardMatchType)
		{
			throw ComparerAndPatternConflict();
		}

		_comparer = comparer;
		return this;
	}

	/// <summary>
	///     Creates the exception for a custom comparer that is combined with <see cref="IgnoringCase(bool)" />.
	/// </summary>
	/// <remarks>
	///     Only one of the two can be honoured, so the combination is rejected instead of silently dropping the
	///     casing option, which would also disappear from the expectation text.
	/// </remarks>
	private static InvalidOperationException CaseAndComparerConflict()
		// ReSharper disable once LocalizableElement
		=> Tracing.WriteException(new InvalidOperationException(
			"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead."));

	/// <summary>
	///     Creates the exception for a custom comparer that is combined with a regex or wildcard pattern.
	/// </summary>
	/// <remarks>
	///     A pattern is matched by the regex engine, which has no way to consult a comparer, so the combination is
	///     rejected instead of silently ignoring the comparer.
	/// </remarks>
	private static InvalidOperationException ComparerAndPatternConflict()
		// ReSharper disable once LocalizableElement
		=> Tracing.WriteException(new InvalidOperationException(
			"A custom comparer is not supported for regex or wildcard matching."));

	/// <summary>
	///     Creates the exception for an <paramref name="expected" /> pattern that did not finish matching within the
	///     timeout.
	/// </summary>
	/// <remarks>
	///     An <see cref="ArgumentException" /> is not wrapped by the expectation node, so that the pattern which has
	///     to be simplified stays visible instead of being hidden behind a generic evaluation error.
	/// </remarks>
	private ArgumentException CreateTimeoutException(string expected, RegexMatchTimeoutException innerException)
		// ReSharper disable once LocalizableElement
		=> Tracing.WriteException(new ArgumentException(
			$"The {(_matchType is RegexMatchType ? "regex" : "wildcard pattern")} {Formatter.Format(expected)} did not complete within {Formatter.Format(RegexTimeout)}. Simplify the pattern to avoid catastrophic backtracking.",
			_parameterName, innerException));

	private static StringComparer UseDefaultComparer(bool ignoreCase)
		=> ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

	private static bool AreEqualByComparer(IEqualityComparer<string> comparer, string actual, string expected)
		=> UserCode.Invoke(static values => values.Comparer.Equals(values.Actual, values.Expected),
			(Comparer: comparer, Actual: actual, Expected: expected), "the comparer");

	/// <summary>
	///     Applies all options that transform the <paramref name="value" /> as a whole.
	/// </summary>
	/// <remarks>
	///     All these options change the length of the <paramref name="value" />, so they must never be applied to an
	///     individual substring of an already normalized value.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(value))]
	private string? Normalize(string? value)
		=> TrimWhiteSpace(NormalizeLines(value));

	/// <summary>
	///     Applies all options that transform the <paramref name="expected" /> value as a whole, but removes its whitespace
	///     only at the <paramref name="anchoredEdges" />, which line up with the corresponding edge of the subject.
	/// </summary>
	[return: NotNullIfNotNull(nameof(expected))]
	private string? NormalizeExpected(string? expected, (bool IsStartAnchored, bool IsEndAnchored) anchoredEdges)
	{
		expected = NormalizeLines(expected);
		if (_ignoreLeadingWhiteSpace && anchoredEdges.IsStartAnchored)
		{
			expected = expected?.TrimStart();
		}

		if (_ignoreTrailingWhiteSpace && anchoredEdges.IsEndAnchored)
		{
			expected = expected?.TrimEnd();
		}

		return expected;
	}

	/// <summary>
	///     Returns which edges of the expected value line up with the corresponding edge of the subject for the current
	///     match type.
	/// </summary>
	/// <remarks>
	///     A regex may match any part of the subject, like a substring. A custom match type keeps the behaviour of an exact
	///     match, whose expected value covers the whole subject.
	/// </remarks>
	private (bool IsStartAnchored, bool IsEndAnchored) GetAnchoredEdges()
		=> _matchType switch
		{
			PrefixMatchType => (true, false),
			SuffixMatchType => (false, true),
			ContainingMatchType or RegexMatchType => (false, false),
			_ => (true, true),
		};

	/// <summary>
	///     Surrounds the already normalized <paramref name="actual" /> value with the whitespace that the
	///     <paramref name="expected" /> value has at an edge that is not anchored, so that this whitespace is optional
	///     where the expected value reaches the edge of the subject, but still has to match inside the subject.
	/// </summary>
	/// <remarks>
	///     An <paramref name="expected" /> value that consists only of whitespace is empty at an edge of the subject, and
	///     an empty value never occurs, so a substring that consists only of whitespace can only occur inside the subject.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(actual))]
	private string? PadWithInnerWhiteSpace(string? actual, string expected,
		(bool IsStartAnchored, bool IsEndAnchored) anchoredEdges)
	{
		int leading = _ignoreLeadingWhiteSpace && !anchoredEdges.IsStartAnchored
			? expected.Length - expected.TrimStart().Length
			: 0;
		int trailing = _ignoreTrailingWhiteSpace && !anchoredEdges.IsEndAnchored
			? expected.Length - expected.TrimEnd().Length
			: 0;
		if (actual is null || leading + trailing == 0 ||
		    (!anchoredEdges.IsStartAnchored && !anchoredEdges.IsEndAnchored &&
		     Math.Max(leading, trailing) == expected.Length))
		{
			return actual;
		}

#if NET8_0_OR_GREATER
		return string.Concat(expected.AsSpan(0, leading), actual, expected.AsSpan(expected.Length - trailing));
#else
		return expected.Substring(0, leading) + actual + expected.Substring(expected.Length - trailing);
#endif
	}

	/// <summary>
	///     Applies the options that transform the lines of the <paramref name="value" />.
	/// </summary>
	/// <remarks>
	///     Removing the indentation also normalizes the newline style, so it supersedes
	///     <see cref="IgnoringNewlineStyle(bool)" />.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(value))]
	private string? NormalizeLines(string? value)
	{
		if (_ignoreIndentation)
		{
			return value.RemoveIndentation();
		}

		return _ignoreNewlineStyle ? value.RemoveNewlineStyle() : value;
	}

	/// <summary>
	///     Removes the whitespace at the start and the end of the <paramref name="value" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(value))]
	private string? TrimWhiteSpace(string? value)
	{
		if (_ignoreLeadingWhiteSpace)
		{
			value = value?.TrimStart();
		}

		if (_ignoreTrailingWhiteSpace)
		{
			value = value?.TrimEnd();
		}

		return value;
	}

	/// <summary>
	///     Counts the lines and the columns in the last of these lines that are covered by the leading whitespace
	///     of the <paramref name="value" />.
	/// </summary>
	private static (int Lines, int Columns) CountLeadingWhiteSpace(string value)
	{
		int lines = 0;
		int columns = 0;
		foreach (char c in value)
		{
			if (c == '\n')
			{
				lines++;
				columns = 0;
			}
			else if (char.IsWhiteSpace(c))
			{
				columns++;
			}
			else
			{
				break;
			}
		}

		return (lines, columns);
	}

	/// <summary>
	///     Creates the table of ignored columns per line for the <paramref name="value" />, pre-filled with the width
	///     of the ignored indentation, or <see langword="null" /> when no option shifts the reported positions.
	/// </summary>
	/// <remarks>
	///     The table is indexed by the line number of the value returned by <see cref="NormalizeLines" />, so that a
	///     position in the normalized value can be mapped back to the position in the original
	///     <paramref name="value" />.
	/// </remarks>
	private int[]? GetIgnoredColumnsPerLine(string? value)
	{
		if (value is null || (!_ignoreIndentation && !_ignoreLeadingWhiteSpace))
		{
			return null;
		}

		// When the indentation is ignored, the normalized value has the same lines as the newline normalized value,
		// but still contains the indentation whose width is measured below.
		string[] lines = (_ignoreIndentation ? value.RemoveNewlineStyle() : NormalizeLines(value)).Split('\n');
		int[] result = new int[lines.Length];
		if (_ignoreIndentation)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				result[i] = lines[i].Length - lines[i].TrimStart().Length;
			}
		}

		return result;
	}

	private string GetOptionString()
	{
		string initialString = _matchType.GetOptionString(_ignoreCase, _comparer);
		if (!_ignoreLeadingWhiteSpace && !_ignoreTrailingWhiteSpace && !_ignoreNewlineStyle &&
		    !_ignoreIndentation)
		{
			return initialString;
		}

		List<string> tokens = [];
		switch (_ignoreLeadingWhiteSpace, _ignoreTrailingWhiteSpace)
		{
			case (true, true):
				tokens.Add("whitespace");
				break;
			case (true, false):
				tokens.Add("leading whitespace");
				break;
			case (false, true):
				tokens.Add("trailing whitespace");
				break;
		}

		if (_ignoreNewlineStyle)
		{
			tokens.Add("newline style");
		}

		if (_ignoreIndentation)
		{
			tokens.Add("indentation");
		}

		StringBuilder sb = new();
		sb.Append(initialString);
		if (!initialString.Contains("ignoring"))
		{
			sb.Append(" ignoring");
		}
		else
		{
			sb.Append(tokens.Count == 1 ? " and" : ",");
		}

		for (int i = 0; i < tokens.Count; i++)
		{
			if (i > 0)
			{
				sb.Append(i == tokens.Count - 1 ? " and" : ",");
			}

			sb.Append(' ').Append(tokens[i]);
		}

		return sb.ToString();
	}

	/// <summary>
	///     Creates the exception for an expected value of the <paramref name="kind" /> that is empty.
	/// </summary>
	private ArgumentException CreateEmptyPatternException(string kind)
		// ReSharper disable once LocalizableElement
		=> Tracing.WriteException(new ArgumentException($"The '{_parameterName}' {kind} cannot be empty.",
			_parameterName));

	/// <summary>
	///     Names the kind of pattern of the current match type, or returns <see langword="null" /> when the expected
	///     value is not a pattern.
	/// </summary>
	private string? GetPatternKind()
		=> _matchType switch
		{
			PrefixMatchType => "prefix",
			SuffixMatchType => "suffix",
			RegexMatchType => "regex pattern",
			WildcardMatchType => "wildcard pattern",
			_ => null,
		};

	/// <summary>
	///     Verifies that the <paramref name="expected" /> value is a usable pattern for the current match type and
	///     returns the parsed <see cref="Regex" /> for a regex pattern.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> pattern describes nothing to look for and an empty regex, prefix or suffix pattern
	///     matches every value, so such an expectation says nothing about the subject. This can only be detected while
	///     the expectation is verified, because the match type can also be set after the pattern.
	/// </remarks>
	private Regex? ValidatePattern(string? expected)
	{
		string? patternKind = GetPatternKind();
		if (patternKind is null)
		{
			return null;
		}

		if (expected is null)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentNullException(_parameterName,
				$"The '{_parameterName}' {patternKind} cannot be null."));
		}

		if (expected.Length == 0 && _matchType is not WildcardMatchType)
		{
			throw CreateEmptyPatternException(patternKind);
		}

		if (_matchType is not RegexMatchType)
		{
			return null;
		}

		try
		{
			return GetParsedPattern(expected, static (matchType, pattern, ignoreCase)
				=> ((RegexMatchType)matchType).CreateRegex(pattern, ignoreCase));
		}
		catch (ArgumentException exception) when (exception is not ArgumentOutOfRangeException)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException(
				$"The '{_parameterName}' regex pattern is invalid: {exception.Message}", _parameterName, exception));
		}
	}

	/// <summary>
	///     Returns the regex for the <paramref name="expected" /> pattern of the current match type, and parses it with
	///     <paramref name="parse" /> only when the last parsed pattern was a different one.
	/// </summary>
	private Regex GetParsedPattern(string expected, Func<IStringMatchType, string, bool, Regex> parse)
	{
		ParsedPattern? parsedPattern = _parsedPattern;
		if (parsedPattern is not null && parsedPattern.IsFor(_matchType, expected, _ignoreCase))
		{
			return parsedPattern.Regex;
		}

		Regex regex = parse(_matchType, expected, _ignoreCase);
		_parsedPattern = new ParsedPattern(_matchType, expected, _ignoreCase, regex);
		return regex;
	}

	/// <remarks>
	///     Immutable, so that it can be replaced as a whole while another evaluation reads it.
	/// </remarks>
	private sealed class ParsedPattern(IStringMatchType matchType, string expected, bool ignoreCase, Regex regex)
	{
		public Regex Regex { get; } = regex;

		public bool IsFor(IStringMatchType otherMatchType, string otherExpected, bool otherIgnoreCase)
			=> ReferenceEquals(matchType, otherMatchType) && ignoreCase == otherIgnoreCase &&
			   string.Equals(expected, otherExpected, StringComparison.Ordinal);
	}
}
