using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class TimeProviderExtensionsTests
{
	[Test]
	public async Task CancelAfter_OnAnotherProvider_ShouldCancelTheSourceWhenTheTimerFires()
	{
		ManualTimeProvider timeProvider = new();
		using CancellationTokenSource cts = new();

		timeProvider.CancelAfter(cts, 10.Seconds());
		bool isCanceledAtOnce = cts.IsCancellationRequested;
		timeProvider.Timer!.Fire();

		await That(isCanceledAtOnce).IsFalse();
		await That(timeProvider.Timer.DueTime).IsEqualTo(10.Seconds());
		await That(timeProvider.Timer.State).IsSameAs(cts)
			.Because("a virtual clock recognizes a scheduled cancellation by its source");
		await That(cts.IsCancellationRequested).IsTrue();
		await That(timeProvider.Timer.IsDisposed).IsTrue()
			.Because("the timer is released once it canceled the source");
	}

	[Test]
	public async Task CancelAfter_OnAnotherProvider_WhenTheSourceIsCanceledBefore_ShouldReleaseTheTimer()
	{
		ManualTimeProvider timeProvider = new();
		using CancellationTokenSource cts = new();
		timeProvider.CancelAfter(cts, 10.Seconds());

		cts.Cancel();

		await That(timeProvider.Timer!.IsDisposed).IsTrue();
	}

	[Test]
	public async Task CancelAfter_OnAnotherProvider_WhenTheSourceIsDisposedBefore_ShouldNotThrowFromTheTimer()
	{
		ManualTimeProvider timeProvider = new();
		CancellationTokenSource cts = new();
		timeProvider.CancelAfter(cts, 10.Seconds());
		cts.Dispose();

		void Act() => timeProvider.Timer!.Fire();

		await That(Act).DoesNotThrow()
			.Because("an exception in a timer callback would crash the process");
	}

	[Test]
	public async Task CancelAfter_OnAnotherProvider_WithInfiniteDelay_ShouldCreateNoTimer()
	{
		ManualTimeProvider timeProvider = new();
		using CancellationTokenSource cts = new();

		timeProvider.CancelAfter(cts, Timeout.InfiniteTimeSpan);

		await That(timeProvider.Timer).IsNull();
		await That(cts.IsCancellationRequested).IsFalse();
	}

	[Test]
	public async Task CancelAfter_OnAnotherProvider_WithoutDelay_ShouldCancelAtOnce()
	{
		ManualTimeProvider timeProvider = new();
		using CancellationTokenSource cts = new();

		timeProvider.CancelAfter(cts, TimeSpan.Zero);

		await That(timeProvider.Timer).IsNull();
		await That(cts.IsCancellationRequested).IsTrue();
	}

	[Test]
	public async Task CancelAfter_OnTheSystemClock_ShouldCancelTheSourceAfterTheDelay()
	{
		TimeProvider timeProvider = TimeProvider.System;
		using CancellationTokenSource cts = new();
		TaskCompletionSource<bool> canceled = new(TaskCreationOptions.RunContinuationsAsynchronously);
		using CancellationTokenRegistration _ = cts.Token.Register(() => canceled.TrySetResult(true));

		timeProvider.CancelAfter(cts, 10.Milliseconds());
		bool isCanceledAtOnce = cts.IsCancellationRequested;
		await Task.WhenAny(canceled.Task, Task.Delay(30.Seconds()));

		await That(isCanceledAtOnce).IsFalse();
		await That(canceled.Task.IsCompleted).IsTrue()
			.Because("the real timer cancels the source after 10 ms, long before the half minute the test waits");
	}

	[Test]
	public async Task CancelAfter_OnTheSystemClock_WithInfiniteDelay_ShouldNotCancelTheSource()
	{
		TimeProvider timeProvider = TimeProvider.System;
		using CancellationTokenSource cts = new();

		timeProvider.CancelAfter(cts, Timeout.InfiniteTimeSpan);

		await That(cts.IsCancellationRequested).IsFalse();
	}

	[Test]
	public async Task CreateDelay_OnAnotherProvider_ShouldCompleteWhenTheTimerFires()
	{
		ManualTimeProvider timeProvider = new();

		Task delay = timeProvider.CreateDelay(TimeSpan.FromTicks(15_000), CancellationToken.None);
		bool isCompletedAtOnce = delay.IsCompleted;
		timeProvider.Timer!.Fire();
		await delay;

		await That(isCompletedAtOnce).IsFalse();
		await That(timeProvider.Timer.DueTime).IsEqualTo(TimeSpan.FromTicks(15_000))
			.Because("unlike Task.Delay, the delay is not rounded to whole milliseconds");
		await That(timeProvider.Timer.IsDisposed).IsTrue();
	}

	[Test]
	public async Task CreateDelay_OnAnotherProvider_WhenCanceled_ShouldBeCanceledAndReleaseTheTimer()
	{
		ManualTimeProvider timeProvider = new();
		using CancellationTokenSource cts = new();
		Task delay = timeProvider.CreateDelay(10.Seconds(), cts.Token);

		cts.Cancel();
		await Task.WhenAny(delay);

		await That(delay.IsCanceled).IsTrue();
		await That(timeProvider.Timer!.IsDisposed).IsTrue();
	}

	[Test]
	public async Task CreateDelay_OnAnotherProvider_WithInfiniteDelay_ShouldCreateATimer()
	{
		ManualTimeProvider timeProvider = new();
		using CancellationTokenSource cts = new();

		Task delay = timeProvider.CreateDelay(Timeout.InfiniteTimeSpan, cts.Token);
		cts.Cancel();
		await Task.WhenAny(delay);

		await That(timeProvider.Timer!.DueTime).IsEqualTo(Timeout.InfiniteTimeSpan)
			.Because("a virtual clock sees a wait without a limit as well");
		await That(delay.IsCanceled).IsTrue();
	}

	[Test]
	public async Task CreateDelay_OnAnotherProvider_WithoutDelay_ShouldCompleteAtOnce()
	{
		ManualTimeProvider timeProvider = new();

		Task delay = timeProvider.CreateDelay(TimeSpan.Zero, CancellationToken.None);

		await That(delay.IsCompleted).IsTrue();
		await That(timeProvider.Timer).IsNull();
	}

	[Test]
	public async Task CreateDelay_OnTheSystemClock_ShouldUseRealValues()
	{
		TimeProvider timeProvider = TimeProvider.System;
		long timestamp = timeProvider.GetTimestamp();

		await timeProvider.CreateDelay(20.Milliseconds(), CancellationToken.None);

		await That(timeProvider.GetElapsedTime(timestamp)).IsGreaterThan(10.Milliseconds());
	}

	[Test]
	public async Task CreateDelay_OnTheSystemClock_WhenCanceled_ShouldBeCanceled()
	{
		TimeProvider timeProvider = TimeProvider.System;
		using CancellationTokenSource cts = new();
		cts.Cancel();

		Task Act() => timeProvider.CreateDelay(30.Seconds(), cts.Token);

		await That(Act).Throws<OperationCanceledException>();
	}

	private sealed class ManualTimeProvider : TimeProvider
	{
		public ManualTimer? Timer { get; private set; }

		public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
		{
			Timer = new ManualTimer(callback, state, dueTime);
			return Timer;
		}
	}

	private sealed class ManualTimer(TimerCallback callback, object? state, TimeSpan dueTime) : ITimer
	{
		public TimeSpan DueTime { get; } = dueTime;

		public bool IsDisposed { get; private set; }

		public object? State { get; } = state;

		public bool Change(TimeSpan dueTime, TimeSpan period) => false;

		public void Dispose() => IsDisposed = true;

		public ValueTask DisposeAsync()
		{
			Dispose();
			return default;
		}

		public void Fire() => callback(State);
	}
}
