using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	/// <summary>
	///     A built-in match type that compares and explains with the settings alone, so that its matchers keep no state
	///     and are shared unless a comparer is specified.
	/// </summary>
	private abstract class BuiltInMatchType : StringMatchType
	{
		private BuiltInMatcher? _ignoringCaseMatcher;
		private BuiltInMatcher? _ordinalMatcher;

		/// <summary>
		///     Compares the <paramref name="actual" /> with the <paramref name="expected" /> value.
		/// </summary>
		public abstract MatchResult Compare(string? actual, string? expected, bool ignoreCase,
			IEqualityComparer<string>? comparer);

		/// <summary>
		///     Get an extended failure text.
		/// </summary>
		public abstract string GetExtendedFailure(string it, ExpectationGrammars grammars, string? actual,
			string? expected,
			bool ignoreCase,
			IEqualityComparer<string> comparer,
			StringDifferenceSettings? settings);

		/// <inheritdoc cref="StringMatcher.CountOccurrences(string, string, int)" />
		public virtual int? CountOccurrences(string actual, string expected, StringMatchSettings settings,
			int maximum)
			=> null;

		/// <inheritdoc cref="StringMatcher.GetOccurrenceDetails(string, string, int)" />
		public virtual string GetOccurrenceDetails(string actual, string expected, StringMatchSettings settings,
			int count)
			=> "";

		/// <inheritdoc />
		public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
		{
			if (settings.Comparer is not null)
			{
				return new BuiltInMatcher(this, settings);
			}

			return settings.IgnoreCase
				? _ignoringCaseMatcher ??= new BuiltInMatcher(this, settings)
				: _ordinalMatcher ??= new BuiltInMatcher(this, settings);
		}

		private sealed class BuiltInMatcher(BuiltInMatchType matchType, StringMatchSettings settings)
			: StringMatcher
		{
			/// <inheritdoc />
			public override ValueTask<MatchResult> Match(string? actual, string? expected)
				=> new(matchType.Compare(actual, expected, settings.IgnoreCase, settings.Comparer));

			/// <inheritdoc />
			public override string GetFailure(string it, ExpectationGrammars grammars, string? actual,
				string? expected, MatchResult result, StringDifferenceSettings? differenceSettings)
				=> matchType.GetExtendedFailure(it, grammars, actual, expected, settings.IgnoreCase,
					settings.EffectiveComparer, differenceSettings);

			/// <inheritdoc />
			public override int? CountOccurrences(string actual, string expected, int maximum)
				=> matchType.CountOccurrences(actual, expected, settings, maximum);

			/// <inheritdoc />
			public override string GetOccurrenceDetails(string actual, string expected, int count)
				=> matchType.GetOccurrenceDetails(actual, expected, settings, count);
		}
	}
}
