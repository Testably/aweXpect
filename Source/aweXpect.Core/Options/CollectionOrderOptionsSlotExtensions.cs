using System.Collections.Generic;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="CollectionOrderOptions{TItem}" /> on the <see cref="OptionsSlot{TOptions}" /> of
///     an expectation.
/// </summary>
public static class CollectionOrderOptionsSlotExtensions
{
	/// <inheritdoc cref="CollectionOrderOptions{TItem}.SetComparer(IComparer{TItem})" />
	public static OptionsSlot<CollectionOrderOptions<TItem>> SetComparer<TItem>(
		this OptionsSlot<CollectionOrderOptions<TItem>> slot, IComparer<TItem> comparer)
	{
		slot.Value = slot.Value.SetComparer(comparer);
		return slot;
	}
}
