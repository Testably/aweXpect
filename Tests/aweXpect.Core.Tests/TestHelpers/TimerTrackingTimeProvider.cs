using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     A time provider on the clock of the <paramref name="inner" /> provider that tracks whether its timers are released.
/// </summary>
internal sealed class TimerTrackingTimeProvider(TimeProvider inner) : TimeProvider
{
	private readonly ConcurrentQueue<TrackedTimer> _timers = new();

	/// <inheritdoc />
	public override TimeZoneInfo LocalTimeZone => inner.LocalTimeZone;

	/// <inheritdoc />
	public override long TimestampFrequency => inner.TimestampFrequency;

	/// <summary>
	///     The number of created timers that were not released yet.
	/// </summary>
	public int ActiveTimers => _timers.Count(timer => !timer.IsDisposed);

	/// <inheritdoc />
	public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
	{
		TrackedTimer timer = new(inner.CreateTimer(callback, state, dueTime, period));
		_timers.Enqueue(timer);
		return timer;
	}

	/// <inheritdoc />
	public override long GetTimestamp() => inner.GetTimestamp();

	/// <inheritdoc />
	public override DateTimeOffset GetUtcNow() => inner.GetUtcNow();

	private sealed class TrackedTimer(ITimer timer) : ITimer
	{
		public bool IsDisposed { get; private set; }

		public bool Change(TimeSpan dueTime, TimeSpan period) => timer.Change(dueTime, period);

		public void Dispose()
		{
			IsDisposed = true;
			timer.Dispose();
		}

		public ValueTask DisposeAsync()
		{
			Dispose();
			return default;
		}
	}
}
