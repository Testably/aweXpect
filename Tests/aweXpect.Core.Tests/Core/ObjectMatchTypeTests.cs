using System.Collections.Generic;
using System.Threading;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core;

public sealed class ObjectMatchTypeTests
{
	[Test]
	public async Task ForEvaluation_ShouldReceiveTheContextAndTheCancellationOfTheEvaluation()
	{
		using CancellationTokenSource cts = new();
		IEvaluationContext context = new aweXpect.Core.EvaluationContext.EvaluationContext();
		RecordingMatchType<Track> matchType = new();
		ObjectEqualityOptions<Track> options = new();
		options.SetMatchType(matchType, "ByTitle");

		await options.ForEvaluation(context, cts.Token).AreConsideredEqual(new Track("Let It Be"),
			new Track("Let It Be"));

		await That(matchType.Environments).HasCount(1);
		await That(matchType.Environments[0].Context).IsSameAs(context);
		await That(matchType.Environments[0].CancellationToken).IsEqualTo(cts.Token);
	}

	[Test]
	public async Task WhenAnExpectationIsEvaluated_ShouldCreateOneMatcherForAllItems()
	{
		RecordingMatchType<Track> matchType = new();
		Track[] subject = [new("Let It Be"), new("Get Back"), new("Two of Us"),];

		await That(subject).IsEqualTo([new Track("Let It Be"), new Track("Get Back"), new Track("Two of Us"),])
			.ByTitle(matchType);

		await That(matchType.Environments).HasCount(1)
			.Because("the items of one evaluation are compared with the matcher of that evaluation");
		await That(matchType.Matchers[0].Comparisons).IsEqualTo(3)
			.Because("the matcher keeps its state across the items");
	}

	[Test]
	public async Task WhenNotComparable_ShouldFailBothWays()
	{
		Track subject = new("Let It Be");

		async Task Act()
			=> await That(subject).IsEqualTo(new Track("Get Back")).ByTitle(new NotComparableMatchType<object?>());

		async Task ActNegated()
			=> await That(subject).IsNotEqualTo(new Track("Get Back")).ByTitle(new NotComparableMatchType<object?>());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is titled like ObjectMatchTypeTests.Track {
			                 Title = "Get Back"
			               },
			             but it had no title to compare
			             """);
		await That(ActNegated).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not titled like ObjectMatchTypeTests.Track {
			                 Title = "Get Back"
			               },
			             but it had no title to compare
			             """)
			.Because("a subject that cannot be compared fails the negated expectation as well");
	}

	[Test]
	public async Task WhenTheExpectedValueHasAnotherType_ShouldNotMatchWithoutAskingATypedMatcher()
	{
		RecordingMatchType<Track> matchType = new();
		ObjectEqualityOptions<Track> options = new();
		options.SetMatchType(matchType, "ByTitle");

		bool result = await options.ForEvaluation(new aweXpect.Core.EvaluationContext.EvaluationContext(),
			CancellationToken.None).AreConsideredEqual(new Track("Let It Be"), "Let It Be");

		await That(result).IsFalse();
		await That(matchType.Matchers[0].Comparisons).IsEqualTo(0)
			.Because("a typed matcher only compares expected values of its type");
	}

	[Test]
	public async Task WhenTheMatcherExplainsAMismatch_ShouldUseTheTypedValues()
	{
		Track subject = new("Let It Be");

		async Task Act()
			=> await That(subject).IsEqualTo(new Track("Get Back")).ByTitle(new RecordingMatchType<object?>());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is titled like ObjectMatchTypeTests.Track {
			                 Title = "Get Back"
			               },
			             but it was titled "Let It Be" instead of "Get Back"
			             """);
	}

	public sealed class Track(string title)
	{
		public string Title { get; } = title;
	}

	private sealed class NotComparableMatchType<TSubject> : ObjectMatchType<TSubject>
	{
		public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment)
			=> new Matcher();

		public override MatchPhrase Phrase { get; } = MatchPhrase.Being("titled like");

		private sealed class Matcher : ObjectMatcher<TSubject>
		{
			public override ValueTask<MatchResult> Match<TExpected>(TSubject actual, TExpected expected)
				=> new(MatchResult.NotComparable("had no title to compare"));
		}
	}

	private sealed class RecordingMatchType<TSubject> : ObjectMatchType<TSubject>
	{
		public List<MatchEnvironment> Environments { get; } = [];
		public List<CountingMatcher> Matchers { get; } = [];

		public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment)
		{
			CountingMatcher matcher = new();
			Environments.Add(environment);
			Matchers.Add(matcher);
			return matcher;
		}

		public override MatchPhrase Phrase { get; } = MatchPhrase.Being("titled like");

		public sealed class CountingMatcher : ObjectMatcher<TSubject, Track>
		{
			public int Comparisons { get; private set; }

			public override ValueTask<MatchResult> Match(TSubject actual, Track expected)
			{
				Comparisons++;
				return new ValueTask<MatchResult>(MatchResult.From(TitleOf(actual) == expected.Title));
			}

			public override string GetFailure(string it, ExpectationGrammars grammars, TSubject? actual,
				Track expected, MatchResult result, string? indentation)
				=> $"{it} was titled {Formatter.Format(TitleOf(actual))} instead of {Formatter.Format(expected.Title)}";

			private static string? TitleOf(TSubject? subject)
				=> subject is Track track ? track.Title : null;
		}
	}
}

internal static class ObjectMatchTypeTestExtensions
{
	public static TSelf ByTitle<TSelf, TElement>(this IObjectEqualityResult<TSelf, TElement> result,
		ObjectMatchType<TElement> matchType)
		where TSelf : IObjectEqualityResult<TSelf, TElement>
	{
		result.Options.SetMatchType(matchType, nameof(ByTitle));
		return (TSelf)result;
	}
}