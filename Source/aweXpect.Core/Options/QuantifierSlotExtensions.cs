using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="Quantifier" /> on the <see cref="OptionsSlot{TOptions}" /> of an expectation.
/// </summary>
public static class QuantifierSlotExtensions
{
	/// <inheritdoc cref="Quantifier.AtLeast(int)" />
	public static OptionsSlot<Quantifier> AtLeast(this OptionsSlot<Quantifier> slot, int minimum)
	{
		slot.Value = slot.Value.AtLeast(minimum);
		return slot;
	}

	/// <inheritdoc cref="Quantifier.AtMost(int)" />
	public static OptionsSlot<Quantifier> AtMost(this OptionsSlot<Quantifier> slot, int maximum)
	{
		slot.Value = slot.Value.AtMost(maximum);
		return slot;
	}

	/// <inheritdoc cref="Quantifier.Between(int, int)" />
	public static OptionsSlot<Quantifier> Between(this OptionsSlot<Quantifier> slot, int minimum, int maximum)
	{
		slot.Value = slot.Value.Between(minimum, maximum);
		return slot;
	}

	/// <inheritdoc cref="Quantifier.Exactly(int)" />
	public static OptionsSlot<Quantifier> Exactly(this OptionsSlot<Quantifier> slot, int expected)
	{
		slot.Value = slot.Value.Exactly(expected);
		return slot;
	}

	/// <inheritdoc cref="Quantifier.Exactly(int, string)" />
	internal static OptionsSlot<Quantifier> Exactly(this OptionsSlot<Quantifier> slot, int expected, string option)
	{
		slot.Value = slot.Value.Exactly(expected, option);
		return slot;
	}

	/// <inheritdoc cref="Quantifier.LessThan(int)" />
	public static OptionsSlot<Quantifier> LessThan(this OptionsSlot<Quantifier> slot, int maximum)
	{
		slot.Value = slot.Value.LessThan(maximum);
		return slot;
	}

	/// <inheritdoc cref="Quantifier.MoreThan(int)" />
	public static OptionsSlot<Quantifier> MoreThan(this OptionsSlot<Quantifier> slot, int minimum)
	{
		slot.Value = slot.Value.MoreThan(minimum);
		return slot;
	}
}
