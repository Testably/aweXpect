using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	private static readonly IStringMatchType SuffixMatch = new SuffixMatchType();

	/// <summary>
	///     Interprets the expected <see langword="string" /> to be a suffix for the actual string.
	/// </summary>
	public StringEqualityOptions AsSuffix()
	{
		_matchType = SuffixMatch;
		return this;
	}

	private sealed class SuffixMatchType : IStringMatchType
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
			if (string.IsNullOrEmpty(actual) || expected == null)
			{
				return $"{it} was {Formatter.Format(actual)}";
			}

			string prefix =
				$"{it} was {Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}";
			int minCommonLength = Math.Min(actual.Length, expected.Length);
			StringDifference stringDifference = new(actual, expected, comparer,
				settings.WithMatchType(StringDifference.MatchType.Suffix));
			int indexOfFirstMismatch = stringDifference.IndexOfFirstMismatch(StringDifference.MatchType.Suffix);
			if (indexOfFirstMismatch == 0 && comparer.Equals(actual, expected.TrimStart()))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix} which misses some whitespace (\"{expected.Substring(0, GetIndexOfFirstMatch(expected, actual, comparer)).DisplayWhitespace().TruncateWithEllipsis(maxStringLength)}\" at the beginning)";
			}

			if (indexOfFirstMismatch == actual.Length)
			{
				string? trimmedActual = actual.TrimEnd();
				int commonLength = Math.Min(trimmedActual.Length, expected.Length);
				if (comparer.Equals(trimmedActual[^commonLength..], expected[^commonLength..]))
				{
					int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
					return
						$"{prefix} which has unexpected whitespace (\"{actual.Substring(trimmedActual.Length).DisplayWhitespace().TruncateWithEllipsis(maxStringLength)}\" at the end)";
				}
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

			return $"{prefix} which {stringDifference}";
		}

		/// <inheritdoc cref="IStringMatchType.AreConsideredEqual(string?, string?, bool, IEqualityComparer{string})" />
		public ValueTask<bool>
			AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string>? comparer)
		{
			if (actual is null && expected is null)
			{
				return new ValueTask<bool>(true);
			}

			if (actual is null || expected is null)
			{
				return new ValueTask<bool>(false);
			}

			if (comparer is not null)
			{
			return new ValueTask<bool>(actual.Length >= expected.Length &&
			                           UserCode.Invoke(() => comparer.Equals(actual[^expected.Length..], expected)));
			}

			return new ValueTask<bool>(actual.EndsWith(expected, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
		}

		/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
			{
				(true, false) =>
					$"{grammars.Verb("ends", "end")} with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(false, false) =>
					$"ending with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(true, true) =>
					$"{grammars.Verb("does not end", "do not end")} with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(false, true) =>
					$"not ending with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
			};

		/// <inheritdoc cref="IStringMatchType.GetTypeString()" />
		public string GetTypeString()
			=> " as suffix";

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
