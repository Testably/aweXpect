using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Tests.Core.TimeSystem;

public sealed class RealTimeSystemTests
{
	[Test]
	public async Task CancelAfter_ShouldCancelTheSourceAfterTheDelay()
	{
		ITimeSystem timeSystem = RealTimeSystem.Instance;
		using CancellationTokenSource cts = new();
		TaskCompletionSource<bool> canceled = new(TaskCreationOptions.RunContinuationsAsynchronously);
		using CancellationTokenRegistration _ = cts.Token.Register(() => canceled.TrySetResult(true));

		timeSystem.CancelAfter(cts, 10.Milliseconds());
		bool isCanceledAtOnce = cts.IsCancellationRequested;
		await Task.WhenAny(canceled.Task, Task.Delay(30.Seconds()));

		await That(isCanceledAtOnce).IsFalse();
		await That(canceled.Task.IsCompleted).IsTrue()
			.Because("the real timer cancels the source after 10 ms, long before the half minute the test waits");
	}

	[Test]
	public async Task CancelAfter_WithInfiniteDelay_ShouldNotCancelTheSource()
	{
		ITimeSystem timeSystem = RealTimeSystem.Instance;
		using CancellationTokenSource cts = new();

		timeSystem.CancelAfter(cts, Timeout.InfiniteTimeSpan);

		await That(cts.IsCancellationRequested).IsFalse();
	}

	[Test]
	public async Task Delay_ShouldUseRealValues()
	{
		ITimeSystem timeSystem = RealTimeSystem.Instance;
		long timestamp = timeSystem.GetTimestamp();

		await timeSystem.Delay(20.Milliseconds(), CancellationToken.None);

		await That(timeSystem.GetElapsedTime(timestamp)).IsGreaterThan(10.Milliseconds());
	}

	[Test]
	public async Task Delay_WhenCanceled_ShouldBeCanceled()
	{
		ITimeSystem timeSystem = RealTimeSystem.Instance;
		using CancellationTokenSource cts = new();
		cts.Cancel();

		Task Act() => timeSystem.Delay(30.Seconds(), cts.Token);

		await That(Act).Throws<OperationCanceledException>();
	}

	[Test]
	public async Task Stopwatch_New_ShouldReturnDifferentStopwatches()
	{
		ITimeSystem timeSystem = RealTimeSystem.Instance;
		IStopwatch stopwatch1 = timeSystem.Stopwatch.New();
		IStopwatch stopwatch2 = timeSystem.Stopwatch.New();

		stopwatch1.Start();

		await That(stopwatch1.IsRunning).IsTrue();
		await That(stopwatch2.IsRunning).IsFalse();
	}

	[Test]
	public async Task Stopwatch_ShouldUseRealValues()
	{
		ITimeSystem timeSystem = RealTimeSystem.Instance;
		IStopwatch stopwatch = timeSystem.Stopwatch.New();

		await That(stopwatch.IsRunning).IsFalse();

		stopwatch.Start();

		await That(stopwatch.IsRunning).IsTrue();

		await Task.Delay(20.Milliseconds());
		stopwatch.Stop();

		await That(stopwatch.Elapsed).IsGreaterThan(10.Milliseconds());
	}
}
