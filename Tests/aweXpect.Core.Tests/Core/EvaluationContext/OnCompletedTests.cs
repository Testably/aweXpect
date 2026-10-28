using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Equivalency;
using Context = aweXpect.Core.EvaluationContext.EvaluationContext;

namespace aweXpect.Core.Tests.Core.EvaluationContext;

public sealed class OnCompletedTests
{
	[Test]
	public async Task InACombination_ShouldRunWithTheOutcomeOfTheCombination()
	{
		SideEffectLog log = new();

		await ThatAny(
			That(1).HasSideEffect(log, "a", Outcome.Failure),
			That(2).HasSideEffect(log, "b"));

		await That(log.Effects).IsEqualTo([
			"a does nothing (whole: Success, own: Failure, negated: False)",
			"b deletes a leftover pending snapshot (whole: Success, own: Success, negated: False)",
		]).Because("the combination decides the whole expectation");
	}

	[Test]
	public async Task InAMemberOfWhose_ShouldRunWithTheOutcomeOfTheWholeExpectation()
	{
		SideEffectLog log = new();

		async Task Act()
			=> await That("foo").HasSideEffect(log, "a", Outcome.Failure)
				.Whose(s => s.Length, length => length.HasSideEffect(log, "b"));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that "foo"
			             has side effect a whose Length has side effect b,
			             but it did not
			             """);
		await That(log.Effects).IsEqualTo([
			"a writes the pending snapshot (whole: Failure, own: Failure, negated: False)",
			"b does nothing (whole: Failure, own: Success, negated: False)",
		]);
	}

	[Test]
	public async Task InANestedExpectationOfAnExpectedObject_ShouldRunAtTheEndOfTheWholeExpectation()
	{
		SideEffectLog log = new();
		var subject = new
		{
			Value = 1,
		};
		var expected = new
		{
			Value = It.Is<int>().That.HasSideEffect(log, "a", Outcome.Failure),
		};
		int effectsAfterTheComparison = -1;

		bool CountEffects(object _)
		{
			effectsAfterTheComparison = log.Effects.Count;
			return true;
		}

		async Task Act()
			=> await That(subject).IsEquivalentTo(expected).And.Satisfies(CountEffects);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equivalent to expected and satisfies CountEffects,
			             but it was not:
			               Property Value differed:
			                   Actual: 1
			                 Expected: is int that has side effect a

			             Equivalency options:
			              - include public fields and properties
			             """);
		await That(effectsAfterTheComparison).IsEqualTo(0)
			.Because("the nested expectation does not decide the whole expectation");
		await That(log.Effects)
			.IsEqualTo(["a writes the pending snapshot (whole: Failure, own: Failure, negated: False)"]);
	}

	[Test]
	public async Task ManualExpectationBuilder_WithoutAnEvaluation_ShouldRunWithTheOutcomeOfItsExpectations()
	{
		SideEffectLog log = new();
		using CancellationTokenSource cts = new();
		ManualExpectationBuilder<int> builder = new();
		_ = new ThatSubject<int>(builder).HasSideEffect(log, "a", Outcome.Failure);

		ConstraintResult result = await builder.IsMetBy(1, cts.Token);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(log.Tokens).IsEqualTo([cts.Token]);
		await That(log.Effects)
			.IsEqualTo(["a writes the pending snapshot (whole: Failure, own: Failure, negated: False)"])
			.Because("there is no surrounding evaluation that could decide another outcome");
	}

	[Test]
	public async Task WhenAnotherBranchOfOrIsMet_ShouldRunWithTheOutcomeOfTheWholeExpectation()
	{
		SideEffectLog log = new();

		await That(1).HasSideEffect(log, "a", Outcome.Failure).Or.IsEqualTo(1);

		await That(log.Effects).IsEqualTo(["a does nothing (whole: Success, own: Failure, negated: False)"]);
	}

	[Test]
	public async Task WhenCanceled_ShouldRunWithAnUndecidedOutcome()
	{
		SideEffectLog log = new();
		using CancellationTokenSource cts = new();

		async Task Act()
			=> await That(1).HasSideEffect(log, "a", Outcome.Failure, cancellationToken =>
			{
				cts.Cancel();
				cancellationToken.ThrowIfCancellationRequested();
				return Task.CompletedTask;
			}).WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected that 1
			             has side effect a,
			             but it could not be verified, because the evaluation was already canceled
			             """);
		await That(log.Tokens).IsEqualTo([cts.Token]);
		await That(log.Effects).IsEqualTo(["a does nothing (whole: Undecided, own: Failure, negated: False)"]);
	}

	[Test]
	public async Task WhenCheckedRepeatedly_ShouldRunOnceAfterTheLastCheck()
	{
		SideEffectLog log = new();
		int value = 0;

		await That(1).CompliesWith(it
				=> it.HasSideEffect(log, "a", _ => ++value >= 3 ? Outcome.Success : Outcome.Failure))
			.Within(1.Seconds()).CheckEvery(10.Milliseconds()).WithTimeProvider(new VirtualTimeProvider());

		await That(log.Evaluations).IsEqualTo(3);
		await That(log.Effects).IsEqualTo([
			"a deletes a leftover pending snapshot (whole: Success, own: Success, negated: False)",
		]).Because("each check registers the same callback again");
	}

	[Test]
	public async Task WhenEvaluatedEventually_ShouldRunOnceAfterTheLastAttempt()
	{
		SideEffectLog log = new();
		int value = 0;

		await That(() => ++value).Eventually().OnVirtualTime().Within(1.Seconds()).CheckEvery(10.Milliseconds())
			.HasSideEffect(log, "a", v => v >= 3 ? Outcome.Success : Outcome.Failure);

		await That(log.Evaluations).IsEqualTo(3);
		await That(log.Effects).IsEqualTo([
			"a deletes a leftover pending snapshot (whole: Success, own: Success, negated: False)",
		]).Because("each attempt registers the same callback again");
	}

	[Test]
	public async Task WhenEvaluatedForEachItem_ShouldRunOnce()
	{
		SideEffectLog log = new();

		await That(new[] { 1, 2, 3, }).All().ComplyWith(it => it.HasSideEffect(log, "a"));

		await That(log.Evaluations).IsEqualTo(3);
		await That(log.Effects).IsEqualTo([
			"a deletes a leftover pending snapshot (whole: Success, own: Success, negated: False)",
		]).Because("each item registers the same callback again");
	}

	[Test]
	public async Task WhenFailingBothWays_ShouldRunWithAFailure()
	{
		SideEffectLog log = new();

		async Task Act()
			=> await That(1).HasSideEffect(log, "a", Outcome.FailureBothWays);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             has side effect a,
			             but it did not
			             """);
		await That(log.Effects)
			.IsEqualTo(["a does nothing (whole: Failure, own: FailureBothWays, negated: False)"])
			.Because("nothing negates the whole expectation any more");
	}

	[Test]
	public async Task WhenMet_ShouldDeleteALeftoverPendingSnapshot()
	{
		SideEffectLog log = new();

		await That(1).HasSideEffect(log, "a");

		await That(log.Effects)
			.IsEqualTo(["a deletes a leftover pending snapshot (whole: Success, own: Success, negated: False)"]);
	}

	[Test]
	public async Task WhenNegatedAndMet_ShouldNotHaveTheSideEffect()
	{
		SideEffectLog log = new();

		await That(1).DoesNotComplyWith(it => it.HasSideEffect(log, "a", Outcome.Failure));

		await That(log.Effects).IsEqualTo(["a does nothing (whole: Success, own: Success, negated: True)"]);
	}

	[Test]
	public async Task WhenNotMet_ShouldWriteThePendingSnapshotBeforeTheFailureIsThrown()
	{
		SideEffectLog log = new();
		int effectsWhenThrown = -1;

		async Task Act()
		{
			try
			{
				await That(1).HasSideEffect(log, "a", Outcome.Failure);
			}
			finally
			{
				effectsWhenThrown = log.Effects.Count;
			}
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             has side effect a,
			             but it did not
			             """);
		await That(effectsWhenThrown).IsEqualTo(1);
		await That(log.Effects)
			.IsEqualTo(["a writes the pending snapshot (whole: Failure, own: Failure, negated: False)"]);
	}

	[Test]
	public async Task WhenRegisteredByTwoConstraints_ShouldRunInTheOrderOfRegistration()
	{
		SideEffectLog log = new();

		await That(1).HasSideEffect(log, "b").And.HasSideEffect(log, "a");

		await That(log.Effects).IsEqualTo([
			"b deletes a leftover pending snapshot (whole: Success, own: Success, negated: False)",
			"a deletes a leftover pending snapshot (whole: Success, own: Success, negated: False)",
		]);
	}

	[Test]
	public async Task WhenTheCallbackIsNull_ShouldThrowArgumentNullException()
	{
		Context context = new();

		void Act()
			=> context.OnCompleted(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithMessage("The 'callback' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task WhenTheCallbackThrows_ShouldThrowTheException()
	{
		SideEffectLog log = new()
		{
			ThrowOnCompleted = new InvalidOperationException("the side effect failed"),
		};

		async Task Act()
			=> await That(1).HasSideEffect(log, "a");

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("the side effect failed")
			.Because("hiding the exception would hide a side effect that did not happen");
	}

	[Test]
	public async Task WhenTimedOut_ShouldReceiveTheTokenOfTheCaller()
	{
		SideEffectLog log = new();
		VirtualTimeProvider timeProvider = new();
		using CancellationTokenSource cts = new();
		CancellationToken evaluationToken = default;

		async Task Act()
			=> await That(1).HasSideEffect(log, "a", Outcome.Undecided, cancellationToken =>
				{
					evaluationToken = cancellationToken;
					timeProvider.Advance(2.Seconds());
					return Task.CompletedTask;
				}).WithCancellation(cts.Token).WithTimeout(1.Seconds())
				.WithTimeProvider(timeProvider);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             has side effect a,
			             but it did not finish within 0:01
			             """);
		await That(evaluationToken.IsCancellationRequested).IsTrue()
			.Because("the timeout cancels the token of the evaluation");
		await That(log.Tokens).IsEqualTo([cts.Token])
			.Because("a callback must still be able to finish its side effect after a timeout");
		await That(log.Effects).IsEqualTo(["a does nothing (whole: Failure, own: Undecided, negated: False)"])
			.Because("a timeout fails the expectation");
	}

	[Test]
	public async Task WithoutCallbacks_ShouldCompleteRightAway()
	{
		ValueTaskAwaiter<int> awaiter = That(1).IsEqualTo(1).GetAwaiter();

		await That(awaiter.IsCompleted).IsTrue()
			.Because("an evaluation without callbacks must not start the asynchronous completion");
		await That(awaiter.GetResult()).IsEqualTo(1);
	}
}
