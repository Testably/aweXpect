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
	///     Verifies that the collection is in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TItem, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInDescendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsInOrderConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x, SortOrder.Descending, options, "")),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInDescendingOrder<
			TItem, TMember>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
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
					memberAccessor, SortOrder.Descending, options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TItem, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotInDescendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsInOrderConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x, SortOrder.Descending, options, "").Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotInDescendingOrder<
			TItem, TMember>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
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
					memberAccessor, SortOrder.Descending, options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	/// <remarks>
	///     Fails when the collection contains both <see cref="DateTimeKind.Utc" /> and <see cref="DateTimeKind.Local" />
	///     values, unless a custom comparer is used.
	/// </remarks>
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<DateTime>, IThat<IAsyncEnumerable<DateTime>?>>
		IsInDescendingOrder(this IThat<IAsyncEnumerable<DateTime>?> subject)
		=> IsInKindAwareOrder(subject, x => x, SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<DateTime?>, IThat<IAsyncEnumerable<DateTime?>?>>
		IsInDescendingOrder(this IThat<IAsyncEnumerable<DateTime?>?> subject)
		=> IsInKindAwareOrder(subject, x => x, SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInDescendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInDescendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	/// <remarks>
	///     Fails when the collection contains both <see cref="DateTimeKind.Utc" /> and <see cref="DateTimeKind.Local" />
	///     values, unless a custom comparer is used.
	/// </remarks>
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<DateTime>, IThat<IAsyncEnumerable<DateTime>?>>
		IsNotInDescendingOrder(this IThat<IAsyncEnumerable<DateTime>?> subject)
		=> IsInKindAwareOrder(subject, x => x, SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<DateTime?>, IThat<IAsyncEnumerable<DateTime?>?>>
		IsNotInDescendingOrder(this IThat<IAsyncEnumerable<DateTime?>?> subject)
		=> IsInKindAwareOrder(subject, x => x, SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotInDescendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotInDescendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);
}
#endif
