using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     …satisfy the <paramref name="predicate" />.
	/// </summary>
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		Satisfy<TItem>(this IEnumerableItems<TItem> items, System.Func<TItem, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, DoNotPopulateThisValue: doNotPopulateThisValue, Predicate: predicate),
				static (state, it, grammars)
					=> new CollectionConstraint<IEnumerable<TItem>?, TItem>(
						it, grammars,
						state.Quantifier,
						new ElementSatisfying<TItem, TItem>(state.Predicate, state.DoNotPopulateThisValue),
						"did")),
			items.Subject);
	}

	/// <summary>
	///     …satisfy the <paramref name="predicate" />.
	/// </summary>
	public static AndOrResult<TCollection, IThat<TCollection?>>
		Satisfy<TCollection>(this INonGenericEnumerableItems<TCollection> items, System.Func<object?, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
		where TCollection : IEnumerable?
	{
		predicate.ThrowIfNull();
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new AndOrResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, DoNotPopulateThisValue: doNotPopulateThisValue, Predicate: predicate),
				static (state, it, grammars)
					=> new CollectionConstraint<TCollection, object?>(
						it, grammars,
						state.Quantifier,
						new ElementSatisfying<object?, object?>(state.Predicate, state.DoNotPopulateThisValue),
						"did")),
			items.Subject);
	}

	/// <summary>
	///     …satisfy the <paramref name="predicate" />.
	/// </summary>
	public static AndOrResult<TCollection, IThat<TCollection>>
		Satisfy<TCollection, TItem>(this IStructEnumerableItems<TCollection, TItem> items,
			System.Func<TItem, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
		where TCollection : struct, IEnumerable<TItem>
	{
		predicate.ThrowIfNull();
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new AndOrResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, DoNotPopulateThisValue: doNotPopulateThisValue, Predicate: predicate),
				static (state, it, grammars)
					=> new CollectionConstraint<TCollection, object?>(
						it, grammars,
						state.Quantifier,
						new ElementSatisfying<object?, TItem>(state.Predicate, state.DoNotPopulateThisValue),
						"did")),
			items.Subject);
	}
}
