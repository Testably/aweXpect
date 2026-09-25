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
	private IStringMatchType _matchType = ExactMatch;

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
	public bool InspectsSubject => _matchType is not ExactMatchType;

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

		expectedString = Normalize(expectedString);
		Regex? regex = ValidatePattern(expectedString);
		return AreConsideredEqualToPattern(Normalize(actual), expectedString, regex);
	}

	/// <summary>
	///     Compares the already normalized <paramref name="actual" /> value with the already normalized and validated
	///     <paramref name="expected" /> pattern, which was parsed as <paramref name="regex" /> for a regex match type.
	/// </summary>
	private async ValueTask<bool> AreConsideredEqualToPattern(string? actual, string expected, Regex? regex)
	{
		try
		{
			if (regex is not null)
			{
				return actual is not null && regex.IsMatch(actual);
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
	///     Returns <c>0</c> when the <paramref name="expected" /> <see langword="string" /> is empty after the
	///     normalization.<br />
	///     The pattern is validated outside the asynchronous part, so that an unusable pattern throws at the call
	///     instead of only when the returned task is awaited.
	/// </remarks>
	public ValueTask<int> CountOccurrences(string actual, string expected)
	{
		actual = Normalize(actual);
		expected = Normalize(expected);
		Regex? regex = ValidatePattern(expected);
		int? count = expected.Length == 0 ? 0 : CountOccurrencesWithoutWindow(actual, expected, regex);
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
	private async ValueTask<int> CountOccurrencesWithWindow(string actual, string expected)
	{
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

	/// <summary>
	///     Specifies a new <see cref="IStringMatchType" /> to use for matching two strings.
	/// </summary>
	public void SetMatchType(IStringMatchType matchType) => _matchType = matchType;

	/// <summary>
	///     Get the expectations text.
	/// </summary>
	public string GetExpectation(string? expected, ExpectationGrammars grammars)
		=> _matchType.GetExpectation(expected, grammars) + GetOptionString();

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
		expected = NormalizeLines(expected);

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
		expected = TrimWhiteSpace(expected);

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
				$"{it} had {member} {Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())} which {failure.Substring(member.Length + 1)}";
		}

		return failure;
	}

	/// <summary>
	///     Ignores casing when comparing the <see langword="string" />s.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     A custom comparer is already set via <see cref="Using(IEqualityComparer{string})" />.
	/// </exception>
	public StringEqualityOptions IgnoringCase(bool ignoreCase = true)
	{
		if (ignoreCase && _comparer is not null)
		{
			throw CaseAndComparerConflict();
		}

		_ignoreCase = ignoreCase;
		return this;
	}

	/// <summary>
	///     Ignores the indentation when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreIndentation" /> parameter.
	/// </summary>
	/// <remarks>
	///     Enabling this option will remove the leading white-space from every line and replace all occurrences of
	///     <c>\r\n</c> and <c>\r</c> with <c>\n</c> in the strings before comparing them, which makes
	///     <see cref="IgnoringNewlineStyle(bool)" /> redundant.<br />
	///     Any Unicode white-space counts as indentation, e.g. also a non-breaking space.<br />
	///     Trailing white-space within a line is kept, but a line that consists only of white-space becomes empty.<br />
	///     The expected value is transformed as well, which also applies to wildcard and regex patterns.
	/// </remarks>
	public StringEqualityOptions IgnoringIndentation(bool ignoreIndentation = true)
	{
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
	public StringEqualityOptions IgnoringNewlineStyle(bool ignoreNewlineStyle = true)
	{
		_ignoreNewlineStyle = ignoreNewlineStyle;
		return this;
	}

	/// <summary>
	///     Ignores leading white-space when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreLeadingWhiteSpace" /> parameter.
	/// </summary>
	/// <remarks>
	///     Note:<br />
	///     This affects the index of first mismatch, as the removed whitespace is also ignored for the index calculation!
	/// </remarks>
	public StringEqualityOptions IgnoringLeadingWhiteSpace(bool ignoreLeadingWhiteSpace = true)
	{
		_ignoreLeadingWhiteSpace = ignoreLeadingWhiteSpace;
		return this;
	}

	/// <summary>
	///     Ignores trailing white-space when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreTrailingWhiteSpace" /> parameter.
	/// </summary>
	public StringEqualityOptions IgnoringTrailingWhiteSpace(bool ignoreTrailingWhiteSpace = true)
	{
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
	///     Removes the white-space at the start and the end of the <paramref name="value" />.
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
	///     Counts the lines and the columns in the last of these lines that are covered by the leading white-space
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
		string? patternKind = _matchType switch
		{
			PrefixMatchType => "prefix",
			SuffixMatchType => "suffix",
			RegexMatchType => "regex pattern",
			WildcardMatchType => "wildcard pattern",
			_ => null,
		};
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
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException($"The '{_parameterName}' {patternKind} cannot be empty.",
				_parameterName));
		}

		if (_matchType is not RegexMatchType regexMatchType)
		{
			return null;
		}

		try
		{
			return regexMatchType.CreateRegex(expected, _ignoreCase);
		}
		catch (ArgumentException exception) when (exception is not ArgumentOutOfRangeException)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException(
				$"The '{_parameterName}' regex pattern is invalid: {exception.Message}", _parameterName, exception));
		}
	}
}
