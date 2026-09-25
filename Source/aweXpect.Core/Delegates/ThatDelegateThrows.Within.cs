using System;
using aweXpect.Options;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <summary>
	///     Verifies that the delegate throws within the given <paramref name="duration" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="duration" /> is applied as timeout (a tighter timeout, e.g. from <c>WithTimeout(…)</c>,
	///     still applies), so that a delegate accepting a <see cref="System.Threading.CancellationToken" /> is canceled
	///     once it elapsed. The task of an asynchronous delegate is abandoned at that point, even if it ignores the
	///     cancellation, while a synchronous delegate cannot be interrupted and runs to completion.
	///     A delegate that is canceled or abandoned by the timeout fails with <c>did not finish within …</c>.
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> imposes no limit.
	/// </remarks>
	public ThatDelegateThrows<TException> Within(TimeSpan duration)
	{
		if (duration == System.Threading.Timeout.InfiniteTimeSpan)
		{
			return this;
		}

		ExecutionTimeOptions options = new();
		options.Within(duration);
		ThrowOptions.ExecutionTimeOptions = options;
		ExpectationBuilder.WithTimeout(duration);
		return this;
	}
}
