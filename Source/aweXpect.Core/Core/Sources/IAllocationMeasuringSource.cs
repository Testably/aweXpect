#if NET8_0_OR_GREATER
using System;
using aweXpect.Options;

namespace aweXpect.Core.Sources;

/// <summary>
///     A synchronous delegate source, whose allocations can be measured on the invoking thread.
/// </summary>
internal interface IAllocationMeasuringSource
{
	/// <summary>
	///     Measures the allocations of the delegate according to the <paramref name="options" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">The allocations are already measured.</exception>
	void MeasureAllocations(AllocationOptions options);
}
#endif
