using System;
using DiagnosticsStopwatch = System.Diagnostics.Stopwatch;

namespace aweXpect.Core.TimeSystem;

internal class RealTimeSystem : ITimeSystem
{
	public static ITimeSystem Instance { get; } = new RealTimeSystem();

	#region ITimeSystem Members

	/// <inheritdoc />
	public IStopwatchFactory Stopwatch { get; } = new RealStopwatchFactory();

	#endregion

	private sealed class RealStopwatchFactory : IStopwatchFactory
	{
		#region IStopwatchFactory Members

		/// <inheritdoc />
		public IStopwatch New()
			=> new RealStopwatch();

		#endregion
	}

	/// <remarks>
	///     Reads the timestamps itself instead of wrapping a <see cref="DiagnosticsStopwatch" />, so that every evaluation
	///     of a delegate allocates one object instead of two.
	/// </remarks>
	private sealed class RealStopwatch : IStopwatch
	{
		private static readonly double TicksPerTimestamp =
			(double)TimeSpan.TicksPerSecond / DiagnosticsStopwatch.Frequency;

		private long _elapsedTimestamps;
		private long _startTimestamp;

		#region IStopwatch Members

		/// <inheritdoc />
		public TimeSpan Elapsed
		{
			get
			{
				long elapsedTimestamps = _elapsedTimestamps;
				if (IsRunning)
				{
					elapsedTimestamps += DiagnosticsStopwatch.GetTimestamp() - _startTimestamp;
				}

				return TimeSpan.FromTicks((long)(elapsedTimestamps * TicksPerTimestamp));
			}
		}

		/// <inheritdoc />
		public bool IsRunning { get; private set; }

		/// <inheritdoc />
		public void Start()
		{
			if (!IsRunning)
			{
				_startTimestamp = DiagnosticsStopwatch.GetTimestamp();
				IsRunning = true;
			}
		}

		/// <inheritdoc />
		public void Stop()
		{
			if (IsRunning)
			{
				_elapsedTimestamps += DiagnosticsStopwatch.GetTimestamp() - _startTimestamp;
				IsRunning = false;
			}
		}

		#endregion
	}
}
