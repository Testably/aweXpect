using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="RepeatedCheckOptions" /> on the <see cref="OptionsSlot{TOptions}" /> of an
///     expectation.
/// </summary>
public static class RepeatedCheckOptionsSlotExtensions
{
	/// <inheritdoc cref="RepeatedCheckOptions.CheckEvery(TimeSpan)" />
	public static OptionsSlot<RepeatedCheckOptions> CheckEvery(this OptionsSlot<RepeatedCheckOptions> slot,
		TimeSpan interval)
	{
		slot.Value = slot.Value.CheckEvery(interval);
		return slot;
	}

	/// <inheritdoc cref="RepeatedCheckOptions.Within(TimeSpan)" />
	public static OptionsSlot<RepeatedCheckOptions> Within(this OptionsSlot<RepeatedCheckOptions> slot,
		TimeSpan timeout)
	{
		slot.Value = slot.Value.Within(timeout);
		return slot;
	}
}
