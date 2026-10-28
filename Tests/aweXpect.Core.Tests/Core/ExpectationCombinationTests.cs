using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Synchronous;

namespace aweXpect.Core.Tests.Core;

public sealed class ExpectationCombinationTests
{
	private static Action Taking(VirtualTimeProvider timeProvider, int milliseconds)
		=> () => timeProvider.Advance(milliseconds.Milliseconds());

	public sealed class BecauseTests
	{
		[Test]
		public async Task WhenAllFails_ShouldIncludeTheReasonInTheHeader()
		{
			async Task Act()
				=> await ThatAll(That(1).IsEqualTo(2), That(true).IsTrue()).Because("both must hold");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed, because both must hold:
				              [01] Expected that 1 is equal to 2
				              [02] Expected that true is True
				             but
				              [01] it was 1, which differs by -1
				             """);
		}

		[Test]
		public async Task WhenAnyFails_ShouldIncludeTheReasonInTheHeader()
		{
			async Task Act()
				=> await ThatAny(That(1).IsEqualTo(2), That(2).IsEqualTo(3)).Because("one of them must hold");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected any of the following to succeed, because one of them must hold:
				              [01] Expected that 1 is equal to 2
				              [02] Expected that 2 is equal to 3
				             but
				              [01] it was 1, which differs by -1
				              [02] it was 2, which differs by -1
				             """);
		}

