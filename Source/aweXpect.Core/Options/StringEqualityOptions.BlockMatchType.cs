using System;
using System.Collections.Generic;
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
	///     The block must start and end at line boundaries. All its lines must share the same white-space prefix in the
	///     actual string, so the relative indentation within the block is still compared.<br />
	///     A line that consists only of white-space matches any line that consists only of white-space.
	/// </remarks>
	public StringEqualityOptions AsBlock()
	{
		_matchType = BlockMatch;
		return this;
	}

	private sealed class BlockMatchType : IStringMatchType
	{
		/// <summary>
		///     Counts the non-overlapping occurrences of the <paramref name="expected" /> block in the
		///     <paramref name="actual" /> lines.
		/// </summary>
		public int CountOccurrences(string actual, string expected, IEqualityComparer<string> comparer)
		{
			string[] actualLines = SplitLines(actual);
			string[] expectedLines = SplitLines(expected);
			int count = 0;
			int index = 0;
			while (index <= actualLines.Length - expectedLines.Length)
			{
				if (MatchesAt(actualLines, index, expectedLines, comparer))
				{
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

		private static string[] SplitLines(string value)
			=> value.RemoveNewlineStyle().Split('\n');

		/// <summary>
		///     Checks whether the <paramref name="expectedLines" /> match the <paramref name="actualLines" /> starting
		///     at <paramref name="index" />, when all lines are indented by the same white-space prefix.
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

				if (prefix is null)
				{
					int prefixLength = actualLine.Length - expectedLine.Length;
					if (prefixLength < 0 || !string.IsNullOrWhiteSpace(actualLine.Substring(0, prefixLength)))
					{
						return false;
					}

					prefix = actualLine.Substring(0, prefixLength);
				}
				else if (!actualLine.StartsWith(prefix, StringComparison.Ordinal))
				{
					return false;
				}

				if (!comparer.Equals(actualLine.Substring(prefix.Length), expectedLine))
				{
					return false;
				}
			}

			return true;
		}

		#region IMatchType Members

		/// <inheritdoc
		///     cref="IStringMatchType.GetExtendedFailure(string, string?, string?, bool, IEqualityComparer{string}, StringDifferenceSettings?)" />
		public string GetExtendedFailure(string it, string? actual, string? expected,
			bool ignoreCase,
			IEqualityComparer<string> comparer,
			StringDifferenceSettings? settings)
		{
			if (string.IsNullOrEmpty(actual) || expected == null)
			{
				return $"{it} was {Formatter.Format(actual)}";
			}

			return $"{it} was {Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}";
		}

		/// <inheritdoc cref="IStringMatchType.AreConsideredEqual(string?, string?, bool, IEqualityComparer{string})" />
#if NET8_0_OR_GREATER
		public ValueTask<bool>
#else
		public Task<bool>
#endif
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

#if NET8_0_OR_GREATER
			return ValueTask.FromResult(result);
#else
			return Task.FromResult(result);
#endif
		}

		/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
			{
				(true, false) =>
					$"matches {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())} as block",
				(false, false) =>
					$"matching {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())} as block",
				(true, true) =>
					$"does not match {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())} as block",
				(false, true) =>
					$"not matching {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())} as block",
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

		#endregion
	}
}
