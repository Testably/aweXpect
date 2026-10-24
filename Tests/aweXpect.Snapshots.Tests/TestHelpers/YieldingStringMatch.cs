using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Snapshots.Tests.TestHelpers;

internal static class YieldingStringMatch
{
	/// <remarks>
	///     A custom match type that compares exactly, but only after yielding, as an asynchronous extension would.
	/// </remarks>
	public static TResult AsYieldingMatch<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		result.Options.SetMatchType(new YieldingMatchType(), nameof(AsYieldingMatch));
		return result;
	}

	private sealed class YieldingMatchType : StringMatchType
	{
		private static readonly YieldingMatcher Matcher = new();

		public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
			=> Matcher;

		public override MatchPhrase Phrase { get; } = MatchPhrase.EqualTo.WithName(" after yielding");

		public override StringMatchSettingKinds SupportedSettings
			=> StringMatchSettingKinds.IgnoreCase | StringMatchSettingKinds.Comparer | StringMatchSettingKinds.WhiteSpace;

		private sealed class YieldingMatcher : StringMatcher
		{
			public override async ValueTask<MatchResult> Match(string? actual, string? expected)
			{
				await Task.Yield();
				return MatchResult.From(string.Equals(actual, expected, StringComparison.Ordinal));
			}

			public override string GetFailure(string it, ExpectationGrammars grammars, string? actual,
				string? expected, MatchResult result, StringDifferenceSettings? differenceSettings)
				=> $"{it} was \"{actual}\"";
		}
	}
}