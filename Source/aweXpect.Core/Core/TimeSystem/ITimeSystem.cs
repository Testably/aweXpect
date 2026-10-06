using System;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Core.TimeSystem;

internal interface ITimeSystem
{
	IStopwatchFactory Stopwatch { get; }

	/// <summary>
	///     Gets the current timestamp, from which <see cref="GetElapsedTime(long)" /> measures.
	/// </summary>
	/// <remarks>
	///     Measures without allocating, unlike a stopwatch of the <see cref="Stopwatch" /> factory.
	/// </remarks>
	long GetTimestamp();

	/// <summary>
	///     Gets the time elapsed since the <paramref name="startTimestamp" /> from <see cref="GetTimestamp()" />.
	/// </summary>
	TimeSpan GetElapsedTime(long startTimestamp);

	/// <summary>
	///     Creates a task that completes after the <paramref name="delay" />, or is canceled with the
	///     <paramref name="cancellationToken" />.
	/// </summary>
	/// <remarks>Wrapper around <see cref="Task.Delay(TimeSpan, CancellationToken)" /></remarks>
	Task Delay(TimeSpan delay, CancellationToken cancellationToken);
}
