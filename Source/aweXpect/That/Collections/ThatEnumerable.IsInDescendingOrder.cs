using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInDescendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> source)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"")),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInDescendingOrder<
			TItem, TMember>(
			this IThat<IEnumerable<TItem>?> source,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderConstraint<TItem, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Descending,
					options,
					$" for {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable>>
		IsInDescendingOrder(this IThat<IEnumerable> source)
	{
		CollectionOrderOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<IEnumerable, object?, object?>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"")),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable>>
		IsInDescendingOrder<TMember>(
			this IThat<IEnumerable> source,
			Func<object?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<IEnumerable, object?, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Descending,
					options,
					$" for {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
			source,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInDescendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> source)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"")),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInDescendingOrder<
			TItem, TMember>(
			this IThat<ImmutableArray<TItem>> source,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<ImmutableArray<TItem>, TItem, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Descending,
					options,
					$" for {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
			source,
			options);
	}
#endif

	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInDescendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> source)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"").Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInDescendingOrder<
			TItem, TMember>(
			this IThat<IEnumerable<TItem>?> source,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderConstraint<TItem, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Descending,
					options,
					$" for {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable>>
		IsNotInDescendingOrder(this IThat<IEnumerable> source)
	{
		CollectionOrderOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<IEnumerable, object?, object?>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"").Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable>>
		IsNotInDescendingOrder<TMember>(
			this IThat<IEnumerable> source,
			Func<object?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<IEnumerable, object?, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Descending,
					options,
					$" for {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			source,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInDescendingOrder<TItem>(this IThat<ImmutableArray<TItem>> source)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"").Invert()),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInDescendingOrder<TItem, TMember>(
			this IThat<ImmutableArray<TItem>> source,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<ImmutableArray<TItem>, TItem, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Descending,
					options,
					$" for {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			source,
			options);
	}
#endif
}
