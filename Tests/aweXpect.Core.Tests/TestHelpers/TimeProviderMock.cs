using System.Threading;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     A time provider whose clock advances by the elapsed time of <see cref="SetElapsed(TimeSpan)" /> with every
///     timestamp, so that every measurement from one timestamp to the next reports that time.
/// </summary>
/// <remarks>
///     <see cref="TimeProvider.GetElapsedTime(long)" /> is not virtual and reads the next timestamp itself.
/// </remarks>
internal class TimeProviderMock : TimeProvider
{
	private long _elapsedTicks;
	private long _timestamp;

	/// <inheritdoc />
	public override long TimestampFrequency => TimeSpan.TicksPerSecond;

	/// <inheritdoc />
	public override long GetTimestamp() => Interlocked.Add(ref _timestamp, _elapsedTicks);

	/// <summary>
	///     Lets every measurement report the <paramref name="elapsed" /> time.
	/// </summary>
	public TimeProviderMock SetElapsed(TimeSpan elapsed)
	{
		_elapsedTicks = elapsed.Ticks;
		return this;
	}
}
