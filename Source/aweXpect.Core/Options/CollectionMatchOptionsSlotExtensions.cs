using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="CollectionMatchOptions" /> on the <see cref="OptionsSlot{TOptions}" /> of an
///     expectation.
/// </summary>
public static class CollectionMatchOptionsSlotExtensions
{
	/// <inheritdoc cref="CollectionMatchOptions.IgnoringDuplicates(bool)" />
	public static OptionsSlot<CollectionMatchOptions> IgnoringDuplicates(this OptionsSlot<CollectionMatchOptions> slot,
		bool ignoreDuplicates = true)
	{
		slot.Value = slot.Value.IgnoringDuplicates(ignoreDuplicates);
		return slot;
	}

	/// <inheritdoc cref="CollectionMatchOptions.IgnoringInterspersedItems(bool)" />
	public static OptionsSlot<CollectionMatchOptions> IgnoringInterspersedItems(
		this OptionsSlot<CollectionMatchOptions> slot, bool ignoreInterspersedItems = true)
	{
		slot.Value = slot.Value.IgnoringInterspersedItems(ignoreInterspersedItems);
		return slot;
	}

	/// <inheritdoc cref="CollectionMatchOptions.InAnyOrder()" />
	public static OptionsSlot<CollectionMatchOptions> InAnyOrder(this OptionsSlot<CollectionMatchOptions> slot)
	{
		slot.Value = slot.Value.InAnyOrder();
		return slot;
	}

	/// <inheritdoc cref="CollectionMatchOptions.Properly()" />
	public static OptionsSlot<CollectionMatchOptions> Properly(this OptionsSlot<CollectionMatchOptions> slot)
	{
		slot.Value = slot.Value.Properly();
		return slot;
	}
}
