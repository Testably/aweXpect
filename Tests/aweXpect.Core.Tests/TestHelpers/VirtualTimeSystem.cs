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
///     The timers that cancel stay real, so a test on the virtual clock has to keep them out of its way:
///     <list type="bullet">
///         <item>
///             The timeout of the evaluation (<c>WithTimeout(…)</c> and the test cancellation) and a
///             <see cref="CancellationTokenSource" /> with a delay cancel after real time. A test that needs a
///             timeout which is shorter than the budget stays on the real time system, and a cancellation is
///             scheduled with <see cref="CancelAt(TimeSpan, CancellationTokenSource)" />.
///         </item>
///         <item>
///             The limit of an attempt of <c>Eventually()</c> cancels the token of the attempt after the remaining
///             budget in real time, and abandons a subject that is still running then. A subject that completes
///             synchronously, ignores its token and throws no <see cref="OperationCanceledException" /> is not
///             affected, because only the measured, i.e. virtual, time judges it. Any other subject needs a budget
///             that real time does not reach, and a subject that never completes stays on the real time system.
///         </item>
///     </list>
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
