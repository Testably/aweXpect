#if NETFRAMEWORK
using System.Threading;
using Testably.Abstractions;

namespace aweXpect.TestHelpers;

/// <summary>
///     The <c>ToTimeProvider()</c> of <c>Testably.Abstractions.Testing</c>, which only exists for .NET 8 and newer,
///     so that the tests on .NET Framework run on the same <see cref="MockTimeSystem" />.
/// </summary>
internal static class TimeSystemExtensions
{
	public static TimeProvider ToTimeProvider(this ITimeSystem timeSystem)
		=> new TimeSystemTimeProvider(timeSystem);

	private sealed class TimeSystemTimeProvider(ITimeSystem timeSystem) : TimeProvider
	{
		public override TimeZoneInfo LocalTimeZone => timeSystem.TimeZoneInfo.Local;

		public override long TimestampFrequency => timeSystem.Stopwatch.Frequency;

		public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
			=> new TimerAdapter(timeSystem.Timer.New(callback, state, dueTime, period));

		public override long GetTimestamp() => timeSystem.Stopwatch.GetTimestamp();

		public override DateTimeOffset GetUtcNow() => new(timeSystem.DateTime.UtcNow);
	}

	private sealed class TimerAdapter(Testably.Abstractions.TimeSystem.ITimer timer) : ITimer
	{
		public bool Change(TimeSpan dueTime, TimeSpan period) => timer.Change(dueTime, period);

		public void Dispose() => timer.Dispose();

		public ValueTask DisposeAsync()
		{
			timer.Dispose();
			return default;
		}
	}
}
#endif
