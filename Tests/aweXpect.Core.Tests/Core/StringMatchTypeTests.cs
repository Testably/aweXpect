using System.Collections.Generic;
using System.Threading;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Core;

public sealed class StringMatchTypeTests
{
	[Test]
	public async Task ForEvaluation_ShouldFreezeTheSettings()
	{
		RecordingMatchType matchType = new();
		StringEqualityOptions options = new("expected");
		options.SetMatchType(matchType, "AsTitle");
		options.IgnoringCase();

		await options.ForEvaluation(new aweXpect.Core.EvaluationContext.EvaluationContext(), CancellationToken.None)
			.AreConsideredEqual("let it be", "Let It Be");

		await That(matchType.Settings).HasCount(1);
		await That(matchType.Settings[0].IgnoreCase).IsTrue();
		await That(matchType.Settings[0].Comparer).IsNull();
	}

	[Test]
	public async Task ForEvaluation_ShouldReceiveTheContextAndTheCancellationOfTheEvaluation()
	{
		using CancellationTokenSource cts = new();
		IEvaluationContext context = new aweXpect.Core.EvaluationContext.EvaluationContext();
		RecordingMatchType matchType = new();
		StringEqualityOptions options = new("expected");
		options.SetMatchType(matchType, "AsTitle");

		await options.ForEvaluation(context, cts.Token).AreConsideredEqual("Let It Be", "Let It Be");

		await That(matchType.Environments).HasCount(1);
		await That(matchType.Environments[0].Context).IsSameAs(context);
		await That(matchType.Environments[0].CancellationToken).IsEqualTo(cts.Token);
	}

	[Test]
	public async Task ForEvaluation_WhenTheMatcherIsShared_ShouldReturnTheSameOptions()
	{
		StringEqualityOptions sut = new StringEqualityOptions("expected").AsWildcard();

		StringEqualityOptions first =
			sut.ForEvaluation(new aweXpect.Core.EvaluationContext.EvaluationContext(), CancellationToken.None);
		StringEqualityOptions second =
			sut.ForEvaluation(new aweXpect.Core.EvaluationContext.EvaluationContext(), CancellationToken.None);

		await That(first).IsSameAs(sut);
		await That(second).IsSameAs(sut)
			.Because("a match type without state per evaluation shares its matcher, so nothing is allocated");
	}

	[Test]
	public async Task WhenAnExpectationIsEvaluated_ShouldCreateOneMatcherForAllItems()
	{
		RecordingMatchType matchType = new();
		string[] subject = ["Let It Be", "Get Back", "Two of Us",];

		await That(subject).IsEqualTo(["Let It Be", "Get Back", "Two of Us",]).AsTitle(matchType);

		await That(matchType.Environments).HasCount(1)
			.Because("the items of one evaluation are compared with the matcher of that evaluation");
		await That(matchType.Matchers[0].Comparisons).IsEqualTo(3)
			.Because("the matcher keeps its state across the items");
	}

	[Test]
	public async Task WhenAnExpectationIsEvaluatedTwice_ShouldCreateAMatcherForEachEvaluation()
	{
		RecordingMatchType matchType = new();
		StringEqualityOptions options = new("expected");
		options.SetMatchType(matchType, "AsTitle");

		StringEqualityOptions first =
			options.ForEvaluation(new aweXpect.Core.EvaluationContext.EvaluationContext(), CancellationToken.None);
		StringEqualityOptions second =
			options.ForEvaluation(new aweXpect.Core.EvaluationContext.EvaluationContext(), CancellationToken.None);
		await first.AreConsideredEqual("Let It Be", "Let It Be");
		await second.AreConsideredEqual("Get Back", "Get Back");
		await second.AreConsideredEqual("Two of Us", "Two of Us");

		await That(second).IsNotSameAs(first);
		await That(matchType.Matchers).HasCount(2);
		await That(matchType.Matchers[0].Comparisons).IsEqualTo(1);
		await That(matchType.Matchers[1].Comparisons).IsEqualTo(2)
			.Because("each evaluation keeps the state of its own comparisons");
	}

	[Test]
	public async Task WhenNotComparable_ShouldFailBothWays()
	{
		string subject = "Let It Be";

		async Task Act()
			=> await That(subject).IsEqualTo("Get Back").AsTitle(new RecordingMatchType("had no title"));

		async Task ActNegated()
			=> await That(subject).IsNotEqualTo("Get Back").AsTitle(new RecordingMatchType("had no title"));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is titled "Get Back",
			             but it had no title

			             Actual:
			             Let It Be
			             """);
		await That(ActNegated).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not titled "Get Back",
			             but it had no title

			             Actual:
			             Let It Be
			             """)
			.Because("a subject that cannot be compared fails the negated expectation as well");
	}

	[Test]
	public async Task WhenTheMatcherExplainsAMismatch_ShouldUseTheExplanation()
	{
		string subject = "Let It Be";

		async Task Act()
			=> await That(subject).IsEqualTo("Get Back").AsTitle(new RecordingMatchType());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is titled "Get Back",
			             but it was titled "Let It Be" instead of "Get Back"
			             """);
	}

	private sealed class RecordingMatchType(string? notComparableReason = null) : StringMatchType
	{
		public List<MatchEnvironment> Environments { get; } = [];
		public List<CountingMatcher> Matchers { get; } = [];
		public List<StringMatchSettings> Settings { get; } = [];

		public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
		{
			CountingMatcher matcher = new(notComparableReason);
			Environments.Add(environment);
			Settings.Add(settings);
			Matchers.Add(matcher);
			return matcher;
		}

		public override MatchPhrase Phrase { get; } = MatchPhrase.Being("titled").WithName(" as title");

		public sealed class CountingMatcher(string? notComparableReason) : StringMatcher
		{
			public int Comparisons { get; private set; }

			public override ValueTask<MatchResult> Match(string? actual, string? expected)
			{
				Comparisons++;
				if (notComparableReason is not null)
				{
					return new ValueTask<MatchResult>(MatchResult.NotComparable(notComparableReason));
				}

				return new ValueTask<MatchResult>(actual == expected
					? MatchResult.Match
					: MatchResult.NoMatchBecause(new TitleExplanation(actual, expected)));
			}
		}

		private sealed class TitleExplanation(string? actual, string? expected) : MatchExplanation
		{
			public override string GetFailure(string it, ExpectationGrammars grammars, string? indentation)
				=> $"{it} was titled {Formatter.Format(actual)} instead of {Formatter.Format(expected)}";
		}
	}
}

internal static class StringMatchTypeTestExtensions
{
	public static TResult AsTitle<TResult>(this TResult result, StringMatchType matchType)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		result.Options.SetMatchType(matchType, nameof(AsTitle));
		return result;
	}
}
