using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="TimeTolerance" /> on the <see cref="OptionsHolder{TOptions}" /> of an expectation.
/// </summary>
public static class TimeToleranceHolderExtensions
{
	/// <inheritdoc cref="TimeTolerance.Within(TimeSpan)" />
	public static OptionsHolder<TimeTolerance> Within(this OptionsHolder<TimeTolerance> holder, TimeSpan tolerance)
	{
		holder.Value = holder.Value.Within(tolerance);
		return holder;
	}
}
