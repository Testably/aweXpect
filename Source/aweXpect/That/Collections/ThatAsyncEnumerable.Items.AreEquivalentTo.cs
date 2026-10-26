#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Core.Metadata;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     …are equivalent to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		AreEquivalentTo<TItem, TExpected>(this IAsyncEnumerableItems<TItem> items,
			[RequiresMemberMetadata] TExpected expected,
			Func<EquivalencyOptions<TExpected>, EquivalencyOptions>? options = null,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		EquivalencyOptions equivalencyOptions = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get();
		if (options != null)
		{
			equivalencyOptions = options(new EquivalencyOptions<TExpected>(equivalencyOptions));
		}

		ObjectMatchType<TItem> equalityOptions = new EquivalencyMatchType<TItem>(equivalencyOptions);
		return new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, Expected: expected, DoNotPopulateThisValue: doNotPopulateThisValue,
					EqualityOptions: equalityOptions),
				static (state, it, grammars)
					=> new AsyncCollectionConstraint<TItem>(
						it, grammars,
						state.Quantifier,
						new ElementEquivalentTo<TItem, TItem, TExpected>(state.EqualityOptions, state.Expected,
							state.DoNotPopulateThisValue),
						"were")),
			items.Subject);
	}

	/// <summary>
	///     …are equivalent to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     This overload allows passing a literal <see langword="null" />, for which the generic type cannot be inferred.
	/// </remarks>
	public static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		AreEquivalentTo<TItem>(this IAsyncEnumerableItems<TItem> items, [RequiresMemberMetadata] object? expected,
			Func<EquivalencyOptions<object?>, EquivalencyOptions>? options = null,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		=> AreEquivalentTo<TItem, object?>(items, expected, options, doNotPopulateThisValue);
}
#endif
