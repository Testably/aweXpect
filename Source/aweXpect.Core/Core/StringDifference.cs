using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Core;

/// <summary>
///     Highlights the string difference between the <paramref name="actual" /> and
///     <paramref name="expected" /> values.
/// </summary>
/// <remarks>
///     If no <paramref name="comparer" /> is specified, uses the <see cref="StringComparer.Ordinal" /> string comparer.
/// </remarks>
public sealed class StringDifference(
	string? actual,
	string? expected,
	IEqualityComparer<string>? comparer = null,
	StringDifferenceSettings? settings = null)
{
	/// <summary>
	///     The supported match types in the <see cref="StringDifference" />.
	/// </summary>
	public enum MatchType
	{
		/// <summary>
		///     The strings are compared for equality.
		/// </summary>
		Equality,

		/// <summary>
		///     The expected string is treated as a wildcard pattern.
		/// </summary>
		Wildcard,

		/// <summary>
		///     The expected string is treated as a regex pattern.
		/// </summary>
		Regex,

		/// <summary>
		///     The expected string is treated as a prefix.
		/// </summary>
		/// <remarks>
		///     The actual string has to start with expected string.
		/// </remarks>
		Prefix,

		/// <summary>
		///     The expected string is treated as a suffix.
		/// </summary>
		/// <remarks>
		///     The actual string has to end with expected string.
		/// </remarks>
		Suffix,
	}

	private const string ActualIndicator = " (actual)";

	private readonly IEqualityComparer<string> _comparer = comparer ?? StringComparer.Ordinal;
	private int? _indexOfFirstMismatch;

	/// <summary>
	///     Returns the first index at which the two values do not match exactly.
	/// </summary>
	public int IndexOfFirstMismatch(MatchType matchType)
	{
		if (matchType is MatchType.Wildcard or MatchType.Regex)
		{
			return 0;
		}

		_indexOfFirstMismatch ??= GetIndexOfFirstMismatch(actual, expected, _comparer, matchType is MatchType.Suffix);
		return _indexOfFirstMismatch.Value;
	}

	/// <inheritdoc />
	public override string ToString() => ToString("differs");

	/// <summary>
	///     Writes a string representation of the difference, starting with the <paramref name="prefix" />.
	/// </summary>
	/// <param name="prefix">The prefix, e.g. <c>differs at index</c></param>
	public string ToString(string prefix)
	{
		const char arrowDown = '\u2193';
		const char arrowUp = '\u2191';
		const string linePrefix = "  \"";
		const string suffix = "\"";

		if (actual == null)
		{
			StringBuilder sb = new();
			sb.Append(prefix).AppendLine(":");
			sb.Append("  ").Append(arrowDown).AppendLine(ActualIndicator);
			sb.AppendLine("  <null>");
			AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(sb, linePrefix, expected!,
				0, 0, suffix);
			sb.Append("  ").Append(arrowUp).Append(GetExpected(settings?.MatchType));
			return sb.ToString();
		}

		if (expected == null)
		{
			StringBuilder sb = new();
			sb.Append(prefix).AppendLine(":");
			sb.Append("  ").Append(arrowDown).AppendLine(ActualIndicator);
			AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(sb, linePrefix, actual!,
				0, 0, suffix);
			sb.AppendLine("  <null>");
			sb.Append("  ").Append(arrowUp).Append(GetExpected(settings?.MatchType));
			return sb.ToString();
		}

		return settings?.MatchType switch
		{
			MatchType.Wildcard => ToPatternString(MatchType.Wildcard, prefix, actual, expected),
			MatchType.Regex => ToPatternString(MatchType.Regex, prefix, actual, expected),
			_ => ToEqualityString(prefix, actual, expected,
				IndexOfFirstMismatch(settings?.MatchType ?? MatchType.Equality), settings),
		};
	}

	private static string ToEqualityString(string prefix, string actual, string expected, int indexOfFirstMismatch,
		StringDifferenceSettings? settings)
	{
		const char arrowDown = '\u2193';
		const char arrowUp = '\u2191';
		const string linePrefix = "  \"";
		const string suffix = "\"";

		if (indexOfFirstMismatch < 0)
		{
			if (settings?.MatchType == MatchType.Suffix &&
			    actual.Length < expected.Length)
			{
				return
					$"is shorter than the expected length of {expected.Length} and misses the prefix:{Environment.NewLine}" +
					$"  \"{expected[..^actual.Length].TruncateWithEllipsis(Customize.aweXpect.Formatting().MaximumStringLength.Get()).Escape()}\"";
			}

			return prefix;
		}

		int indexFromEnd = actual.Length - indexOfFirstMismatch;
		StringBuilder sb = new();
		int trimStart = settings?.MatchType == MatchType.Suffix
			? GetStartIndexOfPhraseToShowBeforeTheMismatchingIndexFromEnd(actual, indexFromEnd)
			: GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(actual, indexOfFirstMismatch);

		int whiteSpaceCountBeforeArrow = indexOfFirstMismatch - trimStart + linePrefix.Length;

		if (trimStart > 0)
		{
			whiteSpaceCountBeforeArrow++;
		}

		string visibleText = actual[trimStart..indexOfFirstMismatch];
		whiteSpaceCountBeforeArrow += visibleText.Escape().Length - visibleText.Length;

		int lineNumber = CountLineFeeds(actual, indexOfFirstMismatch);
		if (settings is not null)
		{
			lineNumber += settings.IgnoredTrailingLines;
		}

		int column = GetIgnoredColumns(settings, lineNumber);

		if (settings?.IgnoredTrailingLines > 0 || actual.Contains('\n', StringComparison.Ordinal))
		{
			int indexOfLastNewlineBeforeMismatch = indexOfFirstMismatch > 0
				? actual.LastIndexOf('\n', indexOfFirstMismatch - 1)
				: -1;
			column += indexOfFirstMismatch - indexOfLastNewlineBeforeMismatch;
			sb.Append(prefix).Append(" on line ").Append(lineNumber + 1).Append(" and column ")
				.Append(column).AppendLine(":");
		}
		else
		{
			sb.Append(prefix).Append(" at index ").Append(indexOfFirstMismatch + column).AppendLine(":");
		}

		if (settings?.MatchType == MatchType.Suffix)
		{
			int trimStartExpected =
				GetStartIndexOfPhraseToShowBeforeTheMismatchingIndexFromEnd(expected, indexFromEnd);
			string actualText = CreatePrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(linePrefix, actual,
				trimStart, indexFromEnd, suffix);
			string expectedText = CreatePrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(linePrefix, expected,
				trimStartExpected, indexFromEnd, suffix);
			int actualIndentation = Math.Max(0, expectedText.Length - actualText.Length);
			sb.Append(' ', whiteSpaceCountBeforeArrow + actualIndentation).Append(arrowDown)
				.AppendLine(ActualIndicator);
			sb.Append(' ', actualIndentation);
			sb.Append(actualText);
			sb.Append(' ', Math.Max(0, actualText.Length - expectedText.Length));
			sb.Append(expectedText);
			sb.Append(' ', whiteSpaceCountBeforeArrow + actualIndentation).Append(arrowUp)
				.Append(GetExpected(settings.MatchType));
		}
		else
		{
			sb.Append(' ', whiteSpaceCountBeforeArrow).Append(arrowDown).AppendLine(ActualIndicator);
			AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(sb, linePrefix, actual,
				trimStart, indexOfFirstMismatch, suffix);
			AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(sb, linePrefix, expected,
				trimStart, indexOfFirstMismatch, suffix);
			sb.Append(' ', whiteSpaceCountBeforeArrow).Append(arrowUp).Append(GetExpected(settings?.MatchType));
		}

		return sb.ToString();
	}

	/// <summary>
	///     Counts the <c>\n</c> characters in the first <paramref name="length" /> characters of the
	///     <paramref name="value" />.
	/// </summary>
	private static int CountLineFeeds(string value, int length)
	{
		int count = 0;
		int index = value.IndexOf('\n', 0, length);
		while (index >= 0)
		{
			count++;
			index = value.IndexOf('\n', index + 1, length - index - 1);
		}

		return count;
	}

	/// <summary>
	///     Gets the number of columns that are ignored in the line with the given <paramref name="lineNumber" />.
	/// </summary>
	private static int GetIgnoredColumns(StringDifferenceSettings? settings, int lineNumber)
	{
		if (settings is null)
		{
			return 0;
		}

		if (settings.IgnoredColumnsPerLine is { } ignoredColumnsPerLine)
		{
			return lineNumber < ignoredColumnsPerLine.Count ? ignoredColumnsPerLine[lineNumber] : 0;
		}

		return lineNumber == settings.IgnoredTrailingLines ? settings.IgnoredTrailingColumns : 0;
	}

	private static string ToPatternString(MatchType matchType, string prefix, string actual, string expected)
	{
		const char arrowDown = '\u2193';
		const char arrowUp = '\u2191';

		StringBuilder sb = new();
		sb.Append(prefix).AppendLine(":");
		sb.Append("  ").Append(arrowDown).AppendLine(ActualIndicator);
		sb.Append("  ");
		Formatter.Format(sb, actual);
		sb.AppendLine();
		sb.Append("  ");
		Formatter.Format(sb, expected);
		sb.AppendLine();
		sb.Append("  ").Append(arrowUp).Append(GetExpected(matchType));
		return sb.ToString();
	}

	/// <summary>
	///     Appends the <paramref name="prefix" />, the escaped visible <paramref name="text" /> phrase decorated with ellipsis
	///     and the <paramref name="suffix" /> to the <paramref name="stringBuilder" />.
	/// </summary>
	/// <remarks>
	///     When text phrase starts at <paramref name="indexOfStartingPhrase" /> and with a calculated length omits text
	///     on start or end, an ellipsis is added.<br />
	///     The length is calculated so that at least
	///     <see cref="AwexpectCustomization.FormattingCustomization.MinimumNumberOfCharactersAfterStringDifference" />
	///     characters follow the <paramref name="indexOfFirstMismatch" />.
	/// </remarks>
	private static void AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(
		StringBuilder stringBuilder,
		string prefix, string text, int indexOfStartingPhrase, int indexOfFirstMismatch, string suffix)
	{
		int minimumNumberOfCharactersAfterMismatch = Math.Max(0,
			Customize.aweXpect.Formatting().MinimumNumberOfCharactersAfterStringDifference.Get());
		int subjectLength = GetLengthOfPhraseToShowOrDefaultLength(text, indexOfStartingPhrase,
			indexOfFirstMismatch - indexOfStartingPhrase + minimumNumberOfCharactersAfterMismatch);
		const char ellipsis = '\u2026';

		stringBuilder.Append(prefix);

		if (indexOfStartingPhrase > 0)
		{
			stringBuilder.Append(ellipsis);
		}

		stringBuilder.Append(text
			.Substring(indexOfStartingPhrase, subjectLength).Escape());

		if (text.Length > indexOfStartingPhrase + subjectLength)
		{
			stringBuilder.Append(ellipsis);
		}

		stringBuilder.AppendLine(suffix);
	}

	/// <summary>
	///     Creates the escaped visible <paramref name="text" /> phrase decorated with ellipsis, with
	///     the <paramref name="prefix" /> and the <paramref name="suffix" />.
	/// </summary>
	/// <remarks>
	///     When text phrase starts at <paramref name="indexOfStartingPhrase" /> and with a calculated length omits text
	///     on start or end, an ellipsis is added.
	/// </remarks>
	private static string CreatePrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(string prefix, string text,
		int indexOfStartingPhrase, int indexFromEnd, string suffix)
	{
		StringBuilder? stringBuilder = new();
		int indexOfFirstMismatch = text.Length - indexFromEnd;
		int minLength = indexOfFirstMismatch + 10 - indexOfStartingPhrase;
		int subjectLength = GetLengthOfPhraseToShowOrDefaultLength(text, indexOfStartingPhrase, minLength);
		const char ellipsis = '\u2026';

		stringBuilder.Append(prefix);

		if (indexOfStartingPhrase > 0)
		{
			stringBuilder.Append(ellipsis);
		}

		stringBuilder.Append(text
			.Substring(indexOfStartingPhrase, subjectLength).Escape());

		if (text.Length > indexOfStartingPhrase + subjectLength)
		{
			stringBuilder.Append(ellipsis);
		}

		stringBuilder.AppendLine(suffix);
		return stringBuilder.ToString();
	}

	private static int GetIndexOfFirstMismatch(string? actualValue, string? expectedValue,
		IEqualityComparer<string> comparer, bool fromEnd = false)
	{
		if (comparer.Equals(actualValue, expectedValue))
		{
			return -1;
		}

		if (actualValue is null || expectedValue is null)
		{
			return 0;
		}

		StringComparison? comparison = GetComparison(comparer);
		int maxCommonLength = Math.Min(actualValue.Length, expectedValue.Length);
		int min = 0;
		int max = maxCommonLength + 1;
		while (min < max)
		{
			int mid = (min + max) / 2;
			if (mid == min)
			{
				break;
			}

			if (AreEqual(actualValue, expectedValue, mid, fromEnd, comparer, comparison))
			{
				min = mid;
			}
			else
			{
				max = mid;
			}
		}

		return fromEnd ? actualValue.Length - min - 1 : min;
	}

	/// <summary>
	///     The comparison that decides like the <paramref name="comparer" />, when it is one of the ordinal or culture-aware
	///     comparers of <see cref="StringComparer" />.
	/// </summary>
	/// <remarks>
	///     A culture-aware comparer is recognized by its equality to the well-known comparer, which compares the culture
	///     and the options, so that this does not call into a custom comparer.
	/// </remarks>
	private static StringComparison? GetComparison(IEqualityComparer<string> comparer)
	{
		if (ReferenceEquals(comparer, StringComparer.Ordinal))
		{
			return StringComparison.Ordinal;
		}

		if (ReferenceEquals(comparer, StringComparer.OrdinalIgnoreCase))
		{
			return StringComparison.OrdinalIgnoreCase;
		}

		if (StringComparer.InvariantCulture.Equals(comparer))
		{
			return StringComparison.InvariantCulture;
		}

		if (StringComparer.InvariantCultureIgnoreCase.Equals(comparer))
		{
			return StringComparison.InvariantCultureIgnoreCase;
		}

		if (StringComparer.CurrentCulture.Equals(comparer))
		{
			return StringComparison.CurrentCulture;
		}

		if (StringComparer.CurrentCultureIgnoreCase.Equals(comparer))
		{
			return StringComparison.CurrentCultureIgnoreCase;
		}

		return null;
	}

	/// <summary>
	///     Compares the first or last <paramref name="length" /> characters of both values.
	/// </summary>
	/// <remarks>
	///     With a <paramref name="comparison" />, the ranges are compared without copying them: a substring per step of
	///     the search allocates about twice the length of the values for every halving, which adds up to gigabytes for
	///     long values. A comparison of the ranges decides exactly like the matching comparer does for the substrings.
	/// </remarks>
	private static bool AreEqual(string actualValue, string expectedValue, int length, bool fromEnd,
		IEqualityComparer<string> comparer, StringComparison? comparison)
	{
		if (comparison is null)
		{
			return fromEnd
				? comparer.Equals(actualValue[^length..], expectedValue[^length..])
				: comparer.Equals(actualValue[..length], expectedValue[..length]);
		}

		return fromEnd
			? string.Compare(actualValue, actualValue.Length - length, expectedValue, expectedValue.Length - length,
				length, comparison.Value) == 0
			: string.Compare(actualValue, 0, expectedValue, 0, length, comparison.Value) == 0;
	}

	/// <summary>
	///     Calculates how many characters to keep in <paramref name="text" /> from the <paramref name="startIndex" /> on.
	/// </summary>
	/// <remarks>
	///     If a word end is found between <paramref name="minLength" /> and 15 characters more, use this word end,
	///     otherwise keep 5 characters more than <paramref name="minLength" />, or one more, so that a surrogate pair or a
	///     <c>\r\n</c> line break is not split.
	/// </remarks>
	private static int GetLengthOfPhraseToShowOrDefaultLength(string text, int startIndex, int minLength)
	{
		int defaultLength = minLength + 5;
		int maxLength = minLength + 15;
		const int lengthOfWhitespace = 1;

		int remainingLength = text.Length - startIndex;
		int searchLength = Math.Min(maxLength + lengthOfWhitespace, remainingLength);
		int indexOfWordBoundary = searchLength > 0
			? text.LastIndexOf(' ', startIndex + searchLength - 1, searchLength)
			: -1;

		if (indexOfWordBoundary >= 0 && indexOfWordBoundary - startIndex > minLength)
		{
			return indexOfWordBoundary - startIndex;
		}

		int length = Math.Min(defaultLength, remainingLength);
		return text.IsSplitAt(startIndex + length) ? length + 1 : length;
	}

	/// <summary>
	///     Calculates the start index of the visible segment from <paramref name="value" /> when highlighting the difference
	///     at <paramref name="indexOfFirstMismatch" />.
	/// </summary>
	/// <remarks>
	///     Either keep the last 10 characters before <paramref name="indexOfFirstMismatch" /> or a word begin (separated by
	///     whitespace) between 15 and 5 characters before <paramref name="indexOfFirstMismatch" />.<br />
	///     One more character is kept, if the start would otherwise split a surrogate pair or a <c>\r\n</c> line break.
	/// </remarks>
	private static int GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(string value,
		int indexOfFirstMismatch)
	{
		const int defaultCharactersToKeep = 10;
		const int minCharactersToKeep = 5;
		const int maxCharactersToKeep = 15;
		const int lengthOfWhitespace = 1;
		const int phraseLengthToCheckForWordBoundary =
			maxCharactersToKeep - minCharactersToKeep + lengthOfWhitespace;

		if (indexOfFirstMismatch <= defaultCharactersToKeep)
		{
			return 0;
		}

		int indexToStartSearchingForWordBoundary =
			Math.Max(indexOfFirstMismatch - (maxCharactersToKeep + lengthOfWhitespace), 0);

		int indexOfWordBoundary = value
			                          .IndexOf(' ', indexToStartSearchingForWordBoundary,
				                          phraseLengthToCheckForWordBoundary) -
		                          indexToStartSearchingForWordBoundary;

		if (indexOfWordBoundary >= 0)
		{
			return indexToStartSearchingForWordBoundary + indexOfWordBoundary + lengthOfWhitespace;
		}

		int startIndex = indexOfFirstMismatch - defaultCharactersToKeep;
		return value.IsSplitAt(startIndex) ? startIndex - 1 : startIndex;
	}

	/// <summary>
	///     Calculates the start index of the visible segment from <paramref name="value" /> when highlighting the difference
	///     at <paramref name="indexFromEnd" /> from the end.
	/// </summary>
	/// <remarks>
	///     Either keep the last 10 characters before <paramref name="indexFromEnd" /> from the end or a word begin (separated
	///     by
	///     whitespace) between 15 and 5 characters before <paramref name="indexFromEnd" /> from the end.<br />
	///     One more character is kept, if the start would otherwise split a surrogate pair or a <c>\r\n</c> line break.
	/// </remarks>
	private static int GetStartIndexOfPhraseToShowBeforeTheMismatchingIndexFromEnd(string value,
		int indexFromEnd)
	{
		int minLength = Customize.aweXpect.Formatting().MinimumNumberOfCharactersAfterStringDifference.Get();
		int defaultLength = minLength + 5;
		int maxLength = minLength + 15;
		const int lengthOfWhitespace = 1;
		int phraseLengthToCheckForWordBoundary =
			maxLength - minLength + lengthOfWhitespace;

		int indexOfFirstMismatch = value.Length - indexFromEnd;
		if (indexOfFirstMismatch <= defaultLength)
		{
			return 0;
		}

		int indexToStartSearchingForWordBoundary =
			Math.Max(indexOfFirstMismatch - (maxLength + lengthOfWhitespace), 0);

		int indexOfWordBoundary = value
			                          .IndexOf(' ', indexToStartSearchingForWordBoundary,
				                          Math.Min(phraseLengthToCheckForWordBoundary,
					                          value.Length - indexToStartSearchingForWordBoundary)) -
		                          indexToStartSearchingForWordBoundary;

		if (indexOfWordBoundary >= 0)
		{
			return indexToStartSearchingForWordBoundary + indexOfWordBoundary + lengthOfWhitespace;
		}

		int startIndex = indexOfFirstMismatch - defaultLength;
		return value.IsSplitAt(startIndex) ? startIndex - 1 : startIndex;
	}

	private static string GetExpected(MatchType? matchType)
		=> matchType switch
		{
			MatchType.Wildcard => " (wildcard pattern)",
			MatchType.Regex => " (regex pattern)",
			MatchType.Prefix => " (expected prefix)",
			MatchType.Suffix => " (expected suffix)",
			_ => " (expected)",
		};
}
