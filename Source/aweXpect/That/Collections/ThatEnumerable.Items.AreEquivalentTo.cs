using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Core.Metadata;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     …are equivalent to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		AreEquivalentTo<TItem, TExpected>(this IEnumerableItems<TItem> items,
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
		return new AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, Expected: expected, DoNotPopulateThisValue: doNotPopulateThisValue,
					EqualityOptions: equalityOptions),
				static (state, it, grammars)
					=> new CollectionConstraint<IEnumerable<TItem>?, TItem>(
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
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		AreEquivalentTo<TItem>(this IEnumerableItems<TItem> items, [RequiresMemberMetadata] object? expected,
			Func<EquivalencyOptions<object?>, EquivalencyOptions>? options = null,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		=> AreEquivalentTo<TItem, object?>(items, expected, options, doNotPopulateThisValue);

	/// <summary>
	///     …are equivalent to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<TCollection, IThat<TCollection?>>
		AreEquivalentTo<TCollection, TExpected>(this INonGenericEnumerableItems<TCollection> items,
			[RequiresMemberMetadata] TExpected expected,
			Func<EquivalencyOptions<TExpected>, EquivalencyOptions>? options = null,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TCollection : IEnumerable?
	{
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		EquivalencyOptions equivalencyOptions = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get();
		if (options != null)
		{
			equivalencyOptions = options(new EquivalencyOptions<TExpected>(equivalencyOptions));
		}

		ObjectMatchType<object?> equalityOptions = new EquivalencyMatchType<object?>(equivalencyOptions);
		return new AndOrResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, Expected: expected, DoNotPopulateThisValue: doNotPopulateThisValue,
					EqualityOptions: equalityOptions),
				static (state, it, grammars)
					=> new CollectionConstraint<TCollection, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEquivalentTo<object?, object?, TExpected>(state.EqualityOptions, state.Expected,
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
	public static AndOrResult<TCollection, IThat<TCollection?>>
		AreEquivalentTo<TCollection>(this INonGenericEnumerableItems<TCollection> items,
			[RequiresMemberMetadata] object? expected,
			Func<EquivalencyOptions<object?>, EquivalencyOptions>? options = null,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TCollection : IEnumerable?
		=> AreEquivalentTo<TCollection, object?>(items, expected, options, doNotPopulateThisValue);

	/// <summary>
	///     …are equivalent to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<TCollection, IThat<TCollection>>
		AreEquivalentTo<TCollection, TItem, TExpected>(this IStructEnumerableItems<TCollection, TItem> items,
			[RequiresMemberMetadata] TExpected expected,
			Func<EquivalencyOptions<TExpected>, EquivalencyOptions>? options = null,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TCollection : struct, IEnumerable<TItem>
	{
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		EquivalencyOptions equivalencyOptions = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get();
		if (options != null)
		{
			equivalencyOptions = options(new EquivalencyOptions<TExpected>(equivalencyOptions));
		}

		ObjectMatchType<TItem> equalityOptions = new EquivalencyMatchType<TItem>(equivalencyOptions);
		return new AndOrResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, Expected: expected, DoNotPopulateThisValue: doNotPopulateThisValue,
					EqualityOptions: equalityOptions),
				static (state, it, grammars)
					=> new CollectionConstraint<TCollection, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEquivalentTo<object?, TItem, TExpected>(state.EqualityOptions, state.Expected,
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
	public static AndOrResult<TCollection, IThat<TCollection>>
		AreEquivalentTo<TCollection, TItem>(this IStructEnumerableItems<TCollection, TItem> items,
			[RequiresMemberMetadata] object? expected,
			Func<EquivalencyOptions<object?>, EquivalencyOptions>? options = null,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TCollection : struct, IEnumerable<TItem>
		=> AreEquivalentTo<TCollection, TItem, object?>(items, expected, options, doNotPopulateThisValue);
}
