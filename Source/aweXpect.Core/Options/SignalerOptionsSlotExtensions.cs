using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="SignalerOptions" /> on the <see cref="OptionsSlot{TOptions}" /> of an
///     expectation.
/// </summary>
public static class SignalerOptionsSlotExtensions
{
	/// <inheritdoc cref="SignalerOptions.Within(TimeSpan)" />
	public static OptionsSlot<SignalerOptions> Within(this OptionsSlot<SignalerOptions> slot, TimeSpan timeout)
	{
		slot.Value = slot.Value.Within(timeout);
		return slot;
	}
}
