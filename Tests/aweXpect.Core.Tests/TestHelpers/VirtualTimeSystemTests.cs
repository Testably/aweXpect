using System.Threading;
using aweXpect.Chronology;

namespace aweXpect.Core.Tests.TestHelpers;

public sealed class VirtualTimeSystemTests
{
	[Test]
	public async Task Advance_ShouldMakeTheScheduledCancellationsOnTheWay()
	{
		VirtualTimeSystem sut = new();
		using CancellationTokenSource cts = new();
		TimeSpan? canceledAt = null;
		using CancellationTokenRegistration _ = cts.Token.Register(() => canceledAt = sut.Now);
		sut.CancelAt(40.Milliseconds(), cts);

		sut.Advance(100.Milliseconds());

		await That(canceledAt).IsEqualTo(40.Milliseconds());
		await That(sut.Now).IsEqualTo(100.Milliseconds());
	}

	[Test]
	public async Task CancelAfter_ShouldCancelWhenTheClockReachesTheEndOfTheDelay()
	{
		VirtualTimeSystem sut = new();
		using CancellationTokenSource cts = new();
		sut.Advance(10.Milliseconds());

		sut.CancelAfter(cts, 40.Milliseconds());
		sut.Advance(39.Milliseconds());
		bool isCanceledBeforeTheEnd = cts.IsCancellationRequested;
		sut.Advance(1.Milliseconds());

		await That(isCanceledBeforeTheEnd).IsFalse();
		await That(cts.IsCancellationRequested).IsTrue();
	}

	[Test]
	public async Task CancelAfter_WhenTheSourceIsDisposed_ShouldNotCutAWaitShort()
	{
		VirtualTimeSystem sut = new();
		CancellationTokenSource cts = new();
		sut.CancelAfter(cts, 40.Milliseconds());
		cts.Dispose();

		await sut.Delay(100.Milliseconds(), CancellationToken.None);

		await That(sut.Now).IsEqualTo(100.Milliseconds())
			.Because("the timeout of a disposed source was released, like the timer of a real one");
	}

	[Test]
	public async Task CancelAfter_WithInfiniteDelay_ShouldNotCancel()
	{
		VirtualTimeSystem sut = new();
		using CancellationTokenSource cts = new();

		sut.CancelAfter(cts, Timeout.InfiniteTimeSpan);
		sut.Advance(100.Days());

		await That(cts.IsCancellationRequested).IsFalse();
	}

	[Test]
	public async Task CancelAfter_WithoutDelay_ShouldCancelAtOnce()
	{
		VirtualTimeSystem sut = new();
		using CancellationTokenSource cts = new();

		sut.CancelAfter(cts, TimeSpan.Zero);

		await That(cts.IsCancellationRequested).IsTrue();
		await That(sut.Now).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task Delay_ShouldAdvanceTheClockByItsDuration()
	{
		VirtualTimeSystem sut = new();
		sut.Advance(10.Milliseconds());
		long timestamp = sut.GetTimestamp();

		await sut.Delay(1.Hours(), CancellationToken.None);

		await That(sut.GetElapsedTime(timestamp)).IsEqualTo(1.Hours());
		await That(sut.Now).IsEqualTo(1.Hours() + 10.Milliseconds());
	}

	[Test]
	public async Task Delay_WhenACancellationIsScheduledAfterIt_ShouldNotCancel()
	{
		VirtualTimeSystem sut = new();
		using CancellationTokenSource cts = new();
		sut.CancelAt(101.Milliseconds(), cts);

		await sut.Delay(100.Milliseconds(), CancellationToken.None);

		await That(cts.IsCancellationRequested).IsFalse();
		await That(sut.Now).IsEqualTo(100.Milliseconds());
	}

	[Test]
	public async Task Delay_WhenACancellationIsScheduledWithinIt_ShouldStopTheClockUntilTheWaitIsCanceled()
	{
		VirtualTimeSystem sut = new();
		using CancellationTokenSource cts = new();
		using CancellationTokenSource waitCts = new();
		TaskCompletionSource<bool> canceled = new(TaskCreationOptions.RunContinuationsAsynchronously);
		using CancellationTokenRegistration _ = cts.Token.Register(() => canceled.TrySetResult(true));
		sut.CancelAt(40.Milliseconds(), cts);

		Task delay = sut.Delay(100.Milliseconds(), waitCts.Token);
		await canceled.Task;

		await That(sut.Now).IsEqualTo(40.Milliseconds());
		await That(delay.IsCompleted).IsFalse()
			.Because("the wait was cut short, so only its own cancellation ends it");
		waitCts.Cancel();
		await That(() => delay).Throws<OperationCanceledException>();
		await That(sut.Now).IsEqualTo(40.Milliseconds());
	}

	[Test]
	public async Task Delay_WhenAlreadyCanceled_ShouldNotAdvanceTheClock()
	{
		VirtualTimeSystem sut = new();
		using CancellationTokenSource waitCts = new();
		waitCts.Cancel();

		Task Act() => sut.Delay(100.Milliseconds(), waitCts.Token);

		await That(Act).Throws<OperationCanceledException>();
		await That(sut.Now).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task Delay_WithoutLimit_ShouldMoveTheClockToTheNextCancellationAndWaitForItsOwn()
	{
		VirtualTimeSystem sut = new();
		using CancellationTokenSource cts = new();
		sut.CancelAfter(cts, 40.Milliseconds());

		Task delay = sut.Delay(Timeout.InfiniteTimeSpan, cts.Token);

		await That(() => delay).Throws<OperationCanceledException>()
			.Because("only a cancellation ends a wait without a limit");
		await That(sut.Now).IsEqualTo(40.Milliseconds());
	}

	[Test]
	public async Task GetElapsedTime_ShouldMeasureTheVirtualTimeSinceTheTimestamp()
	{
		VirtualTimeSystem sut = new();
		sut.Advance(10.Milliseconds());
		long timestamp = sut.GetTimestamp();

		sut.Advance(20.Milliseconds());

		await That(sut.GetElapsedTime(timestamp)).IsEqualTo(20.Milliseconds());
	}
}
