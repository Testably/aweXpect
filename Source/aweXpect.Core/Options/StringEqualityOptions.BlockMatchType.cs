using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	private static readonly BlockMatchType BlockMatch = new();

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a block of lines, which may be indented as a whole.
	/// </summary>
	/// <remarks>
	///     The block must start and end at line boundaries. All its lines must share the same whitespace prefix in the
	///     actual string, so the relative indentation within the block is still compared.<br />
	///     A line that consists only of whitespace matches any line that consists only of whitespace.<br />
	///     The newline style is always ignored, and a single trailing line terminator does not start a new line,
	///     so <c>"a\nb\n"</c> has the same two lines as <c>"a\nb"</c>.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     A match type is already specified, or an option that changes the lines, e.g. the indentation, which the
	///     block compares on its own.
	/// </exception>
	public StringEqualityOptions AsBlock()
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_matchTypeOption ?? _lineOption, nameof(AsBlock));
		SetMatchType(BlockMatch, nameof(AsBlock));
		return this;
	}

	private sealed class BlockMatchType : IStringMatchType
	{
		/// <summary>
		///     The most lines of the subject times the lines of the block that are compared to find the closest match, so
		///     that explaining a failure in a long subject stays cheap.
		/// </summary>
		private const int MaximumLineComparisons = 100_000;

		/// <summary>
		///     The most occurrences whose lines are listed in a failure.
		/// </summary>
		private const int MaximumListedOccurrences = 10;

		/// <summary>
		///     Counts the non-overlapping occurrences of the <paramref name="expected" /> block in the
		///     <paramref name="actual" /> lines, up to the <paramref name="maximum" />.
		/// </summary>
		public static int CountOccurrences(string actual, string expected, IEqualityComparer<string> comparer,
			int maximum)
			=> FindOccurrences(SplitLines(actual), SplitLines(expected), comparer, maximum, null);

		/// <summary>
		///     Names the lines of the first <paramref name="count" /> occurrences of the <paramref name="expected" /> block
		///     in the <paramref name="actual" /> lines, or without an occurrence, explains how the closest match differs.
		/// </summary>
		/// <remarks>
		///     The closest match is the first one in which the content of every line matches when the indentation is
		///     ignored, else where the text of a block that is not blank occurs inside of lines, else the first window with
		///     the most matching lines that are not blank. A subject that is too long to compare every window with the block
		///     is only searched for the text of the block. A subject with fewer lines than the block is compared with it from
		///     its first line on.
		/// </remarks>
		public static string GetOccurrenceDetails(string actual, string expected, int count,
			IEqualityComparer<string> comparer)
		{
			string[] actualLines = SplitLines(actual);
			string[] expectedLines = SplitLines(expected);
			if (count > 0)
			{
				List<int> starts = [];
				FindOccurrences(actualLines, expectedLines, comparer, Math.Min(count, MaximumListedOccurrences), starts);
				return starts.Count == 0
					? ""
					: " at " + FormatLines(starts, expectedLines.Length, count > MaximumListedOccurrences);
			}

			int windows = actualLines.Length - expectedLines.Length + 1;
			if (windows <= 0)
			{
				return DescribeShorterSubject(actualLines, expectedLines, comparer);
			}

			bool canCompareAllWindows = (long)actualLines.Length * expectedLines.Length <= MaximumLineComparisons;
			if (canCompareAllWindows)
			{
				int start = IndexOfMatchIgnoringIndentation(actualLines, windows, expectedLines, comparer);
				if (start >= 0)
				{
					return DescribeClosestMatch(actualLines, start, expectedLines, comparer);
				}
			}

			StringComparison? comparison = comparer.GetComparison();
			if (comparison is not null && !string.IsNullOrWhiteSpace(expected))
			{
				string text = string.Join("\n", actualLines);
				int index = text.IndexOf(string.Join("\n", expectedLines), comparison.Value);
				if (index >= 0)
				{
					return $", only as part of {FormatLines([text.CountLineFeeds(index),], expectedLines.Length, false)}";
				}
			}

			if (!canCompareAllWindows)
			{
				return "";
			}

			int closestStart = IndexOfClosestMatch(actualLines, windows, expectedLines, comparer);
			return closestStart < 0 ? "" : DescribeClosestMatch(actualLines, closestStart, expectedLines, comparer);
		}

		/// <summary>
		///     The first of the <paramref name="windows" /> in which the content of every line matches when the indentation
		///     is ignored, or <c>-1</c>.
		/// </summary>
		private static int IndexOfMatchIgnoringIndentation(string[] actualLines, int windows, string[] expectedLines,
			IEqualityComparer<string> comparer)
		{
			for (int start = 0; start < windows; start++)
			{
				if (MatchesIgnoringIndentation(actualLines, start, expectedLines, comparer))
				{
					return start;
				}
			}

			return -1;
		}

		/// <summary>
		///     The first of the <paramref name="windows" /> with the most matching lines that are not blank, or <c>-1</c>
		///     when no such line matches.
		/// </summary>
		private static int IndexOfClosestMatch(string[] actualLines, int windows, string[] expectedLines,
			IEqualityComparer<string> comparer)
		{
			int closestStart = -1;
			int closestScore = 0;
			for (int start = 0; start < windows; start++)
			{
				int score = CountMatchingLines(actualLines, start, expectedLines, comparer);
				if (score > closestScore)
				{
					closestStart = start;
					closestScore = score;
				}
			}

			return closestStart;
		}

		private static int FindOccurrences(string[] actualLines, string[] expectedLines,
			IEqualityComparer<string> comparer, int maximum, List<int>? starts)
		{
			int count = 0;
			int index = 0;
			while (index <= actualLines.Length - expectedLines.Length && count < maximum)
			{
				if (MatchesAt(actualLines, index, expectedLines, comparer))
				{
					starts?.Add(index);
					count++;
					index += expectedLines.Length;
				}
				else
				{
					index++;
				}
			}

			return count;
		}

		private static string DescribeClosestMatch(string[] actualLines, int start, string[] expectedLines,
			IEqualityComparer<string> comparer)
			=> ", and the closest match at " + FormatLines([start,], expectedLines.Length, false) + " " +
			   DescribeFirstMismatch(actualLines, start, expectedLines, expectedLines.Length, comparer);

		/// <summary>
		///     Describes how the <paramref name="actualLines" />, which are fewer than the <paramref name="expectedLines" />,
		///     differ from the start of the block, or else that the block does not fit into them.
		/// </summary>
		private static string DescribeShorterSubject(string[] actualLines, string[] expectedLines,
			IEqualityComparer<string> comparer)
		{
			if (actualLines.Length == 0)
			{
				return "";
			}

			string? mismatch = DescribeFirstMismatch(actualLines, 0, expectedLines, actualLines.Length, comparer);
			if (mismatch is not null)
			{
				return $", which {mismatch}";
			}

			string lines = actualLines.Length == 1 ? "line" : "lines";
			return $", which has {actualLines.Length} {lines} instead of at least {expectedLines.Length}";
		}

		/// <summary>
		///     Describes the first of the <paramref name="length" /> lines from <paramref name="start" /> on, at which the
		///     <paramref name="actualLines" /> stop matching the <paramref name="expectedLines" /> by the same rules as
		///     <see cref="MatchesAt" />, or returns <see langword="null" /> when all of them match.
		/// </summary>
		private static string? DescribeFirstMismatch(string[] actualLines, int start, string[] expectedLines,
			int length, IEqualityComparer<string> comparer)
		{
			string? prefix = null;
			for (int i = 0; i < length; i++)
			{
				string expectedLine = expectedLines[i];
				string actualLine = actualLines[start + i];
				if (string.IsNullOrWhiteSpace(expectedLine))
				{
					if (!string.IsNullOrWhiteSpace(actualLine))
					{
						return
							$"has {Formatter.Format(actualLine.TruncateWithEllipsisOnWord(DefaultMaxLength))} on line {start + i + 1} instead of a blank line";
					}

					continue;
				}

				if (prefix is null
					    ? TrySplitPrefix(actualLine, expectedLine, comparer, out prefix)
					    : IsIndentedBy(actualLine, prefix, expectedLine, comparer))
				{
					continue;
				}

				if (AreEqualByComparer(comparer, actualLine.TrimStart(), expectedLine.TrimStart()))
				{
					return
						$"has line {start + i + 1} indented by \"{GetIndentation(actualLine).Escape()}\" instead of \"{(prefix + GetIndentation(expectedLine)).Escape()}\"";
				}

				return DescribeContentDifference(actualLines, start + i, expectedLines, i,
					prefix ?? GuessPrefix(actualLine, expectedLine), comparer);
			}

			return null;
		}

		/// <summary>
		///     Shows the difference between the <paramref name="actualLines" /> and the same lines in which the one at
		///     <paramref name="actualIndex" /> and all following ones are replaced by the <paramref name="expectedLines" />
		///     from <paramref name="expectedIndex" /> on, indented by the <paramref name="prefix" />.
		/// </summary>
		/// <remarks>
		///     The lines before are taken from the subject, so that the difference is found in the line at
		///     <paramref name="actualIndex" /> and its position refers to the subject.
		/// </remarks>
		private static string DescribeContentDifference(string[] actualLines, int actualIndex,
			string[] expectedLines, int expectedIndex, string prefix, IEqualityComparer<string> comparer)
		{
			StringBuilder expectedText = new();
			for (int i = 0; i < actualIndex; i++)
			{
				expectedText.Append(actualLines[i]).Append('\n');
			}

			for (int i = expectedIndex; i < expectedLines.Length; i++)
			{
				if (i > expectedIndex)
				{
					expectedText.Append('\n');
				}

				string expectedLine = expectedLines[i];
				expectedText.Append(string.IsNullOrWhiteSpace(expectedLine) ? expectedLine : prefix + expectedLine);
			}

			const string prefixText = "differs";
			string difference = new StringDifference(string.Join("\n", actualLines), expectedText.ToString(), comparer)
				.ToString(prefixText);
			return difference.Length == prefixText.Length ? $"{prefixText} on line {actualIndex + 1}" : difference;
		}

		/// <summary>
		///     Splits the <paramref name="value" /> into lines, using the same rule as <c>HasLines</c>:
		///     a single trailing line terminator does not start a new line, so <c>"a\nb\n"</c> has two lines.
		/// </summary>
		private static string[] SplitLines(string value)
		{
			if (value.Length == 0)
			{
				return [];
			}

			string[] lines = value.RemoveNewlineStyle().Split('\n');
			if (lines.Length > 1 && lines[lines.Length - 1].Length == 0)
			{
				Array.Resize(ref lines, lines.Length - 1);
			}

			return lines;
		}

		/// <summary>
		///     Checks whether the <paramref name="expectedLines" /> match the <paramref name="actualLines" /> starting
		///     at <paramref name="index" />, when all lines are indented by the same whitespace prefix.
		/// </summary>
		private static bool MatchesAt(string[] actualLines, int index, string[] expectedLines,
			IEqualityComparer<string> comparer)
		{
			string? prefix = null;
			for (int i = 0; i < expectedLines.Length; i++)
			{
				string expectedLine = expectedLines[i];
				string actualLine = actualLines[index + i];
				if (string.IsNullOrWhiteSpace(expectedLine))
				{
					if (!string.IsNullOrWhiteSpace(actualLine))
					{
						return false;
					}

					continue;
				}

				if (prefix is null
					    ? !TrySplitPrefix(actualLine, expectedLine, comparer, out prefix)
					    : !IsIndentedBy(actualLine, prefix, expectedLine, comparer))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///     Checks whether the <paramref name="actualLine" /> is the <paramref name="expectedLine" /> indented by a
		///     whitespace <paramref name="prefix" />, which the following lines of the block must share.
		/// </summary>
		private static bool TrySplitPrefix(string actualLine, string expectedLine, IEqualityComparer<string> comparer,
			[NotNullWhen(true)] out string? prefix)
		{
			int prefixLength = actualLine.Length - expectedLine.Length;
			if (prefixLength >= 0)
			{
				prefix = actualLine.Substring(0, prefixLength);
				if (string.IsNullOrWhiteSpace(prefix) &&
				    AreEqualByComparer(comparer, actualLine.Substring(prefixLength), expectedLine))
				{
					return true;
				}
			}

			prefix = null;
			return false;
		}

		private static bool IsIndentedBy(string actualLine, string prefix, string expectedLine,
			IEqualityComparer<string> comparer)
			=> actualLine.StartsWith(prefix, StringComparison.Ordinal) &&
			   AreEqualByComparer(comparer, actualLine.Substring(prefix.Length), expectedLine);

		private static bool MatchesIgnoringIndentation(string[] actualLines, int start, string[] expectedLines,
			IEqualityComparer<string> comparer)
		{
			for (int i = 0; i < expectedLines.Length; i++)
			{
				string expectedLine = expectedLines[i];
				string actualLine = actualLines[start + i];
				if (string.IsNullOrWhiteSpace(expectedLine)
					    ? !string.IsNullOrWhiteSpace(actualLine)
					    : !AreEqualByComparer(comparer, actualLine.TrimStart(), expectedLine.TrimStart()))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///     Counts the lines of the block from <paramref name="start" /> on that are not blank and match, where the
		///     first of them that matches determines the whitespace prefix of the others.
		/// </summary>
		private static int CountMatchingLines(string[] actualLines, int start, string[] expectedLines,
			IEqualityComparer<string> comparer)
		{
			string? prefix = null;
			int count = 0;
			for (int i = 0; i < expectedLines.Length; i++)
			{
				string expectedLine = expectedLines[i];
				if (string.IsNullOrWhiteSpace(expectedLine))
				{
					continue;
				}

				string actualLine = actualLines[start + i];
				if (prefix is null
					    ? TrySplitPrefix(actualLine, expectedLine, comparer, out prefix)
					    : IsIndentedBy(actualLine, prefix, expectedLine, comparer))
				{
					count++;
				}
			}

			return count;
		}

		private static string GetIndentation(string line)
			=> line.Substring(0, line.Length - line.TrimStart().Length);

		/// <summary>
		///     Guesses the whitespace prefix of the block from an <paramref name="actualLine" /> whose content differs from
		///     the <paramref name="expectedLine" />, as the indentation it has in addition.
		/// </summary>
		private static string GuessPrefix(string actualLine, string expectedLine)
		{
			string indentation = GetIndentation(actualLine);
			int length = indentation.Length - GetIndentation(expectedLine).Length;
			return length > 0 ? indentation.Substring(0, length) : "";
		}

		/// <summary>
		///     Formats the lines of a block of <paramref name="length" /> lines at each of the <paramref name="starts" />,
		///     e.g. <c>lines 1-2 and 5-6</c>.
		/// </summary>
		private static string FormatLines(List<int> starts, int length, bool isTruncated)
		{
			StringBuilder sb = new(starts.Count == 1 && length == 1 && !isTruncated ? "line " : "lines ");
			for (int i = 0; i < starts.Count; i++)
			{
				if (i > 0)
				{
					sb.Append(i == starts.Count - 1 && !isTruncated ? " and " : ", ");
				}

				sb.Append(starts[i] + 1);
				if (length > 1)
				{
					sb.Append('-').Append(starts[i] + length);
				}
			}

			if (isTruncated)
			{
				const char ellipsis = '\u2026';
				sb.Append(", ").Append(ellipsis);
			}

			return sb.ToString();
		}

		#region IMatchType Members

		/// <inheritdoc cref="IStringMatchType.InspectsSubject" />
		public bool InspectsSubject => true;

		/// <inheritdoc
		///     cref="IStringMatchType.GetExtendedFailure(string, string?, string?, bool, IEqualityComparer{string}, StringDifferenceSettings?)" />
		/// <remarks>
		///     The first line that does not match is explained before a different number of lines, because it also locates
		///     a missing or an additional line inside the block.
		/// </remarks>
		public string GetExtendedFailure(string it, string? actual, string? expected,
			bool ignoreCase,
			IEqualityComparer<string> comparer,
			StringDifferenceSettings? settings)
		{
			if (string.IsNullOrEmpty(actual) || expected == null)
			{
				return $"{it} was {Formatter.Format(actual)}";
			}

			string prefix = $"{it} was {Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength))}";
			string[] actualLines = SplitLines(actual);
			string[] expectedLines = SplitLines(expected);
			string? mismatch = DescribeFirstMismatch(actualLines, 0, expectedLines,
				Math.Min(actualLines.Length, expectedLines.Length), comparer);
			if (mismatch is not null)
			{
				return $"{prefix}, which {mismatch}";
			}

			if (actualLines.Length != expectedLines.Length)
			{
				return
					$"{prefix}, which has {actualLines.Length} {(actualLines.Length == 1 ? "line" : "lines")} instead of {expectedLines.Length}";
			}

			return prefix;
		}

		/// <inheritdoc cref="IStringMatchType.AreConsideredEqual(string?, string?, bool, IEqualityComparer{string})" />
		public ValueTask<StringMatchResult>
			AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string>? comparer)
		{
			bool result;
			if (actual is null || expected is null)
			{
				result = actual is null && expected is null;
			}
			else
			{
				string[] actualLines = SplitLines(actual);
				string[] expectedLines = SplitLines(expected);
				result = actualLines.Length == expectedLines.Length &&
				         MatchesAt(actualLines, 0, expectedLines, comparer ?? UseDefaultComparer(ignoreCase));
			}

			return new ValueTask<StringMatchResult>(result);
		}

		/// <inheritdoc cref="IStringMatchType.AppendContexts(ResultContextCollector)" />
		public void AppendContexts(ResultContextCollector contexts)
		{
			// The options are already part of the expectation.
		}

		/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.IsActive(), grammars.IsNegated()) switch
			{
				(true, false) =>
					$"{grammars.Verb("matches", "match")} {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))} as block",
				(false, false) =>
					$"matching {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))} as block",
				(true, true) =>
					$"{grammars.Verb("does not match", "do not match")} {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))} as block",
				(false, true) =>
					$"not matching {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))} as block",
			};

		/// <inheritdoc cref="IStringMatchType.GetTypeString()" />
		public string GetTypeString()
			=> " as block";

		/// <inheritdoc cref="IStringMatchType.GetOptionString(bool, IEqualityComparer{string})" />
		public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
		{
			if (comparer != null)
			{
				return $" using {Formatter.Format(comparer.GetType())}";
			}

			if (ignoreCase)
			{
				return " ignoring case";
			}

			return "";
		}

		/// <inheritdoc cref="IStringMatchType.ValidateOptions(bool, IEqualityComparer{string})" />
		public void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer)
		{
			// StringEqualityOptions rejects the options that a built-in match type cannot honour.
		}

		/// <inheritdoc cref="IStringMatchType.ValidateExpected(string?)" />
		public void ValidateExpected(string? expected)
		{
			// StringEqualityOptions validates the expected value of a built-in match type.
		}

		#endregion
	}
}
