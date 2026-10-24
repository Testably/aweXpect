#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     …comply with the <paramref name="expectations" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		ComplyWith<TItem>(this IAsyncEnumerableItems<TItem> items, Action<IThatSubject<TItem>> expectations)
	{
		expectations.ThrowIfNull();
		return new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			items.Subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, Expectations: expectations),
				static (state, it, grammars)
					=> new AsyncComplyWithConstraint<TItem>(it, grammars, state.Quantifier, state.Expectations)),
			items.Subject);
	}
}
#endif
