using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class PollingTests
{
	[Test]
	public async Task Elapsed_AfterACancellation_ShouldStayAtTheTimeOfTheCancellation()
	{
		VirtualTimeSystem time = new();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Start(time, 30.Seconds(), 1.Hours(), cancellation);
		time.CancelAt(40.Milliseconds(), cts);
		await sut.WaitForNextCheck();

		time.Advance(1.Hours());

		await That(sut.Elapsed).IsEqualTo(40.Milliseconds())
			.Because("what follows a cancellation does not count for the budget");
		await That(sut.Remaining).IsEqualTo(30.Seconds() - 40.Milliseconds());
	}

	[Test]
	public async Task GetElapsedTime_ShouldMeasureWithTheTimeSystem()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, 30.Seconds(), 1.Hours(), EvaluationCancellation.None);
		time.Advance(10.Milliseconds());
		long timestamp = sut.GetTimestamp();

		time.Advance(70.Milliseconds());

		await That(sut.GetElapsedTime(timestamp)).IsEqualTo(70.Milliseconds());
		await That(sut.Elapsed).IsEqualTo(80.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_AfterTheLastCheck_ShouldBeElapsed()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, 20.Milliseconds(), 1.Hours(), EvaluationCancellation.None);
		await sut.WaitForNextCheck();

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Elapsed)
			.Because("no check is made after the last one at the end of the budget");
		await That(time.Now).IsEqualTo(20.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_ShouldCheckInTheIntervalAndMakeTheLastCheckAtTheEndOfTheBudget()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, 250.Milliseconds(), 100.Milliseconds(), EvaluationCancellation.None);
		(PollStep Step, TimeSpan Elapsed)[] steps = new (PollStep, TimeSpan)[4];

		for (int i = 0; i < steps.Length; i++)
		{
			steps[i] = (await sut.WaitForNextCheck(), sut.Elapsed);
		}

		await That(steps).IsEqualTo([
			(PollStep.Check, 100.Milliseconds()),
			(PollStep.Check, 200.Milliseconds()),
			(PollStep.LastCheck, 250.Milliseconds()),
			(PollStep.Elapsed, 250.Milliseconds()),
		]).Because("the last wait is shortened to the remaining budget");
	}

	[Test]
	public async Task WaitForNextCheck_WhenACheckTookLongerThanTheBudget_ShouldBeElapsedWithoutWaiting()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, 100.Milliseconds(), 60.Milliseconds(), EvaluationCancellation.None);
		time.Advance(130.Milliseconds());

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Elapsed);
		await That(time.Now).IsEqualTo(130.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_WhenACheckTookTime_ShouldOnlyWaitForTheRemainingBudget()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, 100.Milliseconds(), 60.Milliseconds(), EvaluationCancellation.None);
		time.Advance(70.Milliseconds());

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.LastCheck)
			.Because("the time that the checks take counts for the budget");
		await That(sut.Elapsed).IsEqualTo(100.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_WhenAlreadyCanceled_ShouldBeCanceled()
	{
		VirtualTimeSystem time = new();
		using CancellationTokenSource cts = new();
		cts.Cancel();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Start(time, 30.Seconds(), 1.Hours(), cancellation);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled);
		await That(sut.Elapsed).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task WaitForNextCheck_WhenOnlyASliverOfTheBudgetWouldBeLeft_ShouldMakeTheLastCheck()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, 101.Milliseconds(), 100.Milliseconds(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();
		PollStep nextStep = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.LastCheck)
			.Because("the remaining millisecond is within the tolerance of the timers");
		await That(nextStep).IsEqualTo(PollStep.Elapsed);
		await That(time.Now).IsEqualTo(100.Milliseconds())
			.Because("no wait follows the last check");
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheBudgetIsUnlimited_ShouldCheckAgain()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, Timeout.InfiniteTimeSpan, 1.Milliseconds(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Check);
		await That(sut.Remaining).IsEqualTo(TimeSpan.MaxValue);
		await That(sut.Elapsed).IsEqualTo(1.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheBudgetIsUnlimitedAndTheCallerCancels_ShouldBeCanceled()
	{
		VirtualTimeSystem time = new();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Start(time, Timeout.InfiniteTimeSpan, 1.Hours(), cancellation);
		time.CancelAt(1.Hours(), cts);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled)
			.Because("an unlimited budget can never be used up by a cancellation");
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheBudgetIsUnlimitedAndTheIntervalExceedsTheMaximumWait_ShouldCapTheWait()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, Timeout.InfiniteTimeSpan, 60.Days(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Check);
		await That(time.Now).IsEqualTo(TimeSpan.FromMilliseconds(int.MaxValue))
			.Because("a longer wait is not accepted by the delay of the real time system");
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheBudgetIsUsedUp_ShouldBeElapsed()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, TimeSpan.Zero, 1.Hours(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Elapsed);
		await That(time.Now).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheCallerCancels_ShouldBeCanceled()
	{
		VirtualTimeSystem time = new();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Start(time, 30.Seconds(), 1.Hours(), cancellation);
		time.CancelAt(20.Milliseconds(), cts);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled);
		await That(sut.Elapsed).IsEqualTo(20.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheCallerCancelsAtTheEndOfTheBudget_ShouldMakeTheLastCheck()
	{
		VirtualTimeSystem time = new();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Start(time, 50.Milliseconds(), 1.Hours(), cancellation);
		time.CancelAt(49.Milliseconds(), cts);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.LastCheck)
			.Because("a cancellation within the tolerance of the timers counts as the budget being used up");
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheCallerCancelsDuringALaterWait_ShouldBeCanceled()
	{
		VirtualTimeSystem time = new();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Start(time, 30.Seconds(), 100.Milliseconds(), cancellation);
		time.CancelAt(250.Milliseconds(), cts);

		PollStep[] steps =
		[
			await sut.WaitForNextCheck(), await sut.WaitForNextCheck(), await sut.WaitForNextCheck(),
		];

		await That(steps).IsEqualTo([PollStep.Check, PollStep.Check, PollStep.Canceled,]);
		await That(sut.Elapsed).IsEqualTo(250.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheIntervalExceedsTheBudget_ShouldMakeTheLastCheckAtTheEndOfTheBudget()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, 50.Milliseconds(), 1.Hours(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.LastCheck);
		await That(sut.Elapsed).IsEqualTo(50.Milliseconds())
			.Because("the wait is shortened to the remaining budget");
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheIntervalIsNotPositive_ShouldCheckAgainWithoutWaiting()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, 30.Seconds(), TimeSpan.Zero, EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Check);
		await That(time.Now).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheIntervalIsShorterThanTheBudget_ShouldCheckAgain()
	{
		VirtualTimeSystem time = new();
		using Polling sut = Start(time, 30.Seconds(), 1.Milliseconds(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Check);
		await That(sut.Elapsed).IsEqualTo(1.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheTimeoutElapsedBeforeTheStart_ShouldBeCanceled()
	{
		EvaluationCancellation cancellation = new(100.Milliseconds(), CancellationToken.None);
		TaskCompletionSource<bool> canceled = new();
		using CancellationTokenRegistration _ = cancellation.Token.Register(() => canceled.TrySetResult(true));
		await canceled.Task;
		using Polling sut = Polling.Start(RealTimeSystem.Instance, RealTimeSystem.Instance.GetTimestamp(),
			100.Milliseconds(), 1.Hours(), cancellation);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled)
			.Because("the timeout left no time for the budget, so it is reported as the timeout");
		cancellation.Release();
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheTimeoutIsShorterThanTheBudget_ShouldBeCanceled()
	{
		EvaluationCancellation cancellation = new(20.Milliseconds(), CancellationToken.None);
		using Polling sut = Polling.Start(RealTimeSystem.Instance, RealTimeSystem.Instance.GetTimestamp(),
			30.Seconds(), 1.Hours(), cancellation);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled)
			.Because("a shorter timeout ends the checks, so that it is reported as the timeout");
		cancellation.Release();
	}

	private static Polling Start(VirtualTimeSystem time, TimeSpan budget, TimeSpan interval,
		EvaluationCancellation cancellation)
		=> Polling.Start(time, time.GetTimestamp(), budget, interval, cancellation);
}
