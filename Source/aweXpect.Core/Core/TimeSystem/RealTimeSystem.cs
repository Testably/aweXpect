using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Internal;
using DiagnosticsStopwatch = System.Diagnostics.Stopwatch;

namespace aweXpect.Core.TimeSystem;

internal class RealTimeSystem : ITimeSystem
{
	public static ITimeSystem Instance { get; } = new RealTimeSystem();

	#region ITimeSystem Members

	/// <inheritdoc />
	public long GetTimestamp()
		=> DiagnosticsStopwatch.GetTimestamp();

	/// <inheritdoc />
	public TimeSpan GetElapsedTime(long startTimestamp)
		=> TimeSpan.FromTicks((long)((DiagnosticsStopwatch.GetTimestamp() - startTimestamp) *
		                             ((double)TimeSpan.TicksPerSecond / DiagnosticsStopwatch.Frequency)));

	/// <inheritdoc />
	public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
		=> Task.Delay(delay, cancellationToken);

	/// <inheritdoc />
	public void CancelAfter(CancellationTokenSource cancellationTokenSource, TimeSpan delay)
		=> cancellationTokenSource.CancelAfter(delay);

	#endregion
}
