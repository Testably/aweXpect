using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="TimeTolerance" /> on the <see cref="OptionsSlot{TOptions}" /> of an expectation.
/// </summary>
public static class TimeToleranceSlotExtensions
{
	/// <inheritdoc cref="TimeTolerance.Within(TimeSpan)" />
	public static OptionsSlot<TimeTolerance> Within(this OptionsSlot<TimeTolerance> slot, TimeSpan tolerance)
	{
		slot.Value = slot.Value.Within(tolerance);
		return slot;
	}
}
