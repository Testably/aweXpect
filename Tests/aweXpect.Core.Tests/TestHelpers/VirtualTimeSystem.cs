using System.Collections.Generic;
using System.Linq;
using System.Threading;
using aweXpect.Core.Internal;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     A time system with a virtual clock for the waits and the timeouts of an evaluation (<c>Eventually()</c>,
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
///     its own cancels after real time; a cancellation by the caller is scheduled with
///     <see cref="CancelAt(TimeSpan, CancellationTokenSource)" />.
/// </remarks>
internal sealed class VirtualTimeSystem : ITimeSystem
{
	/// <summary>
	///     How long a wait that a cancellation cut short waits in real time for being canceled itself, so that a
	///     regression which ignores the cancellation fails the test instead of hanging the test run.
	/// </summary>
	private static readonly TimeSpan SafetyNet = TimeSpan.FromSeconds(30);

	private readonly List<(TimeSpan Time, CancellationTokenSource Source)> _cancellations = [];
	private readonly object _lock = new();
	private TimeSpan _now;

	/// <summary>
	///     The virtual time since the creation of the time system.
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
	public long GetTimestamp() => Now.Ticks;

	/// <inheritdoc />
	public TimeSpan GetElapsedTime(long startTimestamp) => TimeSpan.FromTicks(Now.Ticks - startTimestamp);

	/// <inheritdoc />
	/// <remarks>
	///     The wait completes on the thread pool, like the timer of a real wait. A cancellation that
	///     <see cref="CancelAt(TimeSpan, CancellationTokenSource)" /> scheduled within the <paramref name="delay" /> cuts
	///     the wait short: the clock stops at the time of the cancellation and the wait does not complete, but is
	///     canceled by the <paramref name="cancellationToken" />, as the repeated check does when it notices the
	///     cancellation. Completing it would race with that notice, which is delivered asynchronously.
	///     <para />
	///     A wait without a limit (<see cref="Timeout.InfiniteTimeSpan" />) moves the clock to the next scheduled
	///     cancellation, as only a cancellation ends it.
	/// </remarks>
	public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
		=> Task.Run(() => IsCutShort(delay)
			? Task.Delay(SafetyNet, cancellationToken)
			: Task.CompletedTask, cancellationToken);

	/// <inheritdoc />
	/// <remarks>
	///     The <paramref name="cancellationTokenSource" /> is canceled when the clock reaches the end of the
	///     <paramref name="delay" />, like with <see cref="CancelAt(TimeSpan, CancellationTokenSource)" />.
	/// </remarks>
	public void CancelAfter(CancellationTokenSource cancellationTokenSource, TimeSpan delay)
	{
		if (delay == Timeout.InfiniteTimeSpan)
		{
			return;
		}

		if (delay <= TimeSpan.Zero)
		{
			cancellationTokenSource.Cancel();
			return;
		}

		CancelAt(Now + delay, cancellationTokenSource);
	}

	/// <summary>
	///     Advances the clock by the <paramref name="duration" />, e.g. from inside a subject for the time its attempt
	///     takes. The cancellations scheduled within the <paramref name="duration" /> are made on the way.
	/// </summary>
	public void Advance(TimeSpan duration) => Advance(duration, false);

	/// <summary>
	///     Cancels the <paramref name="source" /> when the clock reaches the <paramref name="time" />, e.g. during a
	///     wait between two checks.
	/// </summary>
	public void CancelAt(TimeSpan time, CancellationTokenSource source)
	{
		lock (_lock)
		{
			_cancellations.Add((time, source));
		}
	}

	/// <summary>
	///     Advances the clock by the <paramref name="duration" /> and returns whether a cancellation cut it short,
	///     which only happens to a wait.
	/// </summary>
	private bool Advance(TimeSpan duration, bool isWait)
	{
		TimeSpan end = Now + duration;
		while (TakeNextCancellation(end) is { } source)
		{
			if (TryCancel(source) && isWait)
			{
				return true;
			}
		}

		MoveTo(end);
		return false;
	}

	/// <summary>
	///     Advances the clock by the <paramref name="delay" /> of a wait and returns whether the wait does not complete,
	///     because a cancellation cut it short or it has no limit.
	/// </summary>
	private bool IsCutShort(TimeSpan delay)
	{
		if (delay != Timeout.InfiniteTimeSpan)
		{
			return Advance(delay, true);
		}

		bool isCanceled = false;
		while (!isCanceled && TakeNextCancellation(TimeSpan.MaxValue) is { } source)
		{
			isCanceled = TryCancel(source);
		}

		return true;
	}

	/// <summary>
	///     Cancels the <paramref name="source" />, unless it is already disposed: its timeout was released, like the
	///     timer of a real one.
	/// </summary>
	private static bool TryCancel(CancellationTokenSource source)
	{
		try
		{
			source.Cancel();
			return true;
		}
		catch (ObjectDisposedException)
		{
			return false;
		}
	}

	/// <summary>
	///     Moves the clock to the earliest cancellation scheduled until the <paramref name="end" /> and returns its
	///     source, or returns <see langword="null" /> when there is none.
	/// </summary>
	private CancellationTokenSource? TakeNextCancellation(TimeSpan end)
	{
		lock (_lock)
		{
			(TimeSpan Time, CancellationTokenSource Source)[] due = _cancellations
				.Where(cancellation => cancellation.Time <= end)
				.OrderBy(cancellation => cancellation.Time)
				.Take(1).ToArray();
			if (due.Length == 0)
			{
				return null;
			}

			_cancellations.Remove(due[0]);
			MoveTo(due[0].Time);
			return due[0].Source;
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
}
