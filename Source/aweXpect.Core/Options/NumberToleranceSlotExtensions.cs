#if NET8_0_OR_GREATER
using System.Numerics;
#else
using System;
#endif
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="NumberTolerance{TNumber}" /> on the <see cref="OptionsSlot{TOptions}" /> of an
///     expectation.
/// </summary>
public static class NumberToleranceSlotExtensions
{
	/// <inheritdoc cref="NumberTolerance{TNumber}.Within(TNumber)" />
	public static OptionsSlot<NumberTolerance<TNumber>> Within<TNumber>(
		this OptionsSlot<NumberTolerance<TNumber>> slot, TNumber tolerance)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		slot.Value = slot.Value.Within(tolerance);
		return slot;
	}
}
