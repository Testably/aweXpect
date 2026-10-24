namespace aweXpect.Core.Tests.Core;

public sealed class MatchResultTests
{
	[Test]
	public async Task Default_ShouldBeAMismatchWithoutAnExplanation()
	{
		MatchResult result = default;

		await That(result.Outcome).IsEqualTo(MatchOutcome.NoMatch);
		await That(result.IsMatch).IsFalse()
			.Because("an uninitialized result must never pass");
		await That(result.NotComparableReason).IsNull();
		await That(result.Explanation).IsNull();
	}

	[Test]
	[Arguments(false, MatchOutcome.NoMatch)]
	[Arguments(true, MatchOutcome.Match)]
	public async Task From_ShouldKeepTheOutcome(bool isMatch, MatchOutcome expectedOutcome)
	{
		MatchResult result = MatchResult.From(isMatch);

		await That(result.Outcome).IsEqualTo(expectedOutcome);
		await That(result.IsMatch).IsEqualTo(isMatch);
		await That(result.Explanation).IsNull();
	}

	[Test]
	public async Task Match_ShouldBeAMatch()
	{
		MatchResult result = MatchResult.Match;

		await That(result.Outcome).IsEqualTo(MatchOutcome.Match);
		await That(result.IsMatch).IsTrue();
		await That(result.NotComparableReason).IsNull();
		await That(result.Explanation).IsNull();
		await That(result.Cause).IsNull();
	}

	[Test]
	public async Task NoMatch_ShouldBeTheDefault()
	{
		MatchResult result = MatchResult.NoMatch;

		await That(result).IsEqualTo(default(MatchResult));
	}

	[Test]
	public async Task NoMatchBecause_ShouldKeepTheExplanation()
	{
		FixedExplanation explanation = new("it differed");

		MatchResult result = MatchResult.NoMatchBecause(explanation);

		await That(result.Outcome).IsEqualTo(MatchOutcome.NoMatch);
		await That(result.IsMatch).IsFalse();
		await That(result.Explanation).IsSameAs(explanation);
		await That(result.Cause).IsNull()
			.Because("an explanation is no cause of a subject that could not be compared");
	}

	[Test]
	public async Task NoMatchBecause_WhenExplanationIsNull_ShouldThrowArgumentNullException()
	{
		void Act() => MatchResult.NoMatchBecause(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithMessage("The 'explanation' cannot be null.").AsPrefix().And
			.WithParamName("explanation");
	}

	[Test]
	public async Task NotComparable_ShouldKeepTheReasonAndTheCause()
	{
		FormatException cause = new("no number");

		MatchResult result = MatchResult.NotComparable("was no number", cause);

		await That(result.Outcome).IsEqualTo(MatchOutcome.NotComparable);
		await That(result.IsMatch).IsFalse();
		await That(result.NotComparableReason).IsEqualTo("was no number");
		await That(result.Cause).IsSameAs(cause);
		await That(result.Explanation).IsNull();
	}

	[Test]
	public async Task NotComparable_WhenReasonIsNull_ShouldThrowArgumentNullException()
	{
		void Act() => MatchResult.NotComparable(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithMessage("The 'reason' cannot be null.").AsPrefix().And
			.WithParamName("reason")
			.Because("without a reason the result would silently count as a plain mismatch");
	}

	private sealed class FixedExplanation(string failure) : MatchExplanation
	{
		public override string GetFailure(string it, ExpectationGrammars grammars, string? indentation)
			=> failure;
	}
}
