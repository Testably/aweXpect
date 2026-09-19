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
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
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
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			subject,
			options);
	}

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
		=> IsInKindAwareOrder(subject, x => x, SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<DateTime?>, IThat<IAsyncEnumerable<DateTime?>?>>
		IsInAscendingOrder(this IThat<IAsyncEnumerable<DateTime?>?> subject)
		=> IsInKindAwareOrder(subject, x => x, SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInAscendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, SortOrder.Ascending,
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
		=> IsInKindAwareOrder(subject, memberAccessor, SortOrder.Ascending,
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
		=> IsInKindAwareOrder(subject, x => x, SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IAsyncEnumerable<DateTime?>, IThat<IAsyncEnumerable<DateTime?>?>>
		IsNotInAscendingOrder(this IThat<IAsyncEnumerable<DateTime?>?> subject)
		=> IsInKindAwareOrder(subject, x => x, SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IAsyncEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotInAscendingOrder<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, SortOrder.Ascending,
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
		=> IsInKindAwareOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	private static CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsInKindAwareOrder<TItem, TMember>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated,
			Func<CollectionOrderOptions<TMember>, Func<Func<TMember, string?>?>> createIncompatibilityCheck)
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
			{
				IsInOrderConstraint<TItem, TMember> constraint = new(
					expectationBuilder, it, grammars,
					memberAccessor, sortOrder, options, memberExpression,
					createIncompatibilityCheck(options));
				return isNegated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}
}
#endif
