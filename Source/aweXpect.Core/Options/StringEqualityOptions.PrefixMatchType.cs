using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	private static readonly StringMatchType PrefixMatch = new PrefixMatchType();

	/// <summary>
	///     Interprets the expected <see langword="string" /> to be a prefix for the actual string.
	/// </summary>
	/// <exception cref="InvalidOperationException">A match type is already specified.</exception>
	public StringEqualityOptions AsPrefix()
	{
		SetMatchType(PrefixMatch, nameof(AsPrefix));
		return this;
	}

	private sealed class PrefixMatchType : BuiltInMatchType
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

		/// <inheritdoc />
		public override bool InspectsSubject => true;

		/// <inheritdoc />
		public override string GetExtendedFailure(string it, string? actual, string? expected,
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
				settings.WithMatchType(StringDifference.MatchType.Prefix));
			int indexOfFirstMismatch = stringDifference.IndexOfFirstMismatch(StringDifference.MatchType.Prefix);
			if (indexOfFirstMismatch == 0)
			{
				string? trimmedActual = actual.TrimStart();
				if (trimmedActual.Length >= expected.Length &&
				    comparer.Equals(trimmedActual[..expected.Length], expected))
				{
					int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
					return
						$"{prefix}, which has unexpected whitespace (\"{actual.Substring(0, GetIndexOfFirstMatch(actual, expected, comparer)).TruncateWithEllipsis(maxStringLength).Escape()}\" at the beginning)";
				}
			}

			if (indexOfFirstMismatch == 0 && comparer.Equals(actual, expected.TrimStart()))
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix}, which misses some whitespace (\"{expected.Substring(0, GetIndexOfFirstMatch(expected, actual, comparer)).TruncateWithEllipsis(maxStringLength).Escape()}\" at the beginning)";
			}

			if (actual.Length < expected.Length && indexOfFirstMismatch == actual.Length)
			{
				int maxStringLength = Customize.aweXpect.Formatting().MaximumStringLength.Get();
				return
					$"{prefix} with a length of {actual.Length}, which is shorter than the expected length of {expected.Length} and misses:{Environment.NewLine}  \"{expected.Substring(actual.Length).TruncateWithEllipsis(maxStringLength).Escape()}\"";
			}

			return $"{prefix}, which {stringDifference}";
		}

		/// <inheritdoc />
		public override MatchResult
			Compare(string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string>? comparer)
		{
			if (actual is null || expected is null)
			{
				return MatchResult.From(false);
			}

			if (comparer is not null)
			{
				return MatchResult.From(actual.Length >= expected.Length &&
				                                        AreEqualByComparer(comparer, actual[..expected.Length], expected));
			}

			return MatchResult.From(actual.StartsWith(expected, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
		}

		/// <inheritdoc />
		public override string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.IsActive(), grammars.IsNegated()) switch
			{
				(true, false) =>
					$"{grammars.Verb("starts", "start")} with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, false) =>
					$"starting with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(true, true) =>
					$"{grammars.Verb("does not start", "do not start")} with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, true) =>
					$"not starting with {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
			};

		/// <inheritdoc />
		public override string GetTypeString()
			=> " as prefix";

		/// <inheritdoc />
		public override string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
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
