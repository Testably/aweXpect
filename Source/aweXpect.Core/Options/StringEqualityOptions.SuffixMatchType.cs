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
	/// <exception cref="InvalidOperationException">A match type is already specified.</exception>
	public StringEqualityOptions AsSuffix()
	{
		SetMatchType(SuffixMatch, nameof(AsSuffix));
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

		/// <inheritdoc cref="IStringMatchType.InspectsSubject" />
		public bool InspectsSubject => true;

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
				$"{it} was {Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength))}";
			StringDifference stringDifference = new(actual, expected, comparer,
				settings.WithMatchType(StringDifference.MatchType.Suffix));
			int indexOfFirstMismatch = stringDifference.IndexOfFirstMismatch(StringDifference.MatchType.Suffix);
			if (comparer.Equals(actual, expected.TrimStart()))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix}, which misses some whitespace (\"{expected.Substring(0, GetIndexOfFirstMatch(expected, actual, comparer)).TruncateWithEllipsis(maxStringLength).Escape()}\" at the beginning)";
			}

			string? trimmedActual = actual.TrimEnd();
			if (trimmedActual.Length >= expected.Length &&
			    comparer.Equals(trimmedActual[^expected.Length..], expected))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix}, which has unexpected whitespace (\"{actual.Substring(trimmedActual.Length).TruncateWithEllipsis(maxStringLength).Escape()}\" at the end)";
			}

			string? trimmedExpected = expected.TrimEnd();
			if (comparer.Equals(actual, trimmedExpected))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix}, which misses some whitespace (\"{expected.Substring(trimmedExpected.Length).TruncateWithEllipsis(maxStringLength).Escape()}\" at the end)";
			}

			if (actual.Length < expected.Length && indexOfFirstMismatch < 0)
			{
				return $"{prefix} with a length of {actual.Length}, which {stringDifference}";
			}

			return $"{prefix}, which {stringDifference}";
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
				                           AreEqualByComparer(comparer, actual[^expected.Length..], expected));
			}

			return new ValueTask<bool>(actual.EndsWith(expected, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
		}

		/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
			{
				(true, false) =>
					$"{grammars.Verb("ends", "end")} with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, false) =>
					$"ending with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(true, true) =>
					$"{grammars.Verb("does not end", "do not end")} with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, true) =>
					$"not ending with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
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
