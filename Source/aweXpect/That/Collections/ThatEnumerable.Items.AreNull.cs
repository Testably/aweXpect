using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     …are <see langword="null" />.
	/// </summary>
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		AreNull<TItem>(this IEnumerableItems<TItem> items)
		=> AreNullCore(items, ElementNull<TItem>.Instance);

	/// <summary>
	///     …are <see langword="null" />.
	/// </summary>
	public static AndOrResult<TCollection, IThat<TCollection?>>
		AreNull<TCollection>(this INonGenericEnumerableItems<TCollection> items)
		where TCollection : IEnumerable?
		=> AreNullCore(items, ElementNull<object?>.Instance);

	/// <summary>
	///     …are <see langword="null" />.
	/// </summary>
	public static AndOrResult<TCollection, IThat<TCollection>>
		AreNull<TCollection, TItem>(this IStructEnumerableItems<TCollection, TItem> items)
		where TCollection : struct, IEnumerable<TItem>
		=> AreNullCore(items, ElementNull<object?>.Instance);

	/// <summary>
	///     …are not <see langword="null" />.
	/// </summary>
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		AreNotNull<TItem>(this IEnumerableItems<TItem> items)
		=> AreNullCore(items, ElementNull<TItem>.NotInstance);

	/// <summary>
	///     …are not <see langword="null" />.
	/// </summary>
	public static AndOrResult<TCollection, IThat<TCollection?>>
		AreNotNull<TCollection>(this INonGenericEnumerableItems<TCollection> items)
		where TCollection : IEnumerable?
		=> AreNullCore(items, ElementNull<object?>.NotInstance);

	/// <summary>
	///     …are not <see langword="null" />.
	/// </summary>
	public static AndOrResult<TCollection, IThat<TCollection>>
		AreNotNull<TCollection, TItem>(this IStructEnumerableItems<TCollection, TItem> items)
		where TCollection : struct, IEnumerable<TItem>
		=> AreNullCore(items, ElementNull<object?>.NotInstance);

	private static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		AreNullCore<TItem>(IEnumerableItems<TItem> items, ElementNull<TItem> condition)
	{
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((items.Quantifier, Condition: condition),
				static (state, it, grammars)
					=> new CollectionConstraint<IEnumerable<TItem>?, TItem>(
						it, grammars,
						state.Quantifier,
						state.Condition,
						"were")),
			items.Subject);
	}

	private static AndOrResult<TCollection, IThat<TCollection?>>
		AreNullCore<TCollection>(INonGenericEnumerableItems<TCollection> items, ElementNull<object?> condition)
		where TCollection : IEnumerable?
	{
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new AndOrResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint((items.Quantifier, Condition: condition),
				static (state, it, grammars)
					=> new CollectionConstraint<TCollection, object?>(
						it, grammars,
						state.Quantifier,
						state.Condition,
						"were")),
			items.Subject);
	}

	private static AndOrResult<TCollection, IThat<TCollection>>
		AreNullCore<TCollection, TItem>(IStructEnumerableItems<TCollection, TItem> items,
			ElementNull<object?> condition)
		where TCollection : struct, IEnumerable<TItem>
	{
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new AndOrResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint((items.Quantifier, Condition: condition),
				static (state, it, grammars)
					=> new CollectionConstraint<TCollection, object?>(
						it, grammars,
						state.Quantifier,
						state.Condition,
						"were")),
			items.Subject);
	}
}
