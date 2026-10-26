using System.Collections.Generic;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="CollectionOrderOptions{TElement}" /> on the <see cref="OptionsHolder{TOptions}" /> of
///     an expectation.
/// </summary>
public static class CollectionOrderOptionsHolderExtensions
{
	/// <inheritdoc cref="CollectionOrderOptions{TElement}.SetComparer(IComparer{TElement})" />
	public static OptionsHolder<CollectionOrderOptions<TElement>> SetComparer<TElement>(
		this OptionsHolder<CollectionOrderOptions<TElement>> holder, IComparer<TElement> comparer)
	{
		holder.Value = holder.Value.SetComparer(comparer);
		return holder;
	}
}
