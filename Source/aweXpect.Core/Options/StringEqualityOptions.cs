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
	private IEqualityComparer<string>? _comparer;
	private bool _ignoreCase;
	private bool _ignoreIndentation;
	private bool _ignoreLeadingWhiteSpace;
	private bool _ignoreNewlineStyle;
	private bool _ignoreTrailingWhiteSpace;
	private IStringMatchType _matchType = ExactMatch;

	/// <summary>
	///     Indicates whether the current match type inspects the content of the subject instead of comparing it as a value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> subject has no content to inspect, so an inspecting match type must fail for it in both
	///     polarities, exactly like the dedicated <c>StartsWith</c> / <c>DoesNotStartWith</c> expectations do.
	/// </remarks>
	public bool InspectsSubject => _matchType is not ExactMatchType;

	/// <inheritdoc />
#if NET8_0_OR_GREATER
	public async ValueTask<bool> AreConsideredEqual<TExpected>(string? actual, TExpected expected)
#else
	public async Task<bool> AreConsideredEqual<TExpected>(string? actual, TExpected expected)
#endif
	{
		bool result;
		if (expected is not string expectedString)
		{
			ValidatePattern(null);
			result = await _matchType.AreConsideredEqual(actual, null, _ignoreCase,
				_comparer);
			return result;
		}

		expectedString = Normalize(expectedString);
		ValidatePattern(expectedString);
		try
		{
			result = await _matchType.AreConsideredEqual(Normalize(actual), expectedString, _ignoreCase,
				_comparer);
		}
		catch (RegexMatchTimeoutException exception)
		{
			throw CreateTimeoutException(expectedString, exception);
		}

		return result;
	}

	/// <summary>
	///     Counts how often the <paramref name="expected" /> <see langword="string" /> occurs
	///     in the <paramref name="actual" /> <see langword="string" />.
	/// </summary>
	/// <remarks>
	///     Both strings are normalized once before the comparison, so that the options which change the length of the
	///     strings are applied to the complete strings and not to the individual substrings that are compared.<br />
	///     Returns <c>0</c> when the <paramref name="expected" /> <see langword="string" /> is empty after the
	///     normalization.
	/// </remarks>
#if NET8_0_OR_GREATER
	public async ValueTask<int> CountOccurrences(string actual, string expected)
#else
	public async Task<int> CountOccurrences(string actual, string expected)
#endif
	{
		actual = Normalize(actual);
		expected = Normalize(expected);
		ValidatePattern(expected);
		if (expected.Length == 0)
		{
			return 0;
		}

		// A block spans whole lines, so its occurrences cannot be found with a window of the expected length.
		if (_matchType is BlockMatchType)
		{
			return BlockMatchType.CountOccurrences(actual, expected, _comparer ?? UseDefaultComparer(_ignoreCase));
		}

		try
		{
			// A pattern can match a different number of characters than it is long, so its occurrences cannot be found
			// with a window of the expected length.
			if (_matchType is RegexMatchType regexMatchType)
			{
				return RegexMatchType.CountOccurrences(actual, expected, _ignoreCase, regexMatchType.Options);
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
	///     If set to <see langword="null" /> (default), uses the <see cref="StringComparer.Ordinal" /> or
	///     <see cref="StringComparer.OrdinalIgnoreCase" /> depending on whether the casing is ignored.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The casing is already ignored via <see cref="IgnoringCase(bool)" />, or the expected value is matched as a
	///     regex or wildcard pattern.
	/// </exception>
	public StringEqualityOptions Using(IEqualityComparer<string>? comparer)
	{
		if (comparer is not null)
		{
			if (_ignoreCase)
			{
				throw CaseAndComparerConflict();
			}

			if (_matchType is RegexMatchType or WildcardMatchType)
			{
				throw ComparerAndPatternConflict();
			}
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
			nameof(expected), innerException));

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
				tokens.Add("white-space");
				break;
			case (true, false):
				tokens.Add("leading white-space");
				break;
			case (false, true):
				tokens.Add("trailing white-space");
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
	///     Verifies that the <paramref name="expected" /> value is a usable pattern for the current match type.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> pattern matches no value and an empty regex, prefix or suffix pattern matches every
	///     value, so such an expectation says nothing about the subject. This can only be detected while the expectation
	///     is verified, because the match type can also be set after the pattern.
	/// </remarks>
	private void ValidatePattern(string? expected)
	{
		if (expected?.Length == 0 && _matchType is PrefixMatchType or SuffixMatchType)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException(
				$"The 'expected' {(_matchType is PrefixMatchType ? "prefix" : "suffix")} cannot be empty.",
				nameof(expected)));
		}

		bool isRegex = _matchType is RegexMatchType;
		if (!isRegex && _matchType is not WildcardMatchType)
		{
			return;
		}

		if (expected is null)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentNullException(nameof(expected),
				$"The 'expected' {(isRegex ? "regex" : "wildcard")} pattern cannot be null."));
		}

		if (isRegex && expected.Length == 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The 'expected' regex pattern cannot be empty.",
				nameof(expected)));
		}
	}
}
