#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	public sealed partial class WithoutValue
	{
		/// <summary>
		///     Verifies that the synchronous delegate allocates…
		/// </summary>
		/// <remarks>
		///     The bytes allocated on the current thread are measured for a single invocation of the delegate, so
		///     allocations on other threads are not included. The first invocation can include one-time allocations,
		///     e.g. of static constructors or lazy caches, which <c>AfterWarmUp()</c> excludes.
		///     <para />
		///     A delegate that throws an exception fails the expectation, unless <c>AllowingExceptions()</c> is specified.
		/// </remarks>
		/// <exception cref="NotSupportedException">
		///     The delegate is asynchronous, as it can continue on another thread, whose allocations are not measured.
		/// </exception>
		[GuaranteesNotNull]
		public AllocatesResult<AndResult<WithoutValue>> Allocates()
		{
			AllocationOptions options = MeasureAllocations();
			return new AllocatesResult<AndResult<WithoutValue>>(
				new AndResult<WithoutValue>(_expectationBuilder.AddConstraint(options,
						static (allocationOptions, it, grammars)
							=> new AllocatesConstraint(it, grammars, allocationOptions, null)),
					this),
				options);
		}
	}
}
#endif
