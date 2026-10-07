using System.Threading;
using aweXpect.Core.Internal;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Tests.TestHelpers;

internal class TimeSystemMock : ITimeSystem
{
	private TimeSpan _elapsed = TimeSpan.Zero;

	public long GetTimestamp() => 0;

	public TimeSpan GetElapsedTime(long startTimestamp) => _elapsed;

	public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
		=> RealTimeSystem.Instance.Delay(delay, cancellationToken);

	public void CancelAfter(CancellationTokenSource cancellationTokenSource, TimeSpan delay)
		=> RealTimeSystem.Instance.CancelAfter(cancellationTokenSource, delay);

	/// <summary>
	///     Lets every measurement report the <paramref name="elapsed" /> time.
	/// </summary>
	public TimeSystemMock SetElapsed(TimeSpan elapsed)
	{
		_elapsed = elapsed;
		return this;
	}
}
