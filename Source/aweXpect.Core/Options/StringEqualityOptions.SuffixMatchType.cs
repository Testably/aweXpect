using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	private static readonly StringMatchType SuffixMatch = new SuffixMatchType();

	/// <summary>
	///     Interprets the expected <see langword="string" /> to be a suffix for the actual string.
	/// </summary>
	/// <exception cref="InvalidOperationException">A match type is already specified.</exception>
	public StringEqualityOptions AsSuffix()
	{
		SetMatchType(SuffixMatch, nameof(AsSuffix));
		return this;
	}

	private sealed class SuffixMatchType : BuiltInMatchType
	{
		private static readonly MatchPhrase SharedPhrase =
			MatchPhrase.Verb("ends with", "end with", "ending with")
				.WithName(" as suffix")
				.WithPattern("suffix", "suffix");

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
		public override MatchPhrase Phrase => SharedPhrase;

		/// <inheritdoc />
		public override bool InspectsSubject => true;

		/// <inheritdoc />
		public override StringMatchAnchors Anchors => StringMatchAnchors.End;

		/// <inheritdoc />
		public override StringMatchSettingKinds SupportedSettings
			=> StringMatchSettingKinds.IgnoreCase | StringMatchSettingKinds.Comparer |
			   StringMatchSettingKinds.WhiteSpace;

		/// <inheritdoc />
		public override bool SuggestsIgnoringNewlineStyle => true;

		/// <inheritdoc />
		/// <remarks>
		///     An empty suffix matches every value, so that the expectation would say nothing about the subject.
		/// </remarks>
		public override void ValidateExpected(string? expected, string parameterName)
		{
			if (expected?.Length == 0)
			{
				throw CreateEmptyPatternException(parameterName, "suffix");
			}
		}

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
				                                        AreEqualByComparer(comparer, actual[^expected.Length..], expected));
			}

			return MatchResult.From(actual.EndsWith(expected, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
		}

		#endregion
	}
}
