#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	private const string InDescendingOrder = "Verifies that the collection is in Descending order.";
	private const string NotInDescendingOrder = "Verifies that the collection is not in Descending order.";

	[CreateCollectionExpectation("Is{Not}InDescendingOrder", GuaranteesNotNull = true,
		Summary = InDescendingOrder, NegatedSummary = NotInDescendingOrder)]
	internal static CollectionOrderResult<TItem, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInDescendingOrderCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			bool negated)
		=> IsInOrder(subject, x => x, SortOrder.Descending, "", negated);

	[CreateCollectionExpectation("Is{Not}InDescendingOrder", GuaranteesNotNull = true,
		Summary = InDescendingOrder, NegatedSummary = NotInDescendingOrder)]
	internal static CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInDescendingOrderByMemberCore<TItem, TMember>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			string memberExpression,
			bool negated)
		=> IsInOrder(subject, memberAccessor, SortOrder.Descending,
			$" by {memberExpression.TrimCommonWhiteSpace()}", negated);

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
		=> IsInOrder(subject, x => x, SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<DateTime?>, IThat<IAsyncEnumerable<DateTime?>?>>
		IsInDescendingOrder(this IThat<IAsyncEnumerable<DateTime?>?> subject)
		=> IsInOrder(subject, x => x, SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInDescendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Descending,
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
		=> IsInOrder(subject, memberAccessor, SortOrder.Descending,
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
		=> IsInOrder(subject, x => x, SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<DateTime?>, IThat<IAsyncEnumerable<DateTime?>?>>
		IsNotInDescendingOrder(this IThat<IAsyncEnumerable<DateTime?>?> subject)
		=> IsInOrder(subject, x => x, SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotInDescendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Descending,
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
		=> IsInOrder(subject, memberAccessor, SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);
}
#endif
