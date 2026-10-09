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
		#region IMatchType Members

		/// <inheritdoc cref="IStringMatchType.InspectsSubject" />
		public bool InspectsSubject => false;

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
				$"{it} was {Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength))}";
			int minCommonLength = Math.Min(actual.Length, expected.Length);
			StringDifference stringDifference = new(actual, expected, comparer, settings);
			int indexOfFirstMismatch = stringDifference.IndexOfFirstMismatch(StringDifference.MatchType.Equality);
			if (indexOfFirstMismatch == 0 && comparer.Equals(actual.TrimStart(), expected))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix}, which has unexpected whitespace (\"{actual.Substring(0, actual.Length - actual.TrimStart().Length).TruncateWithEllipsis(maxStringLength).Escape()}\" at the beginning)";
			}

			if (indexOfFirstMismatch == 0 && comparer.Equals(actual, expected.TrimStart()))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix}, which misses some whitespace (\"{expected.Substring(0, expected.Length - expected.TrimStart().Length).TruncateWithEllipsis(maxStringLength).Escape()}\" at the beginning)";
			}

			if (indexOfFirstMismatch == minCommonLength && comparer.Equals(actual.TrimEnd(), expected))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix}, which has unexpected whitespace (\"{actual.Substring(indexOfFirstMismatch).TruncateWithEllipsis(maxStringLength).Escape()}\" at the end)";
			}

			if (indexOfFirstMismatch == minCommonLength && comparer.Equals(actual, expected.TrimEnd()))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix}, which misses some whitespace (\"{expected.Substring(indexOfFirstMismatch).TruncateWithEllipsis(maxStringLength).Escape()}\" at the end)";
			}

			if (actual.Length < expected.Length && indexOfFirstMismatch == actual.Length)
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix} with a length of {actual.Length}, which is shorter than the expected length of {expected.Length} and misses:{Environment.NewLine}  \"{expected.Substring(actual.Length).TruncateWithEllipsis(maxStringLength).Escape()}\"";
			}

			if (actual.Length > expected.Length && indexOfFirstMismatch == expected.Length)
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix} with a length of {actual.Length}, which is longer than the expected length of {expected.Length} and has superfluous:{Environment.NewLine}  \"{actual.Substring(expected.Length).TruncateWithEllipsis(maxStringLength).Escape()}\"";
			}

			return $"{prefix}, which {stringDifference}";
		}

		/// <inheritdoc cref="IStringMatchType.AreConsideredEqual(string?, string?, bool, IEqualityComparer{string})" />
		public ValueTask<StringMatchResult>
			AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string>? comparer)
		{
			if (actual is null && expected is null)
			{
				return new ValueTask<StringMatchResult>(true);
			}

			if (actual is null || expected is null)
			{
				return new ValueTask<StringMatchResult>(false);
			}

			if (comparer is not null)
			{
				return new ValueTask<StringMatchResult>(AreEqualByComparer(comparer, actual, expected));
			}

			return new ValueTask<StringMatchResult>(string.Equals(actual, expected, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
		}

		/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
			{
				(true, false) =>
					$"{grammars.Verb("is", "are")} equal to {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, false) =>
					$"equal to {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(true, true) =>
					$"{grammars.Verb("is not", "are not")} equal to {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, true) =>
					$"not equal to {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
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
