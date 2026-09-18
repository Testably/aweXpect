using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	private static readonly IStringMatchType ExactMatch = new ExactMatchType();

	private sealed class ExactMatchType : IStringMatchType
	{
		private static int GetIndexOfFirstMatch(string stringWithLeadingWhitespace, string value,
			IEqualityComparer<string> comparer)
		{
			int indexOfFirstMatch;
			for (indexOfFirstMatch = 0;
			     indexOfFirstMatch <= stringWithLeadingWhitespace.Length - value.Length;
			     indexOfFirstMatch++)
			{
				if (comparer.Equals(stringWithLeadingWhitespace.Substring(indexOfFirstMatch, value.Length), value))
				{
					break;
				}
			}

			return indexOfFirstMatch;
		}

		#region IMatchType Members

		/// <inheritdoc
		///     cref="IStringMatchType.GetExtendedFailure(string, string?, string?, bool, IEqualityComparer{string}, StringDifferenceSettings?)" />
		public string GetExtendedFailure(string it, string? actual, string? expected,
			bool ignoreCase,
			IEqualityComparer<string> comparer,
			StringDifferenceSettings? settings)
		{
			if (actual == null || expected == null)
			{
				return $"{it} was {Formatter.Format(actual)}";
			}

			string prefix =
				$"{it} was {Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}";
			int minCommonLength = Math.Min(actual.Length, expected.Length);
			StringDifference stringDifference = new(actual, expected, comparer, settings);
			int indexOfFirstMismatch = stringDifference.IndexOfFirstMismatch(StringDifference.MatchType.Equality);
			if (indexOfFirstMismatch == 0 && comparer.Equals(actual.TrimStart(), expected))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix} which has unexpected whitespace (\"{actual.Substring(0, GetIndexOfFirstMatch(actual, expected, comparer)).DisplayWhitespace().TruncateWithEllipsis(maxStringLength)}\" at the beginning)";
			}

			if (indexOfFirstMismatch == 0 && comparer.Equals(actual, expected.TrimStart()))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix} which misses some whitespace (\"{expected.Substring(0, GetIndexOfFirstMatch(expected, actual, comparer)).DisplayWhitespace().TruncateWithEllipsis(maxStringLength)}\" at the beginning)";
			}

			if (indexOfFirstMismatch == minCommonLength && comparer.Equals(actual.TrimEnd(), expected))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix} which has unexpected whitespace (\"{actual.Substring(indexOfFirstMismatch).DisplayWhitespace().TruncateWithEllipsis(maxStringLength)}\" at the end)";
			}

			if (indexOfFirstMismatch == minCommonLength && comparer.Equals(actual, expected.TrimEnd()))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix} which misses some whitespace (\"{expected.Substring(indexOfFirstMismatch).DisplayWhitespace().TruncateWithEllipsis(maxStringLength)}\" at the end)";
			}

			if (actual.Length < expected.Length && indexOfFirstMismatch == actual.Length)
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix} with a length of {actual.Length} which is shorter than the expected length of {expected.Length} and misses:{Environment.NewLine}  \"{expected.Substring(actual.Length).TruncateWithEllipsis(maxStringLength)}\"";
			}

			if (actual.Length > expected.Length && indexOfFirstMismatch == expected.Length)
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix} with a length of {actual.Length} which is longer than the expected length of {expected.Length} and has superfluous:{Environment.NewLine}  \"{actual.Substring(expected.Length).TruncateWithEllipsis(maxStringLength)}\"";
			}

			return $"{prefix} which {stringDifference}";
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
			if (actual is null && expected is null)
			{
#if NET8_0_OR_GREATER
				return ValueTask.FromResult(true);
#else
				return Task.FromResult(true);
#endif
			}

			if (actual is null || expected is null)
			{
#if NET8_0_OR_GREATER
				return ValueTask.FromResult(false);
#else
				return Task.FromResult(false);
#endif
			}

			if (comparer is not null)
			{
#if NET8_0_OR_GREATER
				return ValueTask.FromResult(comparer.Equals(actual, expected));
#else
				return Task.FromResult(comparer.Equals(actual, expected));
#endif
			}
			
#if NET8_0_OR_GREATER
			return ValueTask.FromResult(string.Equals(actual, expected, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
#else
			return Task.FromResult(string.Equals(actual, expected, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
#endif
		}

		/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
			{
				(true, false) =>
					$"is equal to {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(false, false) =>
					$"equal to {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(true, true) =>
					$"is not equal to {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(false, true) =>
					$"not equal to {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
			};

		/// <inheritdoc cref="IStringMatchType.GetTypeString()" />
		public string GetTypeString()
			=> "";

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
