using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	/// <remarks>
	///     The match type is only created when it is first used, so that a comparison with another match type does not
	///     pay for it on its first call.
	/// </remarks>
	private static StringMatchType WildcardMatch => WildcardMatchType.Instance;

	/// <summary>
	///     Interprets the expected <see langword="string" /> as wildcard pattern.<br />
	///     Supports * to match zero or more characters and ? to match exactly one character.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">
	///     A custom comparer is already set, which the regex engine cannot honour, or another match type is already specified.
	/// </exception>
	public StringEqualityOptions AsWildcard()
	{
		return WithMatchType(WildcardMatch, nameof(AsWildcard), WildcardMatchTypeIndex);
	}

	private sealed class WildcardMatchType : StringMatchType
	{
		private static readonly MatchPhrase SharedPhrase =
			MatchPhrase.Verb("matches", "match", "matching")
				.WithName(" as wildcard")
				.WithPattern("wildcard", "wildcard pattern");

		public static readonly WildcardMatchType Instance = new();

		/// <inheritdoc />
		public override bool InspectsSubject => true;

		/// <inheritdoc />
		public override MatchPhrase Phrase => SharedPhrase;

		/// <inheritdoc />
		/// <remarks>
		///     A pattern is matched by the regex engine, which has no way to consult a comparer.
		/// </remarks>
		public override StringMatchSettingKinds SupportedSettings
			=> StringMatchSettingKinds.IgnoreCase | StringMatchSettingKinds.WhiteSpace;

		/// <inheritdoc />
		public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
			=> new WildcardMatcher(settings.IgnoreCase);

		/// <summary>
		///     Matches the patterns of one evaluation, in which a collection compares every item with the same pattern, so
		///     it keeps the regex that was parsed last.
		/// </summary>
		private sealed class WildcardMatcher(bool ignoreCase) : StringMatcher
		{
			/// <remarks>
			///     The pattern and its regex are replaced together, as the matcher of shared options is used by several
			///     threads.
			/// </remarks>
			private ParsedPattern? _parsedPattern;

			/// <inheritdoc />
			public override ValueTask<MatchResult> Match(string? actual, string? expected)
			{
				if (actual is null || expected is null)
				{
					return new ValueTask<MatchResult>(MatchResult.NoMatch);
				}

				return new ValueTask<MatchResult>(
					MatchResult.From(GetRegex(expected).IsMatch(FoldCase(actual, ignoreCase))));
			}

			/// <inheritdoc />
			public override string GetFailure(string it, ExpectationGrammars grammars, string? actual,
				string? expected, MatchResult result, StringDifferenceSettings? differenceSettings)
			{
				if (expected is null)
				{
					return $"could not compare the <null> wildcard pattern with {Formatter.Format(actual)}";
				}

				StringDifference stringDifference = new(actual, expected,
					ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal,
					differenceSettings.WithMatchType(StringDifference.MatchType.Wildcard));
				return $"{it} did not match{stringDifference.ToString("")}";
			}

			/// <inheritdoc />
			/// <remarks>
			///     A pattern can match a different number of characters than it is long, so its occurrences are counted
			///     by the regex engine.
			/// </remarks>
			public override int? CountOccurrences(string actual, string expected, int maximum)
				=> RegexMatchType.CountMatches(FoldCase(actual, ignoreCase),
					GetOrParseRegex(WildcardToUnanchoredRegularExpression(FoldCase(expected, ignoreCase)),
						RegexOptions.Singleline), maximum);

			private Regex GetRegex(string expected)
			{
				ParsedPattern? parsedPattern = _parsedPattern;
				if (parsedPattern is null || !string.Equals(parsedPattern.Pattern, expected, StringComparison.Ordinal) ||
				    parsedPattern.Regex.MatchTimeout != GetRegexTimeout())
				{
					parsedPattern = new ParsedPattern(expected, CreateRegex(expected, ignoreCase));
					_parsedPattern = parsedPattern;
				}

				return parsedPattern.Regex;
			}

			private sealed class ParsedPattern(string pattern, Regex regex)
			{
				public string Pattern { get; } = pattern;

				public Regex Regex { get; } = regex;
			}

			/// <summary>
			///     Parses the <paramref name="expected" /> wildcard pattern into a regex that has to cover the whole value.
			/// </summary>
			private static Regex CreateRegex(string expected, bool ignoreCase)
				=> GetOrParseRegex(WildcardToRegularExpression(FoldCase(expected, ignoreCase)), RegexOptions.Singleline);

			/// <remarks>
			///     The casing is ignored by comparing the upper-case invariant values instead of with
			///     <see cref="RegexOptions.IgnoreCase" />, so that the same characters are considered equal as by
			///     <see cref="System.StringComparison.OrdinalIgnoreCase" /> in the plain comparison.
			/// </remarks>
			private static string FoldCase(string value, bool ignoreCase)
				=> ignoreCase ? value.ToUpperInvariant() : value;

			/// <remarks>
			///     The pattern is anchored with <c>\A</c> and <c>\z</c>, so that it has to cover the complete value:
			///     <c>^</c> and <c>$</c> would bind to a line boundary, and <c>$</c> would also allow a trailing newline.
			/// </remarks>
			private static string WildcardToRegularExpression(string value)
				=> $@"\A{WildcardToUnanchoredRegularExpression(value)}\z";

			/// <remarks>
			///     The resulting pattern requires <see cref="RegexOptions.Singleline" />, so that both wildcards treat a
			///     newline like any other character.<br />
			///     A <c>?</c> matches a surrogate pair as one character, and the atomic group keeps it from matching only
			///     its first half when the rest of the pattern would not match otherwise.
			/// </remarks>
			private static string WildcardToUnanchoredRegularExpression(string value)
				=> Regex.Escape(value)
					.Replace("\\?", @"(?>[\uD800-\uDBFF][\uDC00-\uDFFF]|.)")
					.Replace("\\*", ".*");
		}
	}
}