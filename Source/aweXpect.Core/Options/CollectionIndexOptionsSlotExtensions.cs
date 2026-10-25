using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="CollectionIndexOptions" /> on the <see cref="OptionsSlot{TOptions}" /> of an
///     expectation.
/// </summary>
public static class CollectionIndexOptionsSlotExtensions
{
	/// <inheritdoc cref="CollectionIndexOptions.AtIndex(int)" />
	public static OptionsSlot<CollectionIndexOptions> AtIndex(this OptionsSlot<CollectionIndexOptions> slot, int index)
	{
		slot.Value = slot.Value.AtIndex(index);
		return slot;
	}

	/// <inheritdoc cref="CollectionIndexOptions.AtIndexFromEnd(int)" />
	public static OptionsSlot<CollectionIndexOptions> AtIndexFromEnd(this OptionsSlot<CollectionIndexOptions> slot,
		int index)
	{
		slot.Value = slot.Value.AtIndexFromEnd(index);
		return slot;
	}

	/// <inheritdoc cref="CollectionIndexOptions.SetMatch(CollectionIndexOptions.IMatch, string)" />
	public static OptionsSlot<CollectionIndexOptions> SetMatch(this OptionsSlot<CollectionIndexOptions> slot,
		CollectionIndexOptions.IMatch match, string optionName)
	{
		slot.Value = slot.Value.SetMatch(match, optionName);
		return slot;
	}
}
