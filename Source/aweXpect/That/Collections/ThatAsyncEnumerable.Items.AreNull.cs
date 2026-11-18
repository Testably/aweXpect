#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     …are <see langword="null" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		AreNull<TItem>(this IAsyncEnumerableItems<TItem> items)
		=> AreNullCore(items, ElementNull<TItem>.Instance);

	/// <summary>
	///     …are not <see langword="null" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		AreNotNull<TItem>(this IAsyncEnumerableItems<TItem> items)
		=> AreNullCore(items, ElementNull<TItem>.NotInstance);

	private static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		AreNullCore<TItem>(IAsyncEnumerableItems<TItem> items, ElementNull<TItem> condition)
	{
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((items.Quantifier, Condition: condition),
				static (state, it, grammars)
					=> new AsyncCollectionConstraint<TItem>(
						it, grammars,
						state.Quantifier,
						state.Condition,
						"were")),
			items.Subject);
	}
}
#endif