		[Test]
		public async Task WhenAsyncReasonCompletesLater_ShouldIncludeTheReason()
		{
			TaskCompletionSource<string?> becauseSource = new();
			Task combination = Combination();

			becauseSource.SetResult("r1");

			await That(() => combination).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed, because r1:
				              [01] Expected that 1 is equal to 2
				             but
				              [01] it was 1, which differs by -1
				             """);

			async Task Combination()
				=> await ThatAll(That(1).IsEqualTo(2)).Because(becauseSource.Task).WithTimeout(30.Seconds());
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("  ")]
		public async Task WhenAsyncReasonIsNullOrWhitespace_ShouldNotIncludeBecause(string? because)
		{
			async Task Act()
				=> await ThatAll(That(1).IsEqualTo(2)).Because(Task.FromResult(because));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that 1 is equal to 2
				             but
				              [01] it was 1, which differs by -1
				             """);
		}

		[Test]
		public async Task WhenAsyncReasonIsPending_WhenCancellationIsRequested_ShouldReportItAsNotAvailableInTime()
		{
			using CancellationTokenSource cts = new();
			cts.Cancel();

			async Task Act()
				=> await ThatAll(That(1).IsEqualTo(2))
					.Because(PendingTask.Of<string?>())
					.WithCancellation(cts.Token);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed, because the reason was not available in time:
				              [01] Expected that 1 is equal to 2
				             but
				              [01] it was 1, which differs by -1
				             """)
				.Because("the cancellation must stop waiting for a reason that does not arrive");
		}

		[Test]
		public async Task WhenAsyncReasonIsPending_WhenTimeoutElapses_ShouldReportItAsNotAvailableInTime()
		{
			async Task Act()
				=> await ThatAll(That(1).IsEqualTo(2))
					.Because(PendingTask.Of<string?>())
					.WithTimeout(TimeSpan.Zero)
					.WithTimeProvider(new VirtualTimeProvider());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed, because the reason was not available in time:
				              [01] Expected that 1 is equal to 2
				             but
				              [01] it was 1, which differs by -1
				             """)
				.Because("the timeout of the combination must stop waiting for a reason that does not arrive");
		}

		[Test]
		public async Task WhenAsyncReasonIsSet_ShouldIncludeTheReasonInTheHeader()
		{
			async Task Act()
				=> await ThatAll(That(1).IsEqualTo(2)).Because(Task.FromResult<string?>("it is async"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed, because it is async:
				              [01] Expected that 1 is equal to 2
				             but
				              [01] it was 1, which differs by -1
				             """);
		}

		[Test]
		public async Task WhenCombinationIsMet_ShouldNotAwaitTheAsyncReason()
		{
			async Task Act()
				=> await ThatAll(That(1).IsEqualTo(1)).Because(PendingTask.Of<string?>());

			await That(Act).DoesNotThrow()
				.Because("a met combination never builds a failure message, so it must not wait for the reason");
		}

		[Test]
		public async Task WhenMemberHasItsOwnReason_ShouldKeepItOnTheMember()
		{
			async Task Act()
				=> await ThatAll(That(1).IsEqualTo(2).Because("of the member"), That(true).IsTrue())
					.Because("of the combination");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed, because of the combination:
				              [01] Expected that 1 is equal to 2, because of the member
				              [02] Expected that true is True
				             but
				              [01] it was 1, which differs by -1
				             """);
		}

		[Test]
		public async Task WhenNestedCombinationHasAReason_ShouldIncludeItInItsHeader()
		{
			async Task Act()
				=> await ThatAll(
						ThatAny(That(1).IsEqualTo(2), That(2).IsEqualTo(3)).Because(Task.FromResult<string?>("inner")),
						That(true).IsTrue())
					.Because("outer");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed, because outer:
				               Expected any of the following to succeed, because inner:
				                [01] Expected that 1 is equal to 2
				                [02] Expected that 2 is equal to 3
				              [03] Expected that true is True
				             but
				                [01] it was 1, which differs by -1
				                [02] it was 2, which differs by -1
				             """);
		}

		[Test]
		[Arguments(null)]
		[Arguments("")]
		[Arguments("  ")]
		public async Task WhenReasonIsNullOrWhitespace_ShouldNotIncludeBecause(string? because)
		{
			async Task Act()
				=> await ThatAll(That(1).IsEqualTo(2)).Because(because);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that 1 is equal to 2
				             but
				              [01] it was 1, which differs by -1
				             """);
		}

		[Test]
		public async Task WhenReasonIsSetSeveralTimes_ShouldIncludeAllReasonsInOrder()
		{
			async Task Act()
				=> await ThatAll(That(1).IsEqualTo(2)).Because("r1").Because(Task.FromResult<string?>("r2"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed, because r1, because r2:
				              [01] Expected that 1 is equal to 2
				             but
				              [01] it was 1, which differs by -1
				             """);
		}

		[Test]
		public async Task WhenReasonStartsWithBecause_ShouldNotRepeatIt()
		{
			async Task Act()
				=> await ThatAll(That(1).IsEqualTo(2)).Because("because both must hold");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed, because both must hold:
				              [01] Expected that 1 is equal to 2
				             but
				              [01] it was 1, which differs by -1
				             """);
		}

		[Test]
		public async Task WhenTaskIsNull_ShouldThrowArgumentNullException()
		{
			Expectation.Combination.All sut = ThatAll(That(1).IsEqualTo(1));

			void Act() => sut.Because((Task<string?>)null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("reason").And
				.WithMessage("The 'reason' cannot be null.").AsPrefix();
		}
	}

	public sealed class ChainingTests
	{
		[Test]
		public async Task ThatAll_ShouldReturnTheSameCombinationInAnyOrder()
		{
			using CancellationTokenSource cts = new();
			Expectation.Combination.All sut = ThatAll(That(1).IsEqualTo(1));

			Expectation.Combination.All first = sut.Because("r1").WithTimeout(30.Seconds())
				.WithCancellation(cts.Token).Because(Task.FromResult<string?>("r2"));
			Expectation.Combination.All second = ThatAll(That(1).IsEqualTo(1)).WithCancellation(cts.Token)
				.WithTimeout(30.Seconds()).Because("r1");

			await That(first).IsSameAs(sut);
			await second;
		}

		[Test]
		public async Task ThatAny_ShouldReturnTheSameCombinationInAnyOrder()
		{
			using CancellationTokenSource cts = new();
			Expectation.Combination.Any sut = ThatAny(That(1).IsEqualTo(1));

			Expectation.Combination.Any first = sut.WithTimeout(30.Seconds()).Because("r1")
				.WithCancellation(cts.Token);
			Expectation.Combination.Any second = ThatAny(That(1).IsEqualTo(1)).Because(Task.FromResult<string?>("r1"))
				.WithCancellation(cts.Token).WithTimeout(30.Seconds());

			await That(first).IsSameAs(sut);
			await second;
		}
	}

	public sealed class WithCancellationTests
	{
		[Test]
		public async Task WhenCalledTwice_ShouldThrowInvalidOperationException()
		{
			Expectation.Combination.All sut = ThatAll(That(1).IsEqualTo(1)).WithCancellation(CancellationToken.None);

			void Act() => sut.WithCancellation(CancellationToken.None);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("WithCancellation cannot be specified more than once.");
		}

		[Test]
		public async Task WhenMemberHasAnUncancelableToken_ShouldObserveTheTokenOfTheCombination()
		{
			using CancellationTokenSource cts = new();
			cts.Cancel();
			Task<int> subject = PendingTask.Of<int>();

			async Task Act()
				=> await ThatAll(That(subject).IsEqualTo(1).WithCancellation(CancellationToken.None))
					.WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equal to 1
				             but
				              [01] it could not be verified, because the evaluation was already canceled
				             """);
		}

		[Test]
		[Arguments(true, false)]
		[Arguments(false, true)]
		public async Task WhenMemberHasItsOwnToken_ShouldObserveBothTokens(bool cancelCombination, bool cancelMember)
		{
			using CancellationTokenSource combinationCts = new();
			using CancellationTokenSource memberCts = new();
			if (cancelCombination)
			{
				combinationCts.Cancel();
			}

			if (cancelMember)
			{
				memberCts.Cancel();
			}

			Task<int> subject = PendingTask.Of<int>();

			async Task Act()
				=> await ThatAll(That(subject).IsEqualTo(1).WithCancellation(memberCts.Token))
					.WithCancellation(combinationCts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equal to 1
				             but
				              [01] it could not be verified, because the evaluation was already canceled
				             """)
				.Because("a member with its own token observes the token of the combination as well");
		}

		[Test]
		[Arguments(true, false)]
		[Arguments(false, true)]
		public async Task WhenNestedCombinationHasItsOwnToken_ShouldObserveBothTokens(bool cancelOuter,
			bool cancelInner)
		{
			using CancellationTokenSource outerCts = new();
			using CancellationTokenSource innerCts = new();
			if (cancelOuter)
			{
				outerCts.Cancel();
			}

			if (cancelInner)
			{
				innerCts.Cancel();
			}

			Task<int> subject = PendingTask.Of<int>();

			async Task Act()
				=> await ThatAll(ThatAll(That(subject).IsEqualTo(1)).WithCancellation(innerCts.Token))
					.WithCancellation(outerCts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				               Expected all of the following to succeed:
				                [01] Expected that subject is equal to 1
				             but
				                [01] it could not be verified, because the evaluation was already canceled
				             """);
		}

		[Test]
		public async Task WhenTokenIsCanceled_ShouldMakeWaitingMembersInconclusive()
		{
			using CancellationTokenSource cts = new();
			cts.Cancel();
			Task<int> subject = PendingTask.Of<int>();

			async Task Act()
				=> await ThatAll(That(subject).IsEqualTo(1), That(true).IsTrue()).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equal to 1
				              [02] Expected that true is True
				             but
				              [01] it could not be verified, because the evaluation was already canceled
				             """)
				.Because("a member that waits is interrupted like a single expectation, while the other is still verified");
		}

		[Test]
		public async Task WhenTokenIsNotCanceled_ShouldSucceed()
		{
			using CancellationTokenSource cts = new();

			async Task Act()
				=> await ThatAny(That(1).IsEqualTo(1), That(true).IsFalse()).WithCancellation(cts.Token);

			await That(Act).DoesNotThrow();
		}
	}

	public sealed class WithTimeoutTests
	{
		[Test]
		public async Task WhenAMemberHasATighterTimeout_ShouldKeepIt()
		{
			VirtualTimeProvider timeProvider = new();
			Action first = Taking(timeProvider, 50);

			async Task Act()
				=> await ThatAll(That(first).DoesNotThrow().WithTimeout(30.Milliseconds()))
					.WithTimeout(1.Seconds())
					.WithTimeProvider(timeProvider);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that first does not throw any exception
				             but
				              [01] it did not finish within 0:00.030
				             """)
				.Because("the longer timeout of the combination does not loosen the one of the member");
		}

		[Test]
		public async Task WhenAwaitedTwice_ShouldStartTheDeadlineAgain()
		{
			VirtualTimeProvider timeProvider = new();
			Action first = Taking(timeProvider, 60);
			Expectation.Combination.All sut = ThatAll(That(first).DoesNotThrow())
				.WithTimeout(100.Milliseconds())
				.WithTimeProvider(timeProvider);

			async Task Act()
			{
				await sut;
				await sut;
			}

			await That(Act).DoesNotThrow()
				.Because("each evaluation has the whole timeout, like a single expectation");
		}

		[Test]
		public async Task WhenCalledWithALongerTimeout_ShouldNotLoosenTheTighterOne()
		{
			VirtualTimeProvider timeProvider = new();
			Action first = Taking(timeProvider, 60);

			async Task Act()
				=> await ThatAll(That(first).DoesNotThrow())
					.WithTimeout(50.Milliseconds())
					.WithTimeout(1.Seconds())
					.WithTimeout(Timeout.InfiniteTimeSpan)
					.WithTimeProvider(timeProvider);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that first does not throw any exception
				             but
				              [01] it did not finish within 0:00.050
				             """);
		}

		[Test]
		public async Task WhenMembersTogetherExceedTheTimeout_ShouldOnlyGiveTheRemainingTimeToTheLaterMember()
		{
			VirtualTimeProvider timeProvider = new();
			Action first = Taking(timeProvider, 60);
			Action second = Taking(timeProvider, 60);

			async Task Act()
				=> await ThatAll(That(first).DoesNotThrow(), That(second).DoesNotThrow())
					.WithTimeout(100.Milliseconds())
					.WithTimeProvider(timeProvider);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that first does not throw any exception
				              [02] Expected that second does not throw any exception
				             but
				              [02] it did not finish within 0:00.040
				             """)
				.Because("the members are evaluated one after another, so the timeout is one deadline for all of them");
		}

		[Test]
		public async Task WhenNestedCombinationRunsOutOfTime_ShouldOnlyGiveTheRemainingTimeToItsMembers()
		{
			VirtualTimeProvider timeProvider = new();
			Action first = Taking(timeProvider, 60);
			Action second = Taking(timeProvider, 60);

			async Task Act()
				=> await ThatAll(That(first).DoesNotThrow(), ThatAny(That(second).DoesNotThrow()))
					.WithTimeout(100.Milliseconds())
					.WithTimeProvider(timeProvider);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that first does not throw any exception
				               Expected any of the following to succeed:
				                [02] Expected that second does not throw any exception
				             but
				                [02] it did not finish within 0:00.040
				             """);
		}

		[Test]
		public async Task WhenNestedCombinationSetsATighterTimeout_ShouldKeepIt()
		{
			VirtualTimeProvider timeProvider = new();
			Action first = Taking(timeProvider, 50);

			async Task Act()
				=> await ThatAll(ThatAll(That(first).DoesNotThrow()).WithTimeout(30.Milliseconds()))
					.WithTimeout(1.Seconds())
					.WithTimeProvider(timeProvider);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				               Expected all of the following to succeed:
				                [01] Expected that first does not throw any exception
				             but
				                [01] it did not finish within 0:00.030
				             """);
		}

		[Test]
		public async Task WhenTimeoutIsInfinite_ShouldNotLimitTheMembers()
		{
			VirtualTimeProvider timeProvider = new();
			Action first = Taking(timeProvider, 60);
			Action second = Taking(timeProvider, 60);

			async Task Act()
				=> await ThatAll(That(first).DoesNotThrow(), That(second).DoesNotThrow())
					.WithTimeout(Timeout.InfiniteTimeSpan)
					.WithTimeProvider(timeProvider);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenTimeoutIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			Expectation.Combination.All sut = ThatAll(That(1).IsEqualTo(1));

			void Act() => sut.WithTimeout(-2.Milliseconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("timeout").And
				.WithMessage("The timeout must not be negative.").AsPrefix();
		}

		[Test]
		public async Task WhenTimeoutIsZero_ShouldStillVerifyMembersThatCompleteRightAway()
		{
			async Task Act()
				=> await ThatAny(That(1).IsEqualTo(2), That(2).IsEqualTo(3))
					.WithTimeout(TimeSpan.Zero)
					.WithTimeProvider(new VirtualTimeProvider());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected any of the following to succeed:
				              [01] Expected that 1 is equal to 2
				              [02] Expected that 2 is equal to 3
				             but
				              [01] it was 1, which differs by -1
				              [02] it was 2, which differs by -1
				             """)
				.Because("a member that does not wait is decided before the timeout can interrupt it");
		}

		[Test]
		public async Task WhenVerifiedSynchronously_ShouldOnlyGiveTheRemainingTimeToTheLaterMember()
		{
			VirtualTimeProvider timeProvider = new();
			Action first = Taking(timeProvider, 60);
			Action second = Taking(timeProvider, 60);

			void Act()
				=> ThatAll(That(first).DoesNotThrow(), That(second).DoesNotThrow())
					.WithTimeout(100.Milliseconds())
					.WithTimeProvider(timeProvider)
					.VerifySynchronously();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that first does not throw any exception
				              [02] Expected that second does not throw any exception
				             but
				              [02] it did not finish within 0:00.040
				             """);
		}
	}
}
