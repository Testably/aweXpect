using System.Threading;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Tests.TestHelpers;

internal class TimeSystemMock : ITimeSystem
{
	private readonly StopwatchFactoryMock _stopwatchFactory = new();
	public IStopwatchFactory Stopwatch => _stopwatchFactory;

	public long GetTimestamp() => RealTimeSystem.Instance.GetTimestamp();

	public TimeSpan GetElapsedTime(long startTimestamp) => RealTimeSystem.Instance.GetElapsedTime(startTimestamp);

	public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
		=> RealTimeSystem.Instance.Delay(delay, cancellationToken);

	public void CancelAfter(CancellationTokenSource cancellationTokenSource, TimeSpan delay)
		=> RealTimeSystem.Instance.CancelAfter(cancellationTokenSource, delay);

	public TimeSystemMock SetElapsed(TimeSpan elapsed)
	{
		_stopwatchFactory.SetElapsed(elapsed);
		return this;
	}

	private sealed class StopwatchFactoryMock : IStopwatchFactory
	{
		private TimeSpan _elapsed = TimeSpan.Zero;
		public IStopwatch New() => new StopwatchMock(() => _elapsed);

		public void SetElapsed(TimeSpan elapsed) => _elapsed = elapsed;
	}

	private sealed class StopwatchMock(Func<TimeSpan> getElapsed) : IStopwatch
	{
		public TimeSpan Elapsed { get; private set; }
		public bool IsRunning { get; private set; }

		public void Start() => IsRunning = true;

		public void Stop()
		{
			if (IsRunning)
			{
				Elapsed = getElapsed();
			}

			IsRunning = false;
		}
	}
}
