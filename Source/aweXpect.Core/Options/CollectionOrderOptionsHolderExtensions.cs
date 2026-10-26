using System.Collections.Generic;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="CollectionOrderOptions{TItem}" /> on the <see cref="OptionsHolder{TOptions}" /> of
///     an expectation.
/// </summary>
public static class CollectionOrderOptionsHolderExtensions
{
	/// <inheritdoc cref="CollectionOrderOptions{TItem}.SetComparer(IComparer{TItem})" />
	public static OptionsHolder<CollectionOrderOptions<TItem>> SetComparer<TItem>(
		this OptionsHolder<CollectionOrderOptions<TItem>> holder, IComparer<TItem> comparer)
	{
		holder.Value = holder.Value.SetComparer(comparer);
		return holder;
	}
}
