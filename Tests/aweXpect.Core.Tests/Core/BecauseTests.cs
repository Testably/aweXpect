using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Equivalency;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core;

public class BecauseTests
{
	[Test]
	public async Task ActionDelegate_ShouldApplyAsyncBecauseReason()
	{
		string because = "this is the reason";
		Task<string?> becauseTask = Task.Delay(5).ContinueWith(_ => because)!;
		Action subject = () => throw new MyException();

		async Task Act() => await That(subject).DoesNotThrow().Because(becauseTask);

		await That(Act).Throws().WithMessage($"*{because}*").AsWildcard();
	}

	[Test]
	public async Task ActionDelegate_ShouldApplyBecauseReason()
	{
		string because = "this is the reason";
		Action subject = () => throw new MyException();

		async Task Act() => await That(subject).DoesNotThrow().Because(because);

		await That(Act).Throws().WithMessage($"*{because}*").AsWildcard();
	}

	[Test]
	[Arguments(null)]
	[Arguments("")]
	[Arguments("  ")]
	public async Task ActionDelegate_WhenAsyncReasonIsNullOrWhitespace_ShouldNotIncludeBecause(string? because)
	{
		Task<string?> becauseTask = Task.FromResult(because);
		Action subject = () => throw new MyException();

		async Task Act() => await That(subject).DoesNotThrow().Because(becauseTask);

		Exception exception = await That(Act).Throws();
		await That(exception.Message).DoesNotContain("because");
	}

	[Test]
	[Arguments(null)]
	[Arguments("")]
	[Arguments("  ")]
	public async Task ActionDelegate_WhenReasonIsNullOrWhitespace_ShouldNotIncludeBecause(string? because)
	{
		Action subject = () => throw new MyException();

		async Task Act() => await That(subject).DoesNotThrow().Because(because);

		Exception exception = await That(Act).Throws();
		await That(exception.Message).DoesNotContain("because");
	}

