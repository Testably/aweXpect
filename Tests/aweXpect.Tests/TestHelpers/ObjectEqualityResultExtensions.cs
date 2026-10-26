using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Tests;

public static class ObjectEqualityResultExtensions
{
	/// <remarks>
	///     A typed match type that compares the values by their <paramref name="key" /> and names the key of the expected
	///     value in its failure, as an extension would set it.
	/// </remarks>
	public static TSelf ByKey<TSelf, TElement>(this IObjectEqualityResult<TSelf, TElement> result,
		Func<TElement?, int> key)
		where TSelf : IObjectEqualityResult<TSelf, TElement>
	{
		result.Options.SetMatchType(new KeyMatchType<TElement>(key));
		return (TSelf)result;
	}

	/// <remarks>
	///     A match type that compares by <see cref="object.Equals(object)" /> and rejects the <paramref name="rejected" />
	///     value as an expected value.
	/// </remarks>
	public static TSelf Rejecting<TSelf, TElement>(this IObjectEqualityResult<TSelf, TElement> result,
		TElement rejected)
		where TSelf : IObjectEqualityResult<TSelf, TElement>
	{
		result.Options.SetMatchType(new RejectingMatchType<TElement>(rejected));
		return (TSelf)result;
	}

	private sealed class RejectingMatchType<TSubject>(TSubject rejected) : ObjectMatchType<TSubject>(nameof(Rejecting))
	{
		public override MatchPhrase Phrase => Default.Phrase;

		public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment)
			=> Default.ForEvaluation(environment);

		public override void ValidateExpected(TSubject expected, string parameterName)
		{
			if (Equals(expected, rejected))
			{
				throw new ArgumentException($"The '{parameterName}' item {expected} is rejected.", parameterName);
			}
		}
	}

	private sealed class KeyMatchType<TSubject>(Func<TSubject?, int> key) : ObjectMatchType<TSubject>(nameof(ByKey))
	{
		public override MatchPhrase Phrase { get; } = MatchPhrase.Being("keyed like");

		public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment)
			=> new Matcher(key);

		private sealed class Matcher(Func<TSubject?, int> key) : ObjectMatcher<TSubject, TSubject>
		{
			public override ValueTask<MatchResult> Match(TSubject actual, TSubject expected)
				=> new(MatchResult.From(key(actual) == key(expected)));

			public override string GetFailure(string it, ExpectationGrammars grammars, TSubject? actual,
				TSubject expected, MatchResult result, string? indentation)
				=> $"{it} had the key {key(actual)} of the value with the key {key(expected)}";
		}
	}
}
