using System;
using System.Threading;

namespace aweXpect.Core.Helpers;

internal static class TimerHelpers
{
	/// <summary>
	///     Returns the <paramref name="timeout" /> to pass to a timer or wait, which is
	///     <see cref="Timeout.InfiniteTimeSpan" /> when the <paramref name="timeout" /> exceeds what they accept.
	/// </summary>
	/// <remarks>
	///     A longer duration is valid in an expectation, so it must not make the timer throw; such a timer would not
	///     elapse during a test run anyway. The limit of <see cref="int.MaxValue" /> milliseconds is the one of the
	///     timers on .NET Framework and of the waits on .NET, so it applies uniformly on every target framework.
	/// </remarks>
	public static TimeSpan ToTimerTimeout(this TimeSpan timeout)
		=> (long)timeout.TotalMilliseconds > int.MaxValue ? Timeout.InfiniteTimeSpan : timeout;
}
