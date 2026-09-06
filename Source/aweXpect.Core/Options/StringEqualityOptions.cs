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

		result = await AreConsideredEqualNormalized(Normalize(actual), Normalize(expectedString));
		return result;
	}

	/// <summary>
	///     Counts how often the <paramref name="expected" /> <see langword="string" /> occurs
	///     in the <paramref name="actual" /> <see langword="string" />.
	/// </summary>
	/// <remarks>
	///     Both strings are normalized once before the comparison, so that options which change the length of the
	///     strings (<see cref="IgnoringNewlineStyle(bool)" /> and <see cref="IgnoringIndentation(bool)" />) are
	///     applied to the complete strings and not to the individual substrings that are compared.
	/// </remarks>
#if NET8_0_OR_GREATER
	public async ValueTask<int> CountOccurrences(string actual, string expected)
#else
	public async Task<int> CountOccurrences(string actual, string expected)
#endif
	{
		actual = Normalize(actual);
		expected = Normalize(expected);
		if (expected.Length == 0 || expected.Length > actual.Length)
		{
			return 0;
		}

		int count = 0;
		int index = 0;
		while (index < actual.Length)
		{
			if (await AreConsideredEqualNormalized(
				    actual.Substring(index, Math.Min(expected.Length, actual.Length - index)),
				    expected))
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

		int[]? ignoredColumnsPerLine = null;
		if (_ignoreIndentation)
		{
			if (actual is not null)
			{
				ignoredColumnsPerLine = GetIgnoredColumnsPerLine(actual);
			}

			actual = actual.RemoveIndentation();
			expected = expected.RemoveIndentation();
		}

		StringDifferenceSettings? settings = null;
		if (ignoredColumnsPerLine is not null)
		{
			settings = new StringDifferenceSettings(0, 0, ignoredColumnsPerLine);
		}

		if (_ignoreLeadingWhiteSpace && actual is not null)
		{
			int ignoredLineCount = 0;
			int ignoredColumnCount = 0;
			foreach (char c in actual)
			{
				if (c == '\n')
				{
					ignoredLineCount++;
					ignoredColumnCount = 0;
				}
				else if (char.IsWhiteSpace(c))
				{
					ignoredColumnCount++;
				}
				else
				{
					break;
				}
			}

			settings = new StringDifferenceSettings(ignoredLineCount, ignoredColumnCount, ignoredColumnsPerLine);
		}

		if (_ignoreNewlineStyle)
		{
			actual = actual.RemoveNewlineStyle();
			expected = expected.RemoveNewlineStyle();
		}

		if (_ignoreLeadingWhiteSpace)
		{
			actual = actual?.TrimStart();
			expected = expected?.TrimStart();
		}

		if (_ignoreTrailingWhiteSpace)
		{
			actual = actual?.TrimEnd();
			expected = expected?.TrimEnd();
		}


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
	///     Applies the options that transform the <paramref name="value" /> as a whole.
	/// </summary>
	/// <remarks>
	///     Removing the indentation also normalizes the newline style, so it supersedes
	///     <see cref="IgnoringNewlineStyle(bool)" />.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(value))]
	private string? Normalize(string? value)
	{
		if (_ignoreIndentation)
		{
			return value.RemoveIndentation();
		}

		return _ignoreNewlineStyle ? value.RemoveNewlineStyle() : value;
	}

	/// <summary>
	///     Compares two strings that were already passed through <see cref="Normalize" />.
	/// </summary>
#if NET8_0_OR_GREATER
	private async ValueTask<bool> AreConsideredEqualNormalized(string? actual, string? expected)
#else
	private async Task<bool> AreConsideredEqualNormalized(string? actual, string? expected)
#endif
	{
		if (_ignoreLeadingWhiteSpace)
		{
			actual = actual?.TrimStart();
			expected = expected?.TrimStart();
		}

		if (_ignoreTrailingWhiteSpace)
		{
			actual = actual?.TrimEnd();
			expected = expected?.TrimEnd();
		}

		return await _matchType.AreConsideredEqual(actual, expected, _ignoreCase, _comparer);
	}

	/// <summary>
	///     Counts the leading white-space characters of every line in the <paramref name="value" />.
	/// </summary>
	private static int[] GetIgnoredColumnsPerLine(string value)
	{
		string[] lines = value.RemoveNewlineStyle().Split('\n');
		int[] result = new int[lines.Length];
		for (int i = 0; i < lines.Length; i++)
		{
			result[i] = lines[i].Length - lines[i].TrimStart().Length;
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
		sb.Append(initialString.Contains("ignoring")
			? tokens.Count == 1 ? " and" : ","
			: " ignoring");

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
