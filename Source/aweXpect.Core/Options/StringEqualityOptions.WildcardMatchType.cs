using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	private static readonly StringMatchType WildcardMatch = new WildcardMatchType();

	/// <summary>
	///     Interprets the expected <see langword="string" /> as wildcard pattern.<br />
	///     Supports * to match zero or more characters and ? to match exactly one character.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">
	///     A custom comparer is already set, which the regex engine cannot honour, or a match type is already specified.
	/// </exception>
	public StringEqualityOptions AsWildcard()
	{
		if (_comparer is not null)
		{
			throw ComparerAndPatternConflict();
		}

		SetMatchType(WildcardMatch, nameof(AsWildcard));
		return this;
	}

	private sealed class WildcardMatchType : BuiltInMatchType
	{
		/// <summary>
		///     Counts the non-overlapping matches of the <paramref name="expected" /> wildcard pattern in the
		///     <paramref name="actual" /> value, up to the <paramref name="maximum" />.
		/// </summary>
		public static int CountOccurrences(string actual, string expected, bool ignoreCase, int maximum)
			=> RegexMatchType.CountOccurrences(FoldCase(actual, ignoreCase),
				GetOrParseRegex(WildcardToUnanchoredRegularExpression(FoldCase(expected, ignoreCase)),
					RegexOptions.Singleline), maximum);

		/// <summary>
		///     Parses the <paramref name="expected" /> wildcard pattern into a regex that has to cover the whole value.
		/// </summary>
		public static Regex CreateRegex(string expected, bool ignoreCase)
			=> GetOrParseRegex(WildcardToRegularExpression(FoldCase(expected, ignoreCase)), RegexOptions.Singleline);

		/// <summary>
		///     Whether the <paramref name="actual" /> value matches the <paramref name="regex" /> from
		///     <see cref="CreateRegex" />.
		/// </summary>
		public static bool IsMatch(Regex regex, string actual, bool ignoreCase)
			=> regex.IsMatch(FoldCase(actual, ignoreCase));

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
		///     The resulting pattern requires <see cref="RegexOptions.Singleline" />, so that both wildcards treat a newline
		///     like any other character.<br />
		///     A <c>?</c> matches a surrogate pair as one character, and the atomic group keeps it from matching only its
		///     first half when the rest of the pattern would not match otherwise.
		/// </remarks>
		private static string WildcardToUnanchoredRegularExpression(string value)
			=> Regex.Escape(value)
				.Replace("\\?", @"(?>[\uD800-\uDBFF][\uDC00-\uDFFF]|.)")
				.Replace("\\*", ".*");

		#region IStringMatchType Members

		/// <inheritdoc />
		public override bool InspectsSubject => true;

		/// <inheritdoc />
		public override string GetExtendedFailure(string it, string? actual, string? expected,
			bool ignoreCase,
			IEqualityComparer<string> comparer,
			StringDifferenceSettings? settings)
		{
			if (expected is null)
			{
				return $"could not compare the <null> wildcard pattern with {Formatter.Format(actual)}";
			}

			StringDifference stringDifference = new(actual, expected, comparer,
				settings.WithMatchType(StringDifference.MatchType.Wildcard));
			return $"{it} did not match{stringDifference.ToString("")}";
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

			return MatchResult.From(IsMatch(CreateRegex(expected, ignoreCase), actual, ignoreCase));
		}

		/// <inheritdoc />
		public override string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.IsActive(), grammars.IsNegated()) switch
			{
				(true, false) =>
					$"{grammars.Verb("matches", "match")} {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, false) =>
					$"matching {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(true, true) =>
					$"{grammars.Verb("does not match", "do not match")} {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, true) =>
					$"not matching {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
			};

		/// <inheritdoc />
		public override string GetTypeString()
			=> " as wildcard";

		/// <inheritdoc />
		/// <remarks>
		///     A <paramref name="comparer" /> is rejected for a pattern, so only the casing can be ignored here.
		/// </remarks>
		public override string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
			=> ignoreCase ? " ignoring case" : "";

		#endregion
	}
}
