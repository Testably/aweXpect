using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Reflection.Tests.TestHelpers;

public static class StringEqualityResultExtensions
{
	/// <remarks>
	///     A custom match type that compares case-insensitively and explains a failure with a "Case folding" context, as
	///     an extension would list its options.
	/// </remarks>
	public static TResult AsCaseFoldedWithContext<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		result.Options.SetMatchType(new CaseFoldedMatchType(), nameof(AsCaseFoldedWithContext));
		return result;
	}

	private sealed class CaseFoldedMatchType : StringMatchType
	{
		public override MatchPhrase Phrase { get; } =
			MatchPhrase.Being("case-folded equal to").WithName(" as case-folded");

		public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
			=> new CaseFoldedMatcher();

		private sealed class CaseFoldedMatcher : StringMatcher
		{
			public override ValueTask<MatchResult> Match(string? actual, string? expected)
				=> new(MatchResult.From(string.Equals(actual?.ToUpperInvariant(), expected?.ToUpperInvariant(),
					StringComparison.Ordinal)));

			public override void AppendContexts(ResultContextCollector contexts)
				=> contexts.Add(new ResultContext.Fixed("Case folding", "invariant culture"));
		}
	}
}
