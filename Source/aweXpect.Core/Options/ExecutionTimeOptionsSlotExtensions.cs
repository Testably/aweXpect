using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="ExecutionTimeOptions" /> on the <see cref="OptionsSlot{TOptions}" /> of an
///     expectation.
/// </summary>
public static class ExecutionTimeOptionsSlotExtensions
{
	/// <inheritdoc cref="ExecutionTimeOptions.AtLeast(TimeSpan)" />
	public static OptionsSlot<ExecutionTimeOptions> AtLeast(this OptionsSlot<ExecutionTimeOptions> slot,
		TimeSpan minimum)
	{
		slot.Value = slot.Value.AtLeast(minimum);
		return slot;
	}

	/// <inheritdoc cref="ExecutionTimeOptions.AtMost(TimeSpan)" />
	public static OptionsSlot<ExecutionTimeOptions> AtMost(this OptionsSlot<ExecutionTimeOptions> slot,
		TimeSpan maximum)
	{
		slot.Value = slot.Value.AtMost(maximum);
		return slot;
	}

	/// <inheritdoc cref="ExecutionTimeOptions.Between(TimeSpan, TimeSpan)" />
	public static OptionsSlot<ExecutionTimeOptions> Between(this OptionsSlot<ExecutionTimeOptions> slot,
		TimeSpan minimum, TimeSpan maximum)
	{
		slot.Value = slot.Value.Between(minimum, maximum);
		return slot;
	}

	/// <inheritdoc cref="ExecutionTimeOptions.Within(TimeSpan)" />
	public static OptionsSlot<ExecutionTimeOptions> Within(this OptionsSlot<ExecutionTimeOptions> slot,
		TimeSpan duration)
	{
		slot.Value = slot.Value.Within(duration);
		return slot;
	}
}