	[Test]
	public async Task ASpecifiedAsyncBecauseReason_ShouldBeIncludedInMessage()
	{
		Task<string?> becauseTask = Task.FromResult<string?>("I want to test an async 'because'");
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because(becauseTask);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False, because I want to test an async 'because',
			             but it was True
			             """);
	}

	[Test]
	public async Task ASpecifiedBecauseReason_ShouldBeIncludedInMessage()
	{
		string because = "I want to test 'because'";
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because(because);

		await That(Act).Throws().WithMessage($"*{because}*").AsWildcard();
	}

	[Test]
	public async Task FuncDelegate_ShouldApplyAsyncBecauseReason()
	{
		string because = "this is the reason";
		Task<string?> becauseTask = Task.Delay(5).ContinueWith(_ => because)!;
		Func<int> subject = () => throw new MyException();

		async Task Act() => await That(subject).DoesNotThrow().Because(becauseTask);

		await That(Act).Throws().WithMessage($"*{because}*").AsWildcard();
	}

	[Test]
	public async Task FuncDelegate_ShouldApplyBecauseReason()
	{
		string because = "this is the reason";
		Func<int> subject = () => throw new MyException();

		async Task Act() => await That(subject).DoesNotThrow().Because(because);

		await That(Act).Throws().WithMessage($"*{because}*").AsWildcard();
	}

	[Test]
	[Arguments("we prefix the reason", "because we prefix the reason")]
	[Arguments("  we ignore whitespace", "because we ignore whitespace")]
	[Arguments("because we honor a leading 'because'", "because we honor a leading 'because'")]
	public async Task ShouldPrefixReasonWithBecause(string because, string expectedWithPrefix)
	{
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because(because);

		await That(Act).Throws().WithMessage($"*{expectedWithPrefix}*")
			.AsWildcard();
	}

	[Test]
	public async Task WhenApplyBecauseReasonMultipleTimes_ShouldNotOverwritePreviousReason()
	{
		string because1 = "this is the first reason";
		string because2 = "this is the second reason";
		bool subject = false;

		async Task Act() => await That(subject).IsTrue().Because(because1)
			.And.IsFalse().Because(because2);

		await That(Act).Throws().WithMessage($"*{because1}*").AsWildcard();
	}

	[Test]
	public async Task WhenAsyncReasonIsCancelled_ShouldStillReportTheAssertionFailure()
	{
		TaskCompletionSource<string?> becauseSource = new();
		becauseSource.SetCanceled();
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because(becauseSource.Task);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False, because the reason did throw a TaskCanceledException: *,
			             but it was True
			             """).AsWildcard();
	}

	[Test]
	[Arguments(null)]
	[Arguments("")]
	[Arguments("  ")]
	public async Task WhenAsyncReasonIsNullOrWhitespace_ShouldNotIncludeBecause(string? because)
	{
		Task<string?> becauseTask = Task.FromResult(because);
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because(becauseTask);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False,
			             but it was True
			             """);
	}

	[Test]
	public async Task WhenAsyncReasonIsPending_WhenAbandonedReasonFaultsLater_ShouldNotRaiseUnobservedTaskException()
	{
		MyException exception = new();
		bool isRaised = false;
		EventHandler<UnobservedTaskExceptionEventArgs> handler = (_, e) =>
		{
			if (e.Exception.InnerExceptions.Contains(exception))
			{
				isRaised = true;
			}
		};

		TaskScheduler.UnobservedTaskException += handler;
		bool isCompleted;
		try
		{
			isCompleted = await AbandonReasonThatFaultsLater(exception);
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		finally
		{
			TaskScheduler.UnobservedTaskException -= handler;
		}

		await That(isCompleted).IsTrue()
			.Because("the cancellation must stop waiting for the reason");
		await That(isRaised).IsFalse()
			.Because("the exception of an abandoned reason must be observed, as nobody else awaits it");
	}

	[Test]
	public async Task WhenAsyncReasonIsPending_WhenCancellationIsRequested_ShouldStillReportTheAssertionFailure()
	{
		TaskCompletionSource<string?> becauseSource = new();
		using CancellationTokenSource cts = new();
		bool subject = false;
		Task evaluation = Evaluate();

		cts.Cancel();

		await That(await IsCompletedWithoutTheReason(evaluation, becauseSource)).IsTrue()
			.Because("the cancellation must stop waiting for a reason that does not arrive");
		await That(() => evaluation).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is True, because the reason was not available in time,
			             but it was False
			             """);

		async Task Evaluate()
			=> await That(subject).IsTrue().Because(becauseSource.Task).WithCancellation(cts.Token);
	}

	[Test]
	public async Task WhenAsyncReasonIsPending_WhenItCompletesWithinTheTimeout_ShouldIncludeTheReason()
	{
		TaskCompletionSource<string?> becauseSource = new();
		bool subject = false;
		Task evaluation = Evaluate();

		becauseSource.SetResult("r1");

		await That(() => evaluation).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is True, because r1,
			             but it was False
			             """);

		async Task Evaluate()
			=> await That(subject).IsTrue().Because(becauseSource.Task).WithTimeout(30.Seconds());
	}

	[Test]
	public async Task WhenAsyncReasonIsPending_WhenTimeoutElapses_ShouldStillReportTheAssertionFailure()
	{
		TaskCompletionSource<string?> becauseSource = new();
		bool subject = false;
		Task evaluation = Evaluate();

		await That(await IsCompletedWithoutTheReason(evaluation, becauseSource)).IsTrue()
			.Because("the timeout must stop waiting for a reason that does not arrive");
		await That(() => evaluation).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is True, because the reason was not available in time,
			             but it was False
			             """);

		async Task Evaluate()
			=> await That(subject).IsTrue().Because(becauseSource.Task).WithTimeout(50.Milliseconds());
	}

	[Test]
	public async Task
		WhenAsyncReasonIsPending_WhenTimeoutIsLongerThanTheRetryBudgetOfEventually_ShouldStillReportTheAssertionFailure()
	{
		TaskCompletionSource<string?> becauseSource = new();
		VirtualTimeSystem timeSystem = new();
		int calls = 0;
		Task evaluation = Evaluate();

		await That(await IsCompletedWithoutTheReason(evaluation, becauseSource)).IsTrue()
			.Because("the timeout must stop waiting for the reason, although it does not cancel the attempts");
		await That(() => evaluation).Throws<FailException>()
			.WithMessage("""
			             Expected that Subject
			             eventually is equal to 2 within 0:00.020, because the reason was not available in time,
			             but it was 1, which differs by -1
			             """);
		await That(calls).IsEqualTo(2)
			.Because("the wait of the virtual clock leaves exactly one more attempt at the end of the budget");

		int Subject()
		{
			if (++calls == 2)
			{
				timeSystem.Advance(10.Milliseconds());
			}

			return 1;
		}

		async Task Evaluate()
			=> await That(Subject).Eventually().OnVirtualTime(timeSystem).Within(20.Milliseconds()).IsEqualTo(2)
				.Because(becauseSource.Task)
				.WithTimeout(30.Milliseconds());
	}

	[Test]
	public async Task WhenAsyncReasonIsSlow_WhenExpectationIsMet_ShouldNotAwaitTheReason()
	{
		bool reasonWasResolved = false;
		Task<string?> becauseTask = Task.Delay(TimeSpan.FromSeconds(30)).ContinueWith(_ =>
		{
			reasonWasResolved = true;
			return (string?)"of reasons";
		});

		await That(1).IsEqualTo(1).Because(becauseTask);

		await That(reasonWasResolved).IsFalse()
			.Because("a met expectation never builds a failure message, so it must not wait for the reason");
	}

	[Test]
	public async Task WhenAsyncReasonOfAMemberIsNull_ShouldNotIncludeBecause()
	{
		string subject = "foo";

		async Task Act()
			=> await That(subject).Whose(s => s.Length, l => l.IsEqualTo(5).Because(Task.FromResult<string?>(null)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             whose Length is equal to 5,
			             but Length was 3, which differs by -2
			             """);
	}

	[Test]
	public async Task WhenAsyncReasonOfAMemberIsPending_WhenTimeoutElapses_ShouldStillReportTheAssertionFailure()
	{
		TaskCompletionSource<string?> becauseSource = new();
		string subject = "foo";
		Task evaluation = Evaluate();

		await That(await IsCompletedWithoutTheReason(evaluation, becauseSource)).IsTrue()
			.Because("the timeout must stop waiting for a reason that does not arrive");
		await That(() => evaluation).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             whose Length is equal to 5, because the reason was not available in time,
			             but Length was 3, which differs by -2
			             """);

		async Task Evaluate()
			=> await That(subject).Whose(s => s.Length, l => l.IsEqualTo(5).Because(becauseSource.Task))
				.WithTimeout(50.Milliseconds());
	}

	[Test]
	public async Task WhenAsyncReasonOfAMemberOfANullSubject_ShouldIncludeTheReason()
	{
		string? subject = null;

		async Task Act()
			=> await That(subject).Whose(s => s?.Length,
				l => l.IsEqualTo(5).Because(Task.FromResult<string?>("it must be long")));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             whose Length is equal to 5, because it must be long,
			             but it was <null>
			             """)
			.Because("the expectation on the member is only described, but its reason is resolved for the failure");
	}

	[Test]
	public async Task
		WhenAsyncReasonOfAMetExpectationFaultsLater_OnABlockedTaskScheduler_ShouldNotRaiseUnobservedTaskException()
	{
		MyException exception = new();
		bool isRaised = false;
		EventHandler<UnobservedTaskExceptionEventArgs> handler = (_, e) =>
		{
			if (e.Exception.InnerExceptions.Contains(exception))
			{
				isRaised = true;
			}
		};

		TaskScheduler.UnobservedTaskException += handler;
		try
		{
			MeetExpectationOnABlockedTaskSchedulerWithReasonThatFaultsLater(exception);
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		finally
		{
			TaskScheduler.UnobservedTaskException -= handler;
		}

		await That(isRaised).IsFalse()
			.Because("the exception must be observed without the scheduler of the code that added the reason");
	}

	[Test]
	public async Task WhenAsyncReasonStartsWithBecause_ShouldHonorExistingPrefix()
	{
		int subject = 1;

		async Task Act()
			=> await That(subject).IsEqualTo(2).Because(Task.FromResult<string?>("because it must be two"));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 2, because it must be two,
			             but it was 1, which differs by -1
			             """);
	}

	[Test]
	public async Task WhenAsyncReasonThrowsAnExceptionWhoseMessageThrows_ShouldStillReportTheAssertionFailure()
	{
		Task<string?> becauseTask = Task.FromException<string?>(new ThrowingMessageException());
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because(becauseTask);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is False, because the reason did throw a ThrowingMessageException: [Message of ThrowingMessageException did throw an InvalidOperationException],
			             but it was True
			             """);
	}

	[Test]
	public async Task WhenAsyncReasonThrows_WhenExpectationFails_ShouldEscapeLineBreaksInTheMessage()
	{
		Task<string?> becauseTask = Task.FromException<string?>(new MyException("the reason provider\nis broken"));
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because(becauseTask);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False, because the reason did throw a MyException: the reason provider\nis broken,
			             but it was True
			             """)
			.Because("the reason must stay on the line of the expectation");
	}

	[Test]
	public async Task WhenAsyncReasonThrows_WhenExpectationFails_ShouldStillReportTheAssertionFailure()
	{
		Task<string?> becauseTask = Task.FromException<string?>(new MyException("the reason provider is broken"));
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because(becauseTask);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False, because the reason did throw a MyException: the reason provider is broken,
			             but it was True
			             """);
	}

	[Test]
	public async Task WhenAsyncReasonThrows_WhenExpectationIsMet_ShouldNotThrow()
	{
		Task<string?> becauseTask = Task.FromException<string?>(new MyException("the reason provider is broken"));

		async Task Act() => await That(1).IsEqualTo(1).Because(becauseTask);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenAwaitedTwice_ShouldKeepTheAsyncReason()
	{
		bool subject = true;
		AndOrResult<bool, IThat<bool>> expectation =
			That(subject).IsFalse().Because(Task.FromResult<string?>("it must be false"));

		async Task Act()
			=> await expectation;

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is False, because it must be false,
			             but it was True
			             """);
		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is False, because it must be false,
			             but it was True
			             """)
			.Because("the reason that the first evaluation resolved is kept");
	}

	[Test]
	public async Task WhenAwaitedTwice_WhenAsyncReasonIsNull_ShouldNotIncludeBecause()
	{
		bool subject = true;
		AndOrResult<bool, IThat<bool>> expectation =
			That(subject).IsFalse().Because(Task.FromResult<string?>(null));

		async Task Act()
			=> await expectation;

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is False,
			             but it was True
			             """);
		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is False,
			             but it was True
			             """)
			.Because("the empty reason that the first evaluation resolved is still ignored");
	}

	[Test]
	public async Task WhenCombinedWithWhichContinuation_ShouldAppendReasonAfterTheContinuation()
	{
		Action subject = () => throw new MyException("foo");

		async Task Act() => await That(subject).Throws<MyException>().Because("of reasons")
			.WithMessage("bar");

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             throws a MyException with message equal to "bar", because of reasons,
			             but it had message "foo", which differs at index 0:
			                ↓ (actual)
			               "foo"
			               "bar"
			                ↑ (expected)

			             Message:
			             foo
			             """);
	}

	[Test]
	public async Task WhenCombineWithAnd_ShouldAppendReasonAfterAllConstraints()
	{
		string because = "we append it after all constraints";
		bool subject = true;

		async Task Act() => await That(subject).IsTrue().Because(because)
			.And.IsFalse();

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is True and is False, because we append it after all constraints,
			             but it was True
			             """);
	}

	[Test]
	public async Task WhenCombineWithAnd_ShouldApplyBecauseReason()
	{
		string because1 = "this is the first reason";
		string because2 = "this is the second reason";
		bool subject = true;

		async Task Act() => await That(subject).IsTrue().Because(because1)
			.And.IsFalse().Because(because2);

		await That(Act).Throws().WithMessage($"*{because2}*").AsWildcard();
	}

	[Test]
	public async Task WhenCombineWithAnd_WithReasonOnEachConstraint_ShouldAppendAllReasonsInOrder()
	{
		bool subject = true;

		async Task Act() => await That(subject).IsTrue().Because("of the first reason")
			.And.IsFalse().Because(Task.FromResult<string?>("of the second reason"));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is True and is False, because of the first reason, because of the second reason,
			             but it was True
			             """);
	}

	[Test]
	public async Task WhenCombineWithOr_ShouldAppendReasonAfterAllConstraints()
	{
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because("of reasons")
			.Or.IsFalse();

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False or is False, because of reasons,
			             but it was True
			             """);
	}

	[Test]
	public async Task WhenCombineWithOr_ShouldApplyBecauseReason()
	{
		string because1 = "this is the first reason";
		string because2 = "this is the second reason";
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because(because1)
			.Or.IsFalse().Because(because2);

		await That(Act).Throws().WithMessage($"*{because1}*{because2}*")
			.AsWildcard();
	}

	[Test]
	public async Task WhenNoBecauseReasonIsGiven_ShouldNotIncludeBecause()
	{
		bool subject = true;

		async Task Act() => await That(subject).IsFalse();

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False,
			             but it was True
			             """);
	}

	[Test]
	[Arguments(null)]
	[Arguments("")]
	[Arguments("  ")]
	public async Task WhenReasonIsNullOrWhitespace_ShouldNotIncludeBecause(string? because)
	{
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because(because);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False,
			             but it was True
			             """);
	}

	[Test]
	public async Task WhenReasonStartsWithBecause_ShouldHonorExistingPrefix()
	{
		string because = "because we honor a leading 'because'";
		bool subject = true;

		async Task Act() => await That(subject).IsFalse().Because(because);

		Exception exception = await That(Act).Throws()
			.WithMessage("*because*").AsWildcard();
		await That(exception.Message).DoesNotContain("because because");
	}

	[Test]
	public async Task WhenUsedInExpectThatAll_ShouldAppendReasonToEachExpectation()
	{
		async Task Act() => await ThatAll(
			That(true).IsFalse().Because("of the first reason").And.IsTrue(),
			That(1).IsEqualTo(2).Because("of the second reason"));

		await That(Act).Throws()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that true is False and is True, because of the first reason
			              [02] Expected that 1 is equal to 2, because of the second reason
			             but
			              [01] it was True
			              [02] it was 1, which differs by -1
			             """);
	}

	[Test]
	public async Task WhenUsedInExpectThatAll_WhenAllExpectationsAreMet_ShouldNotAwaitTheAsyncReason()
	{
		Task<string?> becauseTask = PendingTask.Of<string?>();

		async Task Act() => await ThatAll(
			That(true).IsTrue().Because(becauseTask),
			That(1).IsEqualTo(1));

		await That(Act).DoesNotThrow()
			.Because("a met combination never builds a failure message, so it must not wait for the reason");
	}

	[Test]
	public async Task WhenUsedInExpectThatAll_WhenAsyncReasonOfAMetExpectationCompletesLater_ShouldAwaitTheReason()
	{
		TaskCompletionSource<string?> becauseSource = new();
		Task combination = Combination();

		becauseSource.SetResult("r1");

		await That(() => combination).Throws()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that true is True, because r1
			              [02] Expected that 1 is equal to 2
			             but
			              [02] it was 1, which differs by -1
			             """);

		async Task Combination() => await ThatAll(
			That(true).IsTrue().Because(becauseSource.Task),
			That(1).IsEqualTo(2));
	}

	[Test]
	public async Task
		WhenUsedInExpectThatAll_WhenAsyncReasonOfAMetExpectationFaultsLater_ShouldNotRaiseUnobservedTaskException()
	{
		MyException exception = new();
		bool isRaised = false;
		EventHandler<UnobservedTaskExceptionEventArgs> handler = (_, e) =>
		{
			if (e.Exception.InnerExceptions.Contains(exception))
			{
				isRaised = true;
			}
		};

		TaskScheduler.UnobservedTaskException += handler;
		try
		{
			await MeetCombinationWithReasonThatFaultsLater(exception);
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		finally
		{
			TaskScheduler.UnobservedTaskException -= handler;
		}

		await That(isRaised).IsFalse()
			.Because("the exception of a reason that is never awaited must be observed");
	}

	[Test]
	public async Task WhenUsedInExpectThatAll_WhenAsyncReasonOfAMetExpectationIsPending_ShouldStopWaitingAtItsTimeout()
	{
		TaskCompletionSource<string?> becauseSource = new();
		Task evaluation = Evaluate();

		await That(await IsCompletedWithoutTheReason(evaluation, becauseSource)).IsTrue()
			.Because("the timeout of the met expectation must stop waiting for a reason that does not arrive");
		await That(() => evaluation).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that true is True, because the reason was not available in time
			              [02] Expected that 1 is equal to 2
			             but
			              [02] it was 1, which differs by -1
			             """);

		async Task Evaluate() => await ThatAll(
			That(true).IsTrue().Because(becauseSource.Task).WithTimeout(50.Milliseconds()),
			That(1).IsEqualTo(2));
	}

	[Test]
	[Arguments(null)]
	[Arguments("")]
	[Arguments("  ")]
	public async Task WhenUsedInExpectThatAll_WhenAsyncReasonOfAMetExpectationIsNullOrWhitespace_ShouldNotIncludeBecause(
		string? because)
	{
		async Task Act() => await ThatAll(
			That(true).IsTrue().Because(Task.FromResult(because)),
			That(1).IsEqualTo(2));

		await That(Act).Throws()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that true is True
			              [02] Expected that 1 is equal to 2
			             but
			              [02] it was 1, which differs by -1
			             """);
	}

	[Test]
	public async Task WhenUsedInExpectThatAll_WhenAsyncReasonOfAMetExpectationThrows_ShouldKeepTheExpectationMet()
	{
		Task<string?> becauseTask = Task.FromException<string?>(new MyException("the reason provider is broken"));

		async Task Act() => await ThatAll(
			That(true).IsTrue().Because(becauseTask),
			That(1).IsEqualTo(2));

		await That(Act).Throws()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that true is True, because the reason did throw a MyException: the reason provider is broken
			              [02] Expected that 1 is equal to 2
			             but
			              [02] it was 1, which differs by -1
			             """)
			.Because("a broken reason provider must not fail a met expectation");
	}

	[Test]
	public async Task WhenUsedInExpectThatAll_WithAsyncReasonOnAMetExpectation_ShouldAppendTheReason()
	{
		async Task Act() => await ThatAll(
			That(true).IsTrue().Because(Task.FromResult<string?>("r1")),
			That(1).IsEqualTo(2));

		await That(Act).Throws()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that true is True, because r1
			              [02] Expected that 1 is equal to 2
			             but
			              [02] it was 1, which differs by -1
			             """);
	}

	[Test]
	public async Task WhenUsedInExpectThatAll_WithAsyncReasonOnAMetExpectationOfANestedCombination_ShouldAppendTheReason()
	{
		async Task Act() => await ThatAll(
			ThatAll(
				That(true).IsTrue().Because(Task.FromResult<string?>("r1")),
				That(2).IsEqualTo(2)),
			That(1).IsEqualTo(2));

		await That(Act).Throws()
			.WithMessage("""
			             Expected all of the following to succeed:
			               Expected all of the following to succeed:
			                [01] Expected that true is True, because r1
			                [02] Expected that 2 is equal to 2
			              [03] Expected that 1 is equal to 2
			             but
			              [03] it was 1, which differs by -1
			             """);
	}

	[Test]
	public async Task WhenUsedInExpectThatAll_WithReasonsOnSeveralExpectations_ShouldAppendAllReasons()
	{
		async Task Act() => await ThatAll(
			That(true).IsTrue().Because(Task.FromResult<string?>("r1")).And.IsNotEqualTo(false).Because("r2"),
			That(2).IsEqualTo(2).Because("r3"),
			That(3).IsEqualTo(3).Because(Task.FromResult<string?>("r4")),
			That(1).IsEqualTo(2).Because(Task.FromResult<string?>("r5")));

		await That(Act).Throws()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that true is True and is not False, because r1, because r2
			              [02] Expected that 2 is equal to 2, because r3
			              [03] Expected that 3 is equal to 3, because r4
			              [04] Expected that 1 is equal to 2, because r5
			             but
			              [04] it was 1, which differs by -1
			             """);
	}

	[Test]
	public async Task WhenUsedInExpectThatAny_WhenAnOuterCombinationFails_ShouldAppendTheAsyncReasonOfAMetExpectation()
	{
		async Task Act() => await ThatAll(
			ThatAny(
				That(true).IsTrue().Because(Task.FromResult<string?>("r1")),
				That(2).IsEqualTo(3).Because(Task.FromResult<string?>("r2"))),
			That(1).IsEqualTo(2));

		await That(Act).Throws()
			.WithMessage("""
			             Expected all of the following to succeed:
			               Expected any of the following to succeed:
			                [01] Expected that true is True, because r1
			                [02] Expected that 2 is equal to 3, because r2
			              [03] Expected that 1 is equal to 2
			             but
			              [03] it was 1, which differs by -1
			             """);
	}

	[Test]
	public async Task WhenUsedInItIs_ShouldApplyAsyncBecauseReason()
	{
		var actual = new
		{
			Value = 1,
		};
		var expected = new
		{
			Value = It.Is<int>().That.IsGreaterThan(2).Because(Task.FromResult<string?>("it must be large")),
		};

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is equivalent to expected,
			             but it was not:
			               Property Value differed:
			                   Actual: 1
			                 Expected: is int that is greater than 2, because it must be large

			             Equivalency options:
			              - include public fields and properties
			             """);
	}

	[Test]
	public async Task WhenUsedInItIs_WhenAsyncReasonIsNull_ShouldNotIncludeBecause()
	{
		var actual = new
		{
			Value = 1,
		};
		var expected = new
		{
			Value = It.Is<int>().That.IsGreaterThan(2).Because(Task.FromResult<string?>(null)),
		};

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is equivalent to expected,
			             but it was not:
			               Property Value differed:
			                   Actual: 1
			                 Expected: is int that is greater than 2

			             Equivalency options:
			              - include public fields and properties
			             """);
	}

	[Test]
	public async Task WhenUsedInItIs_WhenExpectationIsMet_ShouldNotAwaitTheAsyncReason()
	{
		TaskCompletionSource<string?> becauseSource = new();
		var actual = new
		{
			Value = 3,
		};
		var expected = new
		{
			Value = It.Is<int>().That.IsGreaterThan(2).Because(becauseSource.Task),
		};

		Task evaluation = Evaluate();

		await That(await IsCompletedWithoutTheReason(evaluation, becauseSource)).IsTrue()
			.Because("the reason is only needed for a failure message");

		async Task Evaluate()
			=> await That(actual).IsEquivalentTo(expected);
	}

	/// <summary>
	///     Fails an expectation whose reason is still pending when the evaluation is canceled, and lets the reason fault
	///     afterwards.
	/// </summary>
	/// <remarks>
	///     The reason is only reachable from within this method, so that it can be collected afterwards.
	/// </remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static async Task<bool> AbandonReasonThatFaultsLater(Exception exception)
	{
		TaskCompletionSource<string?> becauseSource = new();
		using CancellationTokenSource cts = new();
		Task evaluation = Evaluate();
		cts.Cancel();

		await Task.WhenAny(evaluation, Task.Delay(TimeSpan.FromSeconds(10)));
		bool isCompleted = evaluation.IsCompleted;
		becauseSource.SetException(exception);
		await That(() => evaluation).Throws<FailException>();
		return isCompleted;

		async Task Evaluate()
			=> await That(false).IsTrue().Because(becauseSource.Task).WithCancellation(cts.Token);
	}

	/// <summary>
	///     Waits for the <paramref name="evaluation" /> whose reason from the <paramref name="becauseSource" /> never
	///     arrives, and returns whether it completed.
	/// </summary>
	/// <remarks>
	///     It gives up after ten seconds and then provides no reason, so that an evaluation that waits for the reason
	///     fails the test instead of hanging the test run.
	/// </remarks>
	private static async Task<bool> IsCompletedWithoutTheReason(Task evaluation,
		TaskCompletionSource<string?> becauseSource)
	{
		await Task.WhenAny(evaluation, Task.Delay(TimeSpan.FromSeconds(10)));
		bool isCompleted = evaluation.IsCompleted;
		becauseSource.TrySetResult(null);
		return isCompleted;
	}

	/// <summary>
	///     Meets a combination with a reason on one of its expectations, and lets the reason fault afterwards.
	/// </summary>
	/// <remarks>
	///     The reason is only reachable from within this method, so that it can be collected afterwards.
	/// </remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static async Task MeetCombinationWithReasonThatFaultsLater(Exception exception)
	{
		TaskCompletionSource<string?> becauseSource = new();
		await ThatAll(
			That(true).IsTrue().Because(becauseSource.Task),
			That(1).IsEqualTo(1));
		becauseSource.SetException(exception);
	}

	/// <summary>
	///     Meets an expectation with a reason as a task of a <see cref="BlockedTaskScheduler" />, and lets the reason
	///     fault afterwards.
	/// </summary>
	/// <remarks>
	///     The reason is only reachable from within this method, so that it can be collected afterwards.
	/// </remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void MeetExpectationOnABlockedTaskSchedulerWithReasonThatFaultsLater(Exception exception)
	{
		TaskCompletionSource<string?> becauseSource = new();
		BlockedTaskScheduler.Run(() => That(1).IsEqualTo(1).Because(becauseSource.Task).GetAwaiter().GetResult());
		becauseSource.SetException(exception);
	}
}
