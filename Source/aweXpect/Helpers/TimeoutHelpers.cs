using System;
using System.Threading;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class TimeoutHelpers
{
	/// <summary>
	///     How close to the timeout a cancellation still counts as the timeout having elapsed.
	/// </summary>
	/// <remarks>
	///     The timers of the wait and of the cancellation do not share the clock of the stopwatch that measures the wait.
	/// </remarks>
	private static readonly TimeSpan CancellationTolerance = TimeSpan.FromMilliseconds(2);

	/// <summary>
	///     Whether a wait for the <paramref name="timeout" /> that a cancellation ended after <paramref name="waited" />
	///     counts as the <paramref name="timeout" /> having elapsed, so that the result decides instead of the
	///     cancellation.
	/// </summary>
	/// <remarks>
	///     Like for <c>Within</c> on repeated checks and for <c>Eventually()</c>, the <paramref name="timeout" /> decides
	///     when the outer timeout of the <paramref name="expectationBuilder" /> is not shorter. The timer of the outer
	///     timeout starts before the wait does, so it can expire slightly before the <paramref name="timeout" /> does.
	/// </remarks>
	public static bool IsTimeoutReachedAt(this ExpectationBuilder expectationBuilder, TimeSpan timeout,
		TimeSpan waited)
	{
		if (timeout == Timeout.InfiniteTimeSpan)
		{
			return false;
		}

		if (timeout - waited < CancellationTolerance)
		{
			return true;
		}

		return expectationBuilder.Timeout >= timeout &&
		       expectationBuilder.CancellationToken?.IsCancellationRequested != true;
	}
}
