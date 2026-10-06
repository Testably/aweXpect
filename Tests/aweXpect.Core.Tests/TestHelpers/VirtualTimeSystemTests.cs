using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Tests.TestHelpers;

public sealed class VirtualTimeSystemTests
{
	[Fact]
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

	[Fact]
	public async Task Delay_ShouldAdvanceTheClockByItsDuration()
	{
		VirtualTimeSystem sut = new();
		sut.Advance(10.Milliseconds());
		long timestamp = sut.GetTimestamp();

		await sut.Delay(1.Hours(), CancellationToken.None);

		await That(sut.GetElapsedTime(timestamp)).IsEqualTo(1.Hours());
		await That(sut.Now).IsEqualTo(1.Hours() + 10.Milliseconds());
	}

	[Fact]
	public async Task Delay_WhenACancellationIsScheduledAfterIt_ShouldNotCancel()
	{
		VirtualTimeSystem sut = new();
		using CancellationTokenSource cts = new();
		sut.CancelAt(101.Milliseconds(), cts);

		await sut.Delay(100.Milliseconds(), CancellationToken.None);

		await That(cts.IsCancellationRequested).IsFalse();
		await That(sut.Now).IsEqualTo(100.Milliseconds());
	}

	[Fact]
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

	[Fact]
	public async Task Delay_WhenAlreadyCanceled_ShouldNotAdvanceTheClock()
	{
		VirtualTimeSystem sut = new();
		using CancellationTokenSource waitCts = new();
		waitCts.Cancel();

		Task Act() => sut.Delay(100.Milliseconds(), waitCts.Token);

		await That(Act).Throws<OperationCanceledException>();
		await That(sut.Now).IsEqualTo(TimeSpan.Zero);
	}

	[Fact]
	public async Task Stopwatch_ShouldMeasureTheVirtualTimeWhileItIsRunning()
	{
		VirtualTimeSystem sut = new();
		IStopwatch stopwatch = sut.Stopwatch.New();
		sut.Advance(10.Milliseconds());
		stopwatch.Start();
		sut.Advance(20.Milliseconds());
		TimeSpan elapsedWhileRunning = stopwatch.Elapsed;
		stopwatch.Stop();
		sut.Advance(40.Milliseconds());

		await That(elapsedWhileRunning).IsEqualTo(20.Milliseconds());
		await That(stopwatch.Elapsed).IsEqualTo(20.Milliseconds());
		await That(stopwatch.IsRunning).IsFalse();
	}
}
