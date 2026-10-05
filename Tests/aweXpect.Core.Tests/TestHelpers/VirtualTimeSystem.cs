using System.Collections.Generic;
using System.Linq;
using System.Threading;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     A time system with a virtual clock for the checks of a repeated check (<c>Eventually()</c> and
///     <c>Within(…)</c>), so that a test neither waits nor depends on the speed of the machine.
/// </summary>
/// <remarks>
///     The clock only moves through the waits between the checks, each of which advances it by exactly its duration
///     and completes at once, and through <see cref="Advance(TimeSpan)" />, with which e.g. a subject states how long
///     its attempt took. As a wait is the only thing that moves the clock on its own, a repeated check with a limited
///     budget and a positive interval always ends.
///     <para />
///     The timers that cancel stay real: the limit after which <c>Eventually()</c> abandons an attempt that is still
///     running, the timeout of the evaluation (<c>WithTimeout(…)</c> and the test cancellation) and a
///     <see cref="CancellationTokenSource" /> with a delay. They cannot influence a subject that completes
///     synchronously without throwing an <see cref="OperationCanceledException" />, because it is judged by the
///     measured, i.e. virtual, time only. Every other test must either stay on the real time system (a subject that
///     never completes, <c>WithTimeout(…)</c> shorter than the budget) or use a budget that real time does not reach
///     (a subject that completes asynchronously or throws an <see cref="OperationCanceledException" />).
/// </remarks>
internal sealed class VirtualTimeSystem : ITimeSystem, IStopwatchFactory
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
	IStopwatch IStopwatchFactory.New() => new VirtualStopwatch(this);

	/// <inheritdoc />
	public IStopwatchFactory Stopwatch => this;

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
	/// </remarks>
	public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
		=> Task.Run(() => Advance(delay, true)
			? Task.Delay(SafetyNet, cancellationToken)
			: Task.CompletedTask, cancellationToken);

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
			source.Cancel();
			if (isWait)
			{
				return true;
			}
		}

		MoveTo(end);
		return false;
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

	private sealed class VirtualStopwatch(VirtualTimeSystem timeSystem) : IStopwatch
	{
		private TimeSpan _elapsed;
		private TimeSpan _startedAt;

		public TimeSpan Elapsed => IsRunning ? _elapsed + (timeSystem.Now - _startedAt) : _elapsed;

		public bool IsRunning { get; private set; }

		public void Start()
		{
			if (!IsRunning)
			{
				_startedAt = timeSystem.Now;
				IsRunning = true;
			}
		}

		public void Stop()
		{
			if (IsRunning)
			{
				_elapsed += timeSystem.Now - _startedAt;
				IsRunning = false;
			}
		}
	}
}
