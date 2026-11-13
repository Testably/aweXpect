using System;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Core.Helpers;

/// <summary>
///     The waits and timeouts of an evaluation on its <see cref="TimeProvider" />.
/// </summary>
/// <remarks>
///     <see cref="TimeProvider.System" /> uses <see cref="Task.Delay(TimeSpan, CancellationToken)" /> and
///     <see cref="CancellationTokenSource.CancelAfter(TimeSpan)" /> directly, without a timer or allocation of its own.
///     Any other provider gets a timer from <see cref="TimeProvider.CreateTimer" /> with the exact delay, also on
///     <c>netstandard2.0</c> and for <see cref="Timeout.InfiniteTimeSpan" />, so that a virtual clock sees every wait.
/// </remarks>
internal static class TimeProviderExtensions
{
	private static readonly TimerCallback CancelCallback = static state =>
	{
		try
		{
			((CancellationTokenSource)state!).Cancel();
		}
		catch (ObjectDisposedException)
		{
			// The source was released before its timeout elapsed, like the timer of a real one.
		}
	};

	private static readonly Action<object?> DisposeTimerCallback = static state => ((ITimer)state!).Dispose();

	/// <summary>
	///     Creates a task that completes after the <paramref name="delay" /> on the <paramref name="timeProvider" />,
	///     or is canceled with the <paramref name="cancellationToken" />.
	/// </summary>
	/// <remarks>
	///     Like <see cref="Task.Delay(TimeSpan, CancellationToken)" />; a delay of
	///     <see cref="Timeout.InfiniteTimeSpan" /> only ends with the <paramref name="cancellationToken" />.
	/// </remarks>
	public static Task CreateDelay(this TimeProvider timeProvider, TimeSpan delay, CancellationToken cancellationToken)
	{
		if (ReferenceEquals(timeProvider, TimeProvider.System))
		{
			return Task.Delay(delay, cancellationToken);
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled(cancellationToken);
		}

		return delay == TimeSpan.Zero
			? Task.CompletedTask
			: new DelayPromise(timeProvider, delay, cancellationToken).Task;
	}

	/// <summary>
	///     Cancels the <paramref name="cancellationTokenSource" /> after the <paramref name="delay" /> on the
	///     <paramref name="timeProvider" />.
	/// </summary>
	/// <remarks>
	///     Like with <see cref="CancellationTokenSource.CancelAfter(TimeSpan)" />,
	///     <see cref="Timeout.InfiniteTimeSpan" /> never cancels and a source that is disposed before is not canceled.
	///     On another provider than <see cref="TimeProvider.System" />, a delay of zero or less cancels at once, and the
	///     timer gets the source as its state and is released when the source is canceled.
	/// </remarks>
	public static void CancelAfter(this TimeProvider timeProvider, CancellationTokenSource cancellationTokenSource,
		TimeSpan delay)
	{
		if (ReferenceEquals(timeProvider, TimeProvider.System))
		{
			cancellationTokenSource.CancelAfter(delay);
			return;
		}

		if (delay == Timeout.InfiniteTimeSpan)
		{
			return;
		}

		if (delay <= TimeSpan.Zero)
		{
			cancellationTokenSource.Cancel();
			return;
		}

		ITimer timer = timeProvider.CreateTimer(CancelCallback, cancellationTokenSource, delay,
			Timeout.InfiniteTimeSpan);
		// The registration keeps the timer referenced as long as the source, and runs at once when the timer already
		// canceled it.
		cancellationTokenSource.Token.Register(DisposeTimerCallback, timer);
	}

	private sealed class DelayPromise : TaskCompletionSource<bool>
	{
		private readonly CancellationToken _cancellationToken;
		private readonly CancellationTokenRegistration _registration;
		private readonly ITimer? _timer;

		/// <remarks>
		///     The timer is created stopped and started only once it is stored, so that a cancellation always releases
		///     it: a virtual clock that moves on its own would otherwise move past a timeout that cancels the token to
		///     a timer that is not stored yet.
		/// </remarks>
		public DelayPromise(TimeProvider timeProvider, TimeSpan delay, CancellationToken cancellationToken)
			: base(TaskCreationOptions.RunContinuationsAsynchronously)
		{
			_cancellationToken = cancellationToken;
			_registration = cancellationToken.Register(static state => ((DelayPromise)state!).Cancel(), this);
			ITimer timer = timeProvider.CreateTimer(static state => ((DelayPromise)state!).Complete(), this,
				Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			// A full fence, so that either the cancellation sees the stored timer or this sees the canceled task.
			Interlocked.Exchange(ref _timer, timer);
			if (Task.IsCompleted)
			{
				Release();
				return;
			}

			try
			{
				timer.Change(delay, Timeout.InfiniteTimeSpan);
			}
			catch (ObjectDisposedException)
			{
				// The cancellation released the timer in the meantime.
			}
		}

		private void Complete()
		{
			if (TrySetResult(true))
			{
				Release();
			}
		}

		private void Cancel()
		{
			if (TrySetCanceled(_cancellationToken))
			{
				Release();
			}
		}

		private void Release()
		{
			_registration.Dispose();
			_timer?.Dispose();
		}
	}
}
