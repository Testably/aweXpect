using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
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
			result = await _matchType.AreConsideredEqual(actual, null, _ignoreCase,
				_comparer);
			return result;
		}

		result = await _matchType.AreConsideredEqual(Normalize(actual), Normalize(expectedString), _ignoreCase,
			_comparer);
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
		if (expected.Length == 0)
		{
			return 0;
		}

		// A block spans whole lines, so its occurrences cannot be found with a window of the expected length.
		if (_matchType is BlockMatchType blockMatchType)
		{
			return blockMatchType.CountOccurrences(actual, expected, _comparer ?? UseDefaultComparer(_ignoreCase));
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
	public StringEqualityOptions IgnoringCase(bool ignoreCase = true)
	{
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
	public StringEqualityOptions UsingComparer(IEqualityComparer<string>? comparer)
	{
		_comparer = comparer;
		return this;
	}

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
}
