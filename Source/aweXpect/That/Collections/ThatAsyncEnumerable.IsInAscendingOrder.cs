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
	private const string InAscendingOrder = "Verifies that the collection is in ascending order.";
	private const string NotInAscendingOrder = "Verifies that the collection is not in ascending order.";

	[CreateCollectionExpectation("Is{Not}InAscendingOrder", GuaranteesNotNull = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<TItem, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInAscendingOrderCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			bool negated)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", negated);

	[CreateCollectionExpectation("Is{Not}InAscendingOrder", GuaranteesNotNull = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInAscendingOrderByMemberCore<TItem, TMember>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			string memberExpression,
			bool negated)
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {memberExpression.TrimCommonWhiteSpace()}", negated);

	/// <summary>
	///     Verifies that the collection is in ascending order.
	/// </summary>
	/// <remarks>
	///     Fails when the collection contains both <see cref="DateTimeKind.Utc" /> and <see cref="DateTimeKind.Local" />
	///     values, unless a custom comparer is used.
	/// </remarks>
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<DateTime>, IThat<IAsyncEnumerable<DateTime>?>>
		IsInAscendingOrder(this IThat<IAsyncEnumerable<DateTime>?> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<DateTime?>, IThat<IAsyncEnumerable<DateTime?>?>>
		IsInAscendingOrder(this IThat<IAsyncEnumerable<DateTime?>?> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInAscendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInAscendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <summary>
	///     Verifies that the collection is not in ascending order.
	/// </summary>
	/// <remarks>
	///     Fails when the collection contains both <see cref="DateTimeKind.Utc" /> and <see cref="DateTimeKind.Local" />
	///     values, unless a custom comparer is used.
	/// </remarks>
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<DateTime>, IThat<IAsyncEnumerable<DateTime>?>>
		IsNotInAscendingOrder(this IThat<IAsyncEnumerable<DateTime>?> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<DateTime?>, IThat<IAsyncEnumerable<DateTime?>?>>
		IsNotInAscendingOrder(this IThat<IAsyncEnumerable<DateTime?>?> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotInAscendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotInAscendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	private static CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInOrder<TItem, TMember>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated,
			Func<CollectionOrderOptions<TMember>, Func<Func<TMember, string?>?>>? createIncompatibilityCheck = null)
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
			{
				IsInOrderConstraint<TItem, TMember> constraint = new(
					expectationBuilder, it, grammars,
					memberAccessor, sortOrder, options, memberExpression,
					createIncompatibilityCheck?.Invoke(options));
				return isNegated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}
}
#endif
