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
	/// <exception cref="InvalidOperationException">Another match type is already specified.</exception>
	public StringEqualityOptions AsPrefix()
	{
		return WithMatchType(PrefixMatch, nameof(AsPrefix), PrefixMatchTypeIndex);
	}

	private sealed class PrefixMatchType : BuiltInMatchType
	{
		private static readonly MatchPhrase SharedPhrase =
			MatchPhrase.Verb("starts with", "start with", "starting with")
				.WithName(" as prefix")
				.WithPattern("prefix", "prefix");

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
		public override StringMatchAnchors Anchors => StringMatchAnchors.Start;

		/// <inheritdoc />
		public override StringMatchSettingKinds SupportedSettings
			=> StringMatchSettingKinds.IgnoreCase | StringMatchSettingKinds.Comparer |
			   StringMatchSettingKinds.WhiteSpace;

		/// <inheritdoc />
		public override bool SuggestsIgnoringNewlineStyle => true;

		/// <inheritdoc />
		/// <remarks>
		///     An empty prefix matches every value, so that the expectation would say nothing about the subject.
		/// </remarks>
		public override void ValidateExpected(string? expected, string parameterName)
		{
			if (expected?.Length == 0)
			{
				throw CreateEmptyPatternException(parameterName, "prefix");
			}
		}

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

			string prefix =
				$"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength))}";
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

		#endregion
	}
}
