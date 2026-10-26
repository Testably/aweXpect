#if NET8_0_OR_GREATER
using System.Numerics;
#else
using System;
#endif
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="NumberTolerance{TNumber}" /> on the <see cref="OptionsHolder{TOptions}" /> of an
///     expectation.
/// </summary>
public static class NumberToleranceHolderExtensions
{
	/// <inheritdoc cref="NumberTolerance{TNumber}.Within(TNumber)" />
	public static OptionsHolder<NumberTolerance<TNumber>> Within<TNumber>(
		this OptionsHolder<NumberTolerance<TNumber>> holder, TNumber tolerance)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		holder.Value = holder.Value.Within(tolerance);
		return holder;
	}
}
