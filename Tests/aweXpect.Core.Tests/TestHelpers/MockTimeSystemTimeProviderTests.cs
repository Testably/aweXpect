using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     The tests run on the <see cref="TimeProvider" /> of a <see cref="MockTimeSystem" />, so these tests pin the
///     behavior of the virtual clock they rely on.
/// </summary>
public sealed class MockTimeSystemTimeProviderTests
{
	[Test]
	public async Task AdvanceBy_ShouldMakeTheScheduledCancellationsOnTheWay()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		long start = sut.GetTimestamp();
		using CancellationTokenSource cts = new();
		TimeSpan? canceledAt = null;
		using CancellationTokenRegistration _ = cts.Token.Register(() => canceledAt = sut.GetElapsedTime(start));
		sut.CancelAfter(cts, 40.Milliseconds());

		timeSystem.TimeProvider.AdvanceBy(100.Milliseconds());

		await That(canceledAt).IsEqualTo(40.Milliseconds());
		await That(sut.GetElapsedTime(start)).IsEqualTo(100.Milliseconds());
	}

	[Test]
	public async Task CancelAfter_ShouldCancelWhenTheClockReachesTheEndOfTheDelay()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		using CancellationTokenSource cts = new();
		timeSystem.TimeProvider.AdvanceBy(10.Milliseconds());

		sut.CancelAfter(cts, 40.Milliseconds());
		timeSystem.TimeProvider.AdvanceBy(39.Milliseconds());
		bool isCanceledBeforeTheEnd = cts.IsCancellationRequested;
		timeSystem.TimeProvider.AdvanceBy(1.Milliseconds());

		await That(isCanceledBeforeTheEnd).IsFalse();
		await That(cts.IsCancellationRequested).IsTrue();
	}

	[Test]
	public async Task CancelAfter_ShouldNotMoveTheClock()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		long start = sut.GetTimestamp();
		using CancellationTokenSource cts = new();

		sut.CancelAfter(cts, 40.Milliseconds());
		await Task.Delay(50.Milliseconds());

		await That(cts.IsCancellationRequested).IsFalse()
			.Because("a timeout only fires when something else moves the clock");
		await That(sut.GetElapsedTime(start)).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task CancelAfter_WhenTheSourceIsDisposed_ShouldNotCutAWaitShort()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		long start = sut.GetTimestamp();
		CancellationTokenSource cts = new();
		sut.CancelAfter(cts, 40.Milliseconds());
		cts.Dispose();

		await sut.CreateDelay(100.Milliseconds(), CancellationToken.None);

		await That(sut.GetElapsedTime(start)).IsEqualTo(100.Milliseconds())
			.Because("the timeout of a disposed source was released, like the timer of a real one");
	}

	[Test]
	public async Task CancelAfter_WithInfiniteDelay_ShouldNotCancel()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		using CancellationTokenSource cts = new();

		sut.CancelAfter(cts, Timeout.InfiniteTimeSpan);
		timeSystem.TimeProvider.AdvanceBy(100.Days());

		await That(cts.IsCancellationRequested).IsFalse();
	}

	[Test]
	public async Task CancelAfter_WithoutDelay_ShouldCancelAtOnce()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		long start = sut.GetTimestamp();
		using CancellationTokenSource cts = new();

		sut.CancelAfter(cts, TimeSpan.Zero);

		await That(cts.IsCancellationRequested).IsTrue();
		await That(sut.GetElapsedTime(start)).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task CreateDelay_ShouldAdvanceTheClockByItsDuration()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		timeSystem.TimeProvider.AdvanceBy(10.Milliseconds());
		long timestamp = sut.GetTimestamp();

		await sut.CreateDelay(1.Hours(), CancellationToken.None);

		await That(sut.GetElapsedTime(timestamp)).IsEqualTo(1.Hours());
	}

	[Test]
	public async Task CreateDelay_WhenACancellationIsScheduledAfterIt_ShouldNotCancel()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		long start = sut.GetTimestamp();
		using CancellationTokenSource cts = new();
		sut.CancelAfter(cts, 101.Milliseconds());

		await sut.CreateDelay(100.Milliseconds(), CancellationToken.None);

		await That(cts.IsCancellationRequested).IsFalse();
		await That(sut.GetElapsedTime(start)).IsEqualTo(100.Milliseconds());
	}

	[Test]
	public async Task CreateDelay_WhenACancellationIsScheduledWithinIt_ShouldCancelOnTheWay()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		long start = sut.GetTimestamp();
		using CancellationTokenSource cts = new();
		TimeSpan? canceledAt = null;
		using CancellationTokenRegistration _ = cts.Token.Register(() => canceledAt = sut.GetElapsedTime(start));
		sut.CancelAfter(cts, 40.Milliseconds());

		await That(() => sut.CreateDelay(100.Milliseconds(), cts.Token)).Throws<OperationCanceledException>()
			.WithMessage(new TaskCanceledException().Message);
		await That(canceledAt).IsEqualTo(40.Milliseconds());
	}

	[Test]
	public async Task CreateDelay_WhenAlreadyCanceled_ShouldNotAdvanceTheClock()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		long start = sut.GetTimestamp();
		using CancellationTokenSource waitCts = new();
		waitCts.Cancel();

		Task Act() => sut.CreateDelay(100.Milliseconds(), waitCts.Token);

		await That(Act).Throws<OperationCanceledException>().WithMessage(new TaskCanceledException().Message);
		await That(sut.GetElapsedTime(start)).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task Delay_ShouldAdvanceTheClockByItsDuration()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		long start = sut.GetTimestamp();

#if NET8_0_OR_GREATER
		await Task.Delay(1.Hours(), sut, CancellationToken.None);
#else
		await TimeProviderTaskExtensions.Delay(sut, 1.Hours(), CancellationToken.None);
#endif

		await That(sut.GetElapsedTime(start)).IsEqualTo(1.Hours())
			.Because("the standard delay of a time provider is a wait on the virtual clock as well");
	}

	[Test]
	public async Task GetElapsedTime_ShouldMeasureTheVirtualTimeSinceTheTimestamp()
	{
		MockTimeSystem timeSystem = new();
		TimeProvider sut = timeSystem.ToTimeProvider();
		timeSystem.TimeProvider.AdvanceBy(10.Milliseconds());
		long timestamp = sut.GetTimestamp();

		timeSystem.TimeProvider.AdvanceBy(20.Milliseconds());

		await That(sut.GetElapsedTime(timestamp)).IsEqualTo(20.Milliseconds());
	}
}
