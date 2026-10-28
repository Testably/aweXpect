using System.Collections.Generic;
using System.Threading;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     A time provider with a virtual clock for the waits and the timeouts of an evaluation (<c>Eventually()</c>,
///     <c>Within(…)</c>, <c>WithTimeout(…)</c>), so that a test neither waits nor depends on the speed of the machine.
/// </summary>
/// <remarks>
///     The clock only moves through the waits of the evaluation, each of which advances it by exactly its duration
///     and completes at once, and through <see cref="Advance(TimeSpan)" />, with which e.g. a subject states how long
///     its attempt took. As a wait is the only thing that moves the clock on its own, a repeated check with a limited
///     budget and a positive interval always ends.
///     <para />
///     The timeouts that cancel are scheduled on the virtual clock as well: the timeout of the evaluation
///     (<c>WithTimeout(…)</c> and the test cancellation) and the limit of an attempt and of a check of
///     <c>Eventually()</c>. They cancel when a wait or <see cref="Advance(TimeSpan)" /> moves the clock to their
///     time, so code that waits for a cancellation without moving the clock, e.g. a subject that never completes,
///     has to call <see cref="Advance(TimeSpan)" /> first. A <see cref="CancellationTokenSource" /> with a delay of
///     its own cancels after real time, unless it is created with this provider; a cancellation by the caller is
///     scheduled with <see cref="CancelAt(TimeSpan, CancellationTokenSource)" />.
///     <para />
///     Waits and timeouts reach the provider as timers from <see cref="CreateTimer" />, e.g. from the helpers of the
///     evaluation, <c>Task.Delay(TimeSpan, TimeProvider, CancellationToken)</c> or
///     <c>new CancellationTokenSource(TimeSpan, TimeProvider)</c>. A timer whose state is a
///     <see cref="CancellationTokenSource" /> is a scheduled cancellation, every other timer is a wait. Only the due
///     time of a timer is used, as no caller sets a period.
/// </remarks>
internal sealed class VirtualTimeProvider : TimeProvider
{
	/// <summary>
	///     How long a wait that a cancellation cut short waits in real time for being canceled itself, so that a
	///     regression which ignores the cancellation fails the test instead of hanging the test run.
	/// </summary>
	private static readonly TimeSpan SafetyNet = TimeSpan.FromSeconds(30);

	private readonly List<ScheduledCancellation> _cancellations = [];
	private readonly object _lock = new();
	private TimeSpan _now;

	/// <summary>
	///     The virtual time since the creation of the time provider.
	/// </summary>
	public TimeSpan Now
	{
		get
		{
			lock (_lock)
			{
				return _now;
			}
		}
	}

	/// <inheritdoc />
	public override long TimestampFrequency => TimeSpan.TicksPerSecond;

	/// <inheritdoc />
	public override long GetTimestamp() => Now.Ticks;

	/// <inheritdoc />
	/// <remarks>
	///     A scheduled cancellation cancels its source when the clock reaches the end of the
	///     <paramref name="dueTime" />, like with <see cref="CancelAt(TimeSpan, CancellationTokenSource)" />, and never for
	///     <see cref="Timeout.InfiniteTimeSpan" />.
	///     <para />
	///     A wait completes on the thread pool, like the timer of a real wait. A cancellation scheduled within the
	///     <paramref name="dueTime" /> cuts the wait short: the clock stops at the time of the cancellation and the
	///     wait does not complete, but is canceled by its own token, as the repeated check does when it notices the
	///     cancellation. Completing it would race with that notice, which is delivered asynchronously. A wait without a
	///     limit (<see cref="Timeout.InfiniteTimeSpan" />) moves the clock to the next scheduled cancellation, as only a
	///     cancellation ends it.
	/// </remarks>
	public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
	{
		if (state is CancellationTokenSource source)
		{
			ScheduledCancellation cancellation = new(this, callback, source);
			if (dueTime != Timeout.InfiniteTimeSpan)
			{
				Schedule(Now + dueTime, cancellation);
			}

			return cancellation;
		}

		Wait wait = new(callback, state);
		_ = Task.Run(() => RunWait(wait, dueTime));
		return wait;
	}

	/// <summary>
	///     Advances the clock by the <paramref name="duration" />, e.g. from inside a subject for the time its attempt
	///     takes. The cancellations scheduled within the <paramref name="duration" /> are made on the way.
	/// </summary>
	public void Advance(TimeSpan duration) => Advance(duration, false);

	/// <summary>
	///     Creates a task that completes after the <paramref name="delay" /> on the virtual clock, like
	///     <c>Task.Delay</c> with a <see cref="TimeProvider" />, e.g. for a subject that waits.
	/// </summary>
	public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
#if NET8_0_OR_GREATER
		=> Task.Delay(delay, this, cancellationToken);
#else
		=> TimeProviderTaskExtensions.Delay(this, delay, cancellationToken);
#endif

	/// <summary>
	///     Cancels the <paramref name="source" /> when the clock reaches the <paramref name="time" />, e.g. during a
	///     wait between two checks.
	/// </summary>
	public void CancelAt(TimeSpan time, CancellationTokenSource source)
		=> Schedule(time, new ScheduledCancellation(this, s => ((CancellationTokenSource)s!).Cancel(), source));

	private async Task RunWait(Wait wait, TimeSpan dueTime)
	{
		if (wait.IsDisposed)
		{
			return;
		}

		if (IsCutShort(dueTime))
		{
			try
			{
				await Task.Delay(SafetyNet, wait.DisposedToken);
			}
			catch (OperationCanceledException)
			{
				return;
			}
		}

		wait.Complete();
	}

	private void Schedule(TimeSpan time, ScheduledCancellation cancellation)
	{
		lock (_lock)
		{
			cancellation.Time = time;
			_cancellations.Add(cancellation);
		}
	}

	private void Unschedule(ScheduledCancellation cancellation)
	{
		lock (_lock)
		{
			_cancellations.Remove(cancellation);
		}
	}

	/// <summary>
	///     Advances the clock by the <paramref name="duration" /> and returns whether a cancellation cut it short,
	///     which only happens to a wait.
	/// </summary>
	private bool Advance(TimeSpan duration, bool isWait)
	{
		TimeSpan end = Now + duration;
		while (TakeNextCancellation(end) is { } cancellation)
		{
			if (cancellation.TryCancel() && isWait)
			{
				return true;
			}
		}

		MoveTo(end);
		return false;
	}

	/// <summary>
	///     Advances the clock by the <paramref name="delay" /> of a wait and returns whether the wait does not
	///     complete, because a cancellation cut it short or it has no limit.
	/// </summary>
	private bool IsCutShort(TimeSpan delay)
	{
		if (delay != Timeout.InfiniteTimeSpan)
		{
			return Advance(delay, true);
		}

		bool isCanceled = false;
		while (!isCanceled && TakeNextCancellation(TimeSpan.MaxValue) is { } cancellation)
		{
			isCanceled = cancellation.TryCancel();
		}

		return true;
	}

	/// <summary>
	///     Moves the clock to the earliest cancellation scheduled until the <paramref name="end" /> and returns it, or
	///     returns <see langword="null" /> when there is none.
	/// </summary>
	private ScheduledCancellation? TakeNextCancellation(TimeSpan end)
	{
		lock (_lock)
		{
			ScheduledCancellation? next = null;
			foreach (ScheduledCancellation cancellation in _cancellations)
			{
				if (cancellation.Time <= end && (next is null || cancellation.Time < next.Time))
				{
					next = cancellation;
				}
			}

			if (next is null)
			{
				return null;
			}

			_cancellations.Remove(next);
			MoveTo(next.Time);
			return next;
		}
	}

	private void MoveTo(TimeSpan time)
	{
		lock (_lock)
		{
			if (time > _now)
			{
				_now = time;
			}
		}
	}

	private sealed class ScheduledCancellation(
		VirtualTimeProvider timeProvider,
		TimerCallback callback,
		CancellationTokenSource source) : ITimer
	{
		public TimeSpan Time { get; set; }

		public bool Change(TimeSpan dueTime, TimeSpan period)
			=> throw new NotSupportedException("The virtual clock does not reschedule a cancellation.");

		public void Dispose() => timeProvider.Unschedule(this);

		public ValueTask DisposeAsync()
		{
			Dispose();
			return default;
		}

		/// <summary>
		///     Cancels the source, unless it is already disposed: its timeout was released, like the timer of a real
		///     one.
		/// </summary>
		public bool TryCancel()
		{
			try
			{
				_ = source.Token;
				callback(source);
				return true;
			}
			catch (ObjectDisposedException)
			{
				return false;
			}
		}
	}

	private sealed class Wait(TimerCallback callback, object? state) : ITimer
	{
		private readonly CancellationTokenSource _disposed = new();

		public CancellationToken DisposedToken => _disposed.Token;

		public bool IsDisposed => _disposed.IsCancellationRequested;

		public bool Change(TimeSpan dueTime, TimeSpan period)
			=> throw new NotSupportedException("The virtual clock does not reschedule a wait.");

		public void Dispose() => _disposed.Cancel();

		public ValueTask DisposeAsync()
		{
			Dispose();
			return default;
		}

		public void Complete()
		{
			if (!IsDisposed)
			{
				callback(state);
			}
		}
	}
}
