using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class PollingTests
{
	[Test]
	public async Task Elapsed_AfterACancellation_ShouldStayAtTheTimeOfTheCancellation()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Start(time, 30.Seconds(), 1.Hours(), cancellation);
		time.CancelAfter(cts, 40.Milliseconds());
		await sut.WaitForNextCheck();

		timeSystem.TimeProvider.AdvanceBy(1.Hours());

		await That(sut.Elapsed).IsEqualTo(40.Milliseconds())
			.Because("what follows a cancellation does not count for the budget");
		await That(sut.Remaining).IsEqualTo(30.Seconds() - 40.Milliseconds());
	}

	[Test]
	public async Task GetElapsedTime_ShouldMeasureWithTheTimeProvider()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		using Polling sut = Start(time, 30.Seconds(), 1.Hours(), EvaluationCancellation.None);
		timeSystem.TimeProvider.AdvanceBy(10.Milliseconds());
		long timestamp = sut.GetTimestamp();

		timeSystem.TimeProvider.AdvanceBy(70.Milliseconds());

		await That(sut.GetElapsedTime(timestamp)).IsEqualTo(70.Milliseconds());
		await That(sut.Elapsed).IsEqualTo(80.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_AfterTheLastCheck_ShouldBeElapsed()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		long start = time.GetTimestamp();
		using Polling sut = Start(time, 20.Milliseconds(), 1.Hours(), EvaluationCancellation.None);
		await sut.WaitForNextCheck();

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Elapsed)
			.Because("no check is made after the last one at the end of the budget");
		await That(time.GetElapsedTime(start)).IsEqualTo(20.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_ShouldCheckInTheIntervalAndMakeTheLastCheckAtTheEndOfTheBudget()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
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
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		long start = time.GetTimestamp();
		using Polling sut = Start(time, 100.Milliseconds(), 60.Milliseconds(), EvaluationCancellation.None);
		timeSystem.TimeProvider.AdvanceBy(130.Milliseconds());

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Elapsed);
		await That(time.GetElapsedTime(start)).IsEqualTo(130.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_WhenACheckTookTime_ShouldOnlyWaitForTheRemainingBudget()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		using Polling sut = Start(time, 100.Milliseconds(), 60.Milliseconds(), EvaluationCancellation.None);
		timeSystem.TimeProvider.AdvanceBy(70.Milliseconds());

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.LastCheck)
			.Because("the time that the checks take counts for the budget");
		await That(sut.Elapsed).IsEqualTo(100.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_WhenAlreadyCanceled_ShouldBeCanceled()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
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
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		long start = time.GetTimestamp();
		using Polling sut = Start(time, 101.Milliseconds(), 100.Milliseconds(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();
		PollStep nextStep = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.LastCheck)
			.Because("the remaining millisecond is within the tolerance of the timers");
		await That(nextStep).IsEqualTo(PollStep.Elapsed);
		await That(time.GetElapsedTime(start)).IsEqualTo(100.Milliseconds())
			.Because("no wait follows the last check");
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheBudgetIsUnlimited_ShouldCheckAgain()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		using Polling sut = Start(time, Timeout.InfiniteTimeSpan, 1.Milliseconds(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Check);
		await That(sut.Remaining).IsEqualTo(TimeSpan.MaxValue);
		await That(sut.Elapsed).IsEqualTo(1.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheBudgetIsUnlimitedAndTheCallerCancels_ShouldBeCanceled()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Start(time, Timeout.InfiniteTimeSpan, 1.Hours(), cancellation);
		time.CancelAfter(cts, 1.Hours());

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled)
			.Because("an unlimited budget can never be used up by a cancellation");
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheBudgetIsUnlimitedAndTheIntervalExceedsTheMaximumWait_ShouldCapTheWait()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		long start = time.GetTimestamp();
		using Polling sut = Start(time, Timeout.InfiniteTimeSpan, 60.Days(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Check);
		await That(time.GetElapsedTime(start)).IsEqualTo(TimeSpan.FromMilliseconds(int.MaxValue))
			.Because("a longer wait is not accepted by the delay of the system time provider");
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheBudgetIsUsedUp_ShouldBeElapsed()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		long start = time.GetTimestamp();
		using Polling sut = Start(time, TimeSpan.Zero, 1.Hours(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Elapsed);
		await That(time.GetElapsedTime(start)).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheCallerCancels_ShouldBeCanceled()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Start(time, 30.Seconds(), 1.Hours(), cancellation);
		time.CancelAfter(cts, 20.Milliseconds());

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled);
		await That(sut.Elapsed).IsEqualTo(20.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheCallerCancels_ShouldReleaseTheWaitAtOnce()
	{
		TimerRecordingTimeProvider time = new();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Polling.Start(time, time.GetTimestamp(), 30.Seconds(), 1.Hours(), cancellation);
		Task<PollStep> step = sut.WaitForNextCheck();

		cts.Cancel();

		await That(time.LastTimer?.IsDisposed).IsTrue()
			.Because("a virtual clock that fires its timers inline must not reach the end of a wait that was canceled");
		await That(await step).IsEqualTo(PollStep.Canceled);
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheCallerCancelsAtTheEndOfTheBudget_ShouldMakeTheLastCheck()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Start(time, 50.Milliseconds(), 1.Hours(), cancellation);
		time.CancelAfter(cts, 49.Milliseconds());

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.LastCheck)
			.Because("a cancellation within the tolerance of the timers counts as the budget being used up");
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheCallerCancelsDuringALaterWait_ShouldBeCanceled()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Start(time, 30.Seconds(), 100.Milliseconds(), cancellation);
		time.CancelAfter(cts, 250.Milliseconds());

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
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		using Polling sut = Start(time, 50.Milliseconds(), 1.Hours(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.LastCheck);
		await That(sut.Elapsed).IsEqualTo(50.Milliseconds())
			.Because("the wait is shortened to the remaining budget");
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheIntervalIsNotPositive_ShouldCheckAgainWithoutWaiting()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		long start = time.GetTimestamp();
		using Polling sut = Start(time, 30.Seconds(), TimeSpan.Zero, EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Check);
		await That(time.GetElapsedTime(start)).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheIntervalIsShorterThanTheBudget_ShouldCheckAgain()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider time = timeSystem.ToTimeProvider();
		using Polling sut = Start(time, 30.Seconds(), 1.Milliseconds(), EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Check);
		await That(sut.Elapsed).IsEqualTo(1.Milliseconds());
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheTimeoutElapsedBeforeTheStart_ShouldBeCanceled()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider timeProvider = timeSystem.ToTimeProvider();
		EvaluationCancellation cancellation = new(100.Milliseconds(), CancellationToken.None, null, timeProvider);
		timeSystem.TimeProvider.AdvanceBy(100.Milliseconds());
		using Polling sut = Polling.Start(timeProvider, timeProvider.GetTimestamp(),
			100.Milliseconds(), 1.Hours(), cancellation);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled)
			.Because("the timeout left no time for the budget, so it is reported as the timeout");
		cancellation.Release();
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheTimeoutIsShorterThanTheBudget_ShouldBeCanceled()
	{
		MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
		TimeProvider timeProvider = timeSystem.ToTimeProvider();
		long start = timeProvider.GetTimestamp();
		EvaluationCancellation cancellation = new(20.Milliseconds(), CancellationToken.None, null, timeProvider);
		using Polling sut = Start(timeProvider, 30.Seconds(), 1.Hours(), cancellation);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled)
			.Because("a shorter timeout ends the checks, so that it is reported as the timeout");
		await That(timeProvider.GetElapsedTime(start)).IsEqualTo(20.Milliseconds());
		cancellation.Release();
	}

	[Test]
	public async Task WaitForNextCheck_WhenTheTimeProviderFailsAtTheCancellation_ShouldStillReleaseTheWait()
	{
		TimerRecordingTimeProvider time = new();
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Polling.Start(time, time.GetTimestamp(), 30.Seconds(), 1.Hours(), cancellation);
		Task<PollStep> step = sut.WaitForNextCheck();
		time.FailsToMeasure = true;

		void Cancel() => cts.Cancel();

		await That(Cancel).ThrowsExactly<AggregateException>()
			.WithInner<InvalidOperationException>(inner => inner.HasMessage("the clock failed"));
		await That(() => step).ThrowsExactly<InvalidOperationException>().WithMessage("the clock failed")
			.Because("the wait is released, so the failure of the time provider surfaces instead of a hang");
	}

	private static Polling Start(TimeProvider time, TimeSpan budget, TimeSpan interval,
		EvaluationCancellation cancellation)
		=> Polling.Start(time, time.GetTimestamp(), budget, interval, cancellation);

	/// <summary>
	///     A time provider whose timers never fire, so that a test sees whether a wait releases its timer, and that can
	///     fail to measure the time.
	/// </summary>
	private sealed class TimerRecordingTimeProvider : TimeProvider
	{
		public bool FailsToMeasure { get; set; }

		public RecordingTimer? LastTimer { get; private set; }

		public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
			=> LastTimer = new RecordingTimer();

		public override long GetTimestamp()
			=> FailsToMeasure ? throw new InvalidOperationException("the clock failed") : base.GetTimestamp();
	}

	private sealed class RecordingTimer : ITimer
	{
		public bool IsDisposed { get; private set; }

		public bool Change(TimeSpan dueTime, TimeSpan period) => !IsDisposed;

		public void Dispose() => IsDisposed = true;

		public ValueTask DisposeAsync()
		{
			Dispose();
			return default;
		}
	}
}
