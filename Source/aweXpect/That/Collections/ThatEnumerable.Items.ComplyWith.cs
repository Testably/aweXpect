using System;
using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     …comply with the <paramref name="expectations" />.
	/// </summary>
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		ComplyWith<TItem>(this IEnumerableItems<TItem> items, Action<IThatSubject<TItem>> expectations)
	{
		expectations.ThrowIfNull();
		return new AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			items.Subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, Expectations: expectations),
				static (state, it, grammars)
					=> new ComplyWithConstraint<IEnumerable<TItem>?, TItem>(it, grammars, state.Quantifier,
						state.Expectations)),
			items.Subject);
	}

	/// <summary>
	///     …comply with the <paramref name="expectations" />.
	/// </summary>
	public static AndOrResult<TCollection, IThat<TCollection?>>
		ComplyWith<TCollection>(this INonGenericEnumerableItems<TCollection> items,
			Action<IThatSubject<object?>> expectations)
		where TCollection : IEnumerable?
	{
		expectations.ThrowIfNull();
		return new AndOrResult<TCollection, IThat<TCollection?>>(
			items.Subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, Expectations: expectations),
				static (state, it, grammars)
					=> new ComplyWithConstraint<TCollection?, object?>(it, grammars,
						state.Quantifier, state.Expectations)),
			items.Subject);
	}

	/// <summary>
	///     …comply with the <paramref name="expectations" />.
	/// </summary>
	public static AndOrResult<TCollection, IThat<TCollection>>
		ComplyWith<TCollection, TItem>(this IStructEnumerableItems<TCollection, TItem> items,
			Action<IThatSubject<TItem>> expectations)
		where TCollection : struct, IEnumerable<TItem>
	{
		expectations.ThrowIfNull();
		return new AndOrResult<TCollection, IThat<TCollection>>(
			items.Subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, Expectations: expectations),
				static (state, it, grammars)
					=> new ComplyWithConstraint<TCollection, TItem>(it,
						grammars, state.Quantifier, state.Expectations)),
			items.Subject);
	}
}
