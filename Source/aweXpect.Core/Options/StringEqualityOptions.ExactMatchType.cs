using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	private static readonly StringMatchType ExactMatch = new ExactMatchType();

	private sealed class ExactMatchType : BuiltInMatchType
	{
		private static readonly MatchPhrase SharedPhrase = MatchPhrase.EqualTo;

		#region IMatchType Members

		/// <inheritdoc />
		public override MatchPhrase Phrase => SharedPhrase;

		/// <inheritdoc />
		public override bool InspectsSubject => false;

		/// <inheritdoc />
		public override bool ListsLineDifferences => true;

		/// <inheritdoc />
		public override bool SuggestsIgnoringNewlineStyle => true;

		/// <inheritdoc />
		public override StringMatchSettingKinds SupportedSettings
			=> StringMatchSettingKinds.IgnoreCase | StringMatchSettingKinds.Comparer |
			   StringMatchSettingKinds.WhiteSpace;

		/// <inheritdoc />
		/// <remarks>
		///     Without a comparer, a window equals the <paramref name="expected" /> string exactly where an ordinal search
		///     finds it, so the windows are not copied.
		/// </remarks>
		public override int? CountOccurrences(string actual, string expected, StringMatchSettings settings,
			int maximum)
			=> settings.Comparer is null
				? CountOrdinalOccurrences(actual, expected,
					settings.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal, maximum)
				: null;

		/// <inheritdoc />
		public override string GetExtendedFailure(string it, ExpectationGrammars grammars, string? actual,
			string? expected,
			bool ignoreCase,
			IEqualityComparer<string> comparer,
			StringDifferenceSettings? settings)
		{
			if (actual == null || expected == null)
			{
				return $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual)}";
			}

			string prefix =
				$"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength))}";
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

		/// <inheritdoc />
		public override MatchResult
			Compare(string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string>? comparer)
		{
			if (actual is null && expected is null)
			{
				return MatchResult.From(true);
			}

			if (actual is null || expected is null)
			{
				return MatchResult.From(false);
			}

			if (comparer is not null)
			{
				return MatchResult.From(AreEqualByComparer(comparer, actual, expected));
			}

			return MatchResult.From(string.Equals(actual, expected, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
		}

		#endregion
	}
}
