using System;
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
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> imposes no limit.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">A timeout is already set.</exception>
	public ThatDelegateThrows<TException> Within(TimeSpan timeout)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(ThrowOptions.IsWithinSpecified, nameof(Within));
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		ThrowOptions.IsWithinSpecified = true;
		if (timeout == System.Threading.Timeout.InfiniteTimeSpan)
		{
			return this;
		}

		ExecutionTimeOptions options = new();
		options.Within(timeout);
		ThrowOptions.ExecutionTimeOptions = options;
		ExpectationBuilder.WithTimeout(timeout);
		return this;
	}
}
