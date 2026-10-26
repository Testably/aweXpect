using System;
using System.Threading;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <summary>
	///     Verifies that the delegate throws within the given <paramref name="timeout" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="timeout" /> also limits the evaluation (a tighter timeout, e.g. from <c>WithTimeout(…)</c>,
	///     still applies), so that a delegate accepting a <see cref="System.Threading.CancellationToken" /> is canceled
	///     once it elapsed. The task of an asynchronous delegate is abandoned at that point, even if it ignores the
	///     cancellation, while a synchronous delegate cannot be interrupted and runs to completion.
	///     A delegate that is canceled or abandoned by the timeout fails with <c>did not finish within …</c>.
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> imposes no limit.<br />
	///     A later timeout replaces the earlier one.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public ThatDelegateThrows<TException> Within(TimeSpan timeout)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		if (timeout == Timeout.InfiniteTimeSpan)
		{
			ThrowOptions.ExecutionTimeOptions = null;
			return this;
		}

		ThrowOptions.ExecutionTimeOptions = ExecutionTimeOptions.Default.Within(timeout);
		ExpectationBuilder.AddTimeoutBound(ThrowOptions);
		return this;
	}
}
