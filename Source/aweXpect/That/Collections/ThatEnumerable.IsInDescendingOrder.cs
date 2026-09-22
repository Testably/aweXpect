using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect;

public static partial class ThatEnumerable
{
	private const string InDescendingOrder = "Verifies that the collection is in Descending order.";
	private const string NotInDescendingOrder = "Verifies that the collection is not in Descending order.";

	[CreateCollectionExpectation("Is{Not}InDescendingOrder", GuaranteesNotNull = true,
		Summary = InDescendingOrder, NegatedSummary = NotInDescendingOrder)]
	internal static CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInDescendingOrderCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			bool negated)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Descending, "", negated);

	[CreateCollectionExpectation("Is{Not}InDescendingOrder", GuaranteesNotNull = true,
		Summary = InDescendingOrder, NegatedSummary = NotInDescendingOrder)]
	internal static CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInDescendingOrderByMemberCore<TItem, TMember>(
			IThat<IEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			string memberExpression,
			bool negated)
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {memberExpression.TrimCommonWhiteSpace()}", negated);

	[CreateCollectionExpectation("Is{Not}InDescendingOrder", GuaranteesNotNull = true, Priority = -1,
		Summary = InDescendingOrder, NegatedSummary = NotInDescendingOrder)]
	internal static CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable?>>
		IsInDescendingOrderForEnumerableCore(
			IThat<IEnumerable?> subject,
			bool negated)
		=> IsInOrderForEnumerable(subject, x => x, aweXpect.SortOrder.Descending, "", negated);

	[CreateCollectionExpectation("Is{Not}InDescendingOrder", GuaranteesNotNull = true,
		Summary = InDescendingOrder, NegatedSummary = NotInDescendingOrder)]
	internal static CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable?>>
		IsInDescendingOrderForEnumerableByMemberCore<TMember>(
			IThat<IEnumerable?> subject,
			Func<object?, TMember> memberAccessor,
			string memberExpression,
			bool negated)
		=> IsInOrderForEnumerable(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {memberExpression.TrimCommonWhiteSpace()}", negated);

	[CreateCollectionExpectation("Is{Not}InDescendingOrder", PerSubject = true,
		Summary = InDescendingOrder, NegatedSummary = NotInDescendingOrder)]
	internal static CollectionOrderResult<TItem, TCollection, IThat<TCollection>>
		IsInDescendingOrderForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			bool negated)
		where TCollection : IEnumerable<TItem>
		=> IsInOrderForCollection<TCollection, TItem, TItem>(subject, x => x, aweXpect.SortOrder.Descending, "",
			negated);

	[CreateCollectionExpectation("Is{Not}InDescendingOrder", PerSubject = true,
		Summary = InDescendingOrder, NegatedSummary = NotInDescendingOrder)]
	internal static CollectionOrderResult<TMember, TCollection, IThat<TCollection>>
		IsInDescendingOrderForCollectionByMemberCore<TCollection, TItem, TMember>(
			IThat<TCollection> subject,
			Func<TItem, TMember> memberAccessor,
			string memberExpression,
			bool negated)
		where TCollection : IEnumerable<TItem>
		=> IsInOrderForCollection(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {memberExpression.TrimCommonWhiteSpace()}", negated);

	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	/// <remarks>
	///     Fails when the collection contains both <see cref="DateTimeKind.Utc" /> and <see cref="DateTimeKind.Local" />
	///     values, unless a custom comparer is used.
	/// </remarks>
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IEnumerable<DateTime>, IThat<IEnumerable<DateTime>?>>
		IsInDescendingOrder(this IThat<IEnumerable<DateTime>?> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>>
		IsInDescendingOrder(this IThat<IEnumerable<DateTime?>?> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInDescendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInDescendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
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
	public static CollectionOrderResult<DateTime, IEnumerable<DateTime>, IThat<IEnumerable<DateTime>?>>
		IsNotInDescendingOrder(this IThat<IEnumerable<DateTime>?> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>>
		IsNotInDescendingOrder(this IThat<IEnumerable<DateTime?>?> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInDescendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInDescendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<DateTime>, IThat<ImmutableArray<DateTime>>>
		IsInDescendingOrder(this IThat<ImmutableArray<DateTime>> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<DateTime?>, IThat<ImmutableArray<DateTime?>>>
		IsInDescendingOrder(this IThat<ImmutableArray<DateTime?>> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInDescendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInDescendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<DateTime>, IThat<ImmutableArray<DateTime>>>
		IsNotInDescendingOrder(this IThat<ImmutableArray<DateTime>> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<DateTime?>, IThat<ImmutableArray<DateTime?>>>
		IsNotInDescendingOrder(this IThat<ImmutableArray<DateTime?>> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInDescendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInDescendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);
#endif
}
