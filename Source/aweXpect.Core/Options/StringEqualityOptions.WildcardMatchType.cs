using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	private static readonly IStringMatchType WildcardMatch = new WildcardMatchType();

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

	private sealed class WildcardMatchType : IStringMatchType
	{
		/// <summary>
		///     Counts the non-overlapping matches of the <paramref name="expected" /> wildcard pattern in the
		///     <paramref name="actual" /> value.
		/// </summary>
		public static int CountOccurrences(string actual, string expected, bool ignoreCase)
			=> RegexMatchType.CountOccurrences(FoldCase(actual, ignoreCase),
				new Regex(WildcardToUnanchoredRegularExpression(FoldCase(expected, ignoreCase)), RegexOptions.Singleline,
					RegexTimeout));

		/// <summary>
		///     Parses the <paramref name="expected" /> wildcard pattern into a regex that has to cover the whole value.
		/// </summary>
		public static Regex CreateRegex(string expected, bool ignoreCase)
			=> new(WildcardToRegularExpression(FoldCase(expected, ignoreCase)), RegexOptions.Singleline, RegexTimeout);

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

		/// <inheritdoc cref="IStringMatchType.InspectsSubject" />
		public bool InspectsSubject => true;

		/// <inheritdoc
		///     cref="IStringMatchType.GetExtendedFailure(string, string?, string?, bool, IEqualityComparer{string}, StringDifferenceSettings?)" />
		public string GetExtendedFailure(string it, string? actual, string? expected,
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

		/// <inheritdoc cref="IStringMatchType.AreConsideredEqual(string?, string?, bool, IEqualityComparer{string})" />
		public ValueTask<bool>
		AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
			IEqualityComparer<string>? comparer)
		{
			if (actual is null || expected is null)
			{
				return new ValueTask<bool>(false);
			}

			return new ValueTask<bool>(Regex.IsMatch(FoldCase(actual, ignoreCase),
				WildcardToRegularExpression(FoldCase(expected, ignoreCase)), RegexOptions.Singleline, RegexTimeout));
		}

		/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
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

		/// <inheritdoc cref="IStringMatchType.GetTypeString()" />
		public string GetTypeString()
			=> " as wildcard";

		/// <inheritdoc cref="IStringMatchType.GetOptionString(bool, IEqualityComparer{string})" />
		/// <remarks>
		///     A <paramref name="comparer" /> is rejected for a pattern, so only the casing can be ignored here.
		/// </remarks>
		public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
			=> ignoreCase ? " ignoring case" : "";

		#endregion
	}
}
