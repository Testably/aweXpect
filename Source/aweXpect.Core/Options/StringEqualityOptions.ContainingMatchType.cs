using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	private static readonly StringMatchType ContainingMatch = new ContainingMatchType();

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a substring that must be contained in the actual string.
	/// </summary>
	/// <exception cref="InvalidOperationException">A match type is already specified.</exception>
	public StringEqualityOptions Containing()
	{
		SetMatchType(ContainingMatch, nameof(Containing));
		return this;
	}

	private sealed class ContainingMatchType : BuiltInMatchType
	{
		private static readonly MatchPhrase SharedPhrase =
			MatchPhrase.Verb("contains", "contain", "containing")
				.WithName(" containing");

		private static bool Contains(string actual, string expected, IEqualityComparer<string> comparer)
		{
			if (actual.Length < expected.Length)
			{
				return false;
			}

			for (int index = 0; index <= actual.Length - expected.Length; index++)
			{
				string candidate = actual.Substring(index, expected.Length);
				if (AreEqualByComparer(comparer, candidate, expected))
				{
					return true;
				}
			}

			return false;
		}

		#region IMatchType Members

		/// <inheritdoc />
		public override MatchPhrase Phrase => SharedPhrase;

		/// <inheritdoc />
		public override bool InspectsSubject => true;

		/// <inheritdoc />
		public override StringMatchAnchors Anchors => StringMatchAnchors.None;

		/// <inheritdoc />
		public override StringMatchSettingKinds SupportedSettings
			=> StringMatchSettingKinds.IgnoreCase | StringMatchSettingKinds.Comparer |
			   StringMatchSettingKinds.WhiteSpace;

		/// <inheritdoc />
		public override string GetExtendedFailure(string it, ExpectationGrammars grammars, string? actual,
			string? expected,
			bool ignoreCase,
			IEqualityComparer<string> comparer,
			StringDifferenceSettings? settings)
		{
			if (string.IsNullOrEmpty(actual) || expected == null)
			{
				return $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual)}";
			}

			string contains =
				$"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength))}";
			if (actual.Length < expected.Length)
			{
				contains +=
					$" with a length of {actual.Length}, which is shorter than the expected length of {expected.Length}";
			}

			return contains;
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
				return MatchResult.From(Contains(actual, expected, comparer));
			}

			return MatchResult.From(actual.Contains(expected, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
		}

		#endregion
	}
}
