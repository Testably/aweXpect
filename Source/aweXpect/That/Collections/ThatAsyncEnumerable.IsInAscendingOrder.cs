#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection is in ascending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TItem, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInAscendingOrder<TItem>(this IThat<IAsyncEnumerable<TItem>?> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsInOrderConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x, SortOrder.Ascending, options, "")),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in ascending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInAscendingOrder<TItem, TMember>(this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsInOrderConstraint<TItem, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor, SortOrder.Ascending, options,
					$" for {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in ascending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TItem, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotInAscendingOrder<TItem>(this IThat<IAsyncEnumerable<TItem>?> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsInOrderConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x, SortOrder.Ascending, options, "").Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in ascending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotInAscendingOrder<TItem, TMember>(this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsInOrderConstraint<TItem, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor, SortOrder.Ascending, options,
					$" for {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			subject,
			options);
	}
}
#endif
