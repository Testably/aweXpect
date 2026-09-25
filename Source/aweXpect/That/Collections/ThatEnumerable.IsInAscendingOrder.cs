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
	private const string InAscendingOrder = "Verifies that the collection is in ascending order.";
	private const string NotInAscendingOrder = "Verifies that the collection is not in ascending order.";

	[CreateCollectionExpectation("Is{Not}InAscendingOrder", GuaranteesNotNull = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInAscendingOrderCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			bool negated)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", negated);

	[CreateCollectionExpectation("Is{Not}InAscendingOrder", GuaranteesNotNull = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInAscendingOrderByMemberCore<TItem, TMember>(
			IThat<IEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			string memberExpression,
			bool negated)
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {memberExpression.TrimCommonWhiteSpace()}", negated);

	[CreateCollectionExpectation("Is{Not}InAscendingOrder", GuaranteesNotNull = true, Priority = -1,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable?>>
		IsInAscendingOrderForEnumerableCore(
			IThat<IEnumerable?> subject,
			bool negated)
		=> IsInOrderForEnumerable(subject, x => x, aweXpect.SortOrder.Ascending, "", negated);

	[CreateCollectionExpectation("Is{Not}InAscendingOrder", GuaranteesNotNull = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable?>>
		IsInAscendingOrderForEnumerableByMemberCore<TMember>(
			IThat<IEnumerable?> subject,
			Func<object?, TMember> memberAccessor,
			string memberExpression,
			bool negated)
		=> IsInOrderForEnumerable(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {memberExpression.TrimCommonWhiteSpace()}", negated);

	[CreateCollectionExpectation("Is{Not}InAscendingOrder", PerSubject = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<TItem, TCollection, IThat<TCollection>>
		IsInAscendingOrderForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			bool negated)
		where TCollection : IEnumerable<TItem>
		=> IsInOrderForCollection<TCollection, TItem, TItem>(subject, x => x, aweXpect.SortOrder.Ascending, "",
			negated);

	[CreateCollectionExpectation("Is{Not}InAscendingOrder", PerSubject = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<TMember, TCollection, IThat<TCollection>>
		IsInAscendingOrderForCollectionByMemberCore<TCollection, TItem, TMember>(
			IThat<TCollection> subject,
			Func<TItem, TMember> memberAccessor,
			string memberExpression,
			bool negated)
		where TCollection : IEnumerable<TItem>
		=> IsInOrderForCollection(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {memberExpression.TrimCommonWhiteSpace()}", negated);

	/// <summary>
	///     Verifies that the collection is in ascending order.
	/// </summary>
	/// <remarks>
	///     Fails when the collection contains both <see cref="DateTimeKind.Utc" /> and <see cref="DateTimeKind.Local" />
	///     values, unless a custom comparer is used.
	/// </remarks>
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IEnumerable<DateTime>, IThat<IEnumerable<DateTime>?>>
		IsInAscendingOrder(this IThat<IEnumerable<DateTime>?> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>>
		IsInAscendingOrder(this IThat<IEnumerable<DateTime?>?> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<DateTime>, IThat<ImmutableArray<DateTime>>>
		IsInAscendingOrder(this IThat<ImmutableArray<DateTime>> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<DateTime?>, IThat<ImmutableArray<DateTime?>>>
		IsInAscendingOrder(this IThat<ImmutableArray<DateTime?>> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);
#endif

	/// <summary>
	///     Verifies that the collection is not in ascending order.
	/// </summary>
	/// <remarks>
	///     Fails when the collection contains both <see cref="DateTimeKind.Utc" /> and <see cref="DateTimeKind.Local" />
	///     values, unless a custom comparer is used.
	/// </remarks>
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IEnumerable<DateTime>, IThat<IEnumerable<DateTime>?>>
		IsNotInAscendingOrder(this IThat<IEnumerable<DateTime>?> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>>
		IsNotInAscendingOrder(this IThat<IEnumerable<DateTime?>?> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<DateTime>, IThat<ImmutableArray<DateTime>>>
		IsNotInAscendingOrder(this IThat<ImmutableArray<DateTime>> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<DateTime?>, IThat<ImmutableArray<DateTime?>>>
		IsNotInAscendingOrder(this IThat<ImmutableArray<DateTime?>> subject)
		=> IsInOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);
#endif

	private static CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInOrder<TItem, TMember>(
			IThat<IEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated,
			Func<CollectionOrderOptions<TMember>, Func<Func<TMember, string?>?>>? createIncompatibilityCheck = null)
	{
		memberAccessor.ThrowIfNull();
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
			{
				IsInOrderConstraint<TItem, TMember> constraint = new(
					expectationBuilder, it, grammars,
					memberAccessor,
					sortOrder,
					options,
					memberExpression,
					createIncompatibilityCheck?.Invoke(options));
				return isNegated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <remarks>
	///     The kind-aware <see cref="DateTime" /> overloads cannot infer the element for the collection helper.
	/// </remarks>
	private static CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInOrder<TItem, TMember>(
			IThat<ImmutableArray<TItem>> subject,
			Func<TItem, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated,
			Func<CollectionOrderOptions<TMember>, Func<Func<TMember, string?>?>>? createIncompatibilityCheck = null)
		=> IsInOrderForCollection(subject, memberAccessor, sortOrder, memberExpression, isNegated,
			createIncompatibilityCheck);
#endif

	private static CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable?>>
		IsInOrderForEnumerable<TMember>(
			IThat<IEnumerable?> subject,
			Func<object?, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated)
	{
		memberAccessor.ThrowIfNull();
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
			{
				IsInOrderForEnumerableConstraint<IEnumerable, object?, TMember> constraint = new(
					expectationBuilder, it, grammars,
					memberAccessor,
					sortOrder,
					options,
					memberExpression);
				return isNegated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	private static CollectionOrderResult<TMember, TCollection, IThat<TCollection>>
		IsInOrderForCollection<TCollection, TItem, TMember>(
			IThat<TCollection> subject,
			Func<TItem, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated,
			Func<CollectionOrderOptions<TMember>, Func<Func<TMember, string?>?>>? createIncompatibilityCheck = null)
		where TCollection : IEnumerable<TItem>
	{
		memberAccessor.ThrowIfNull();
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint((it, grammars) =>
			{
				IsInOrderForEnumerableConstraint<TCollection, TItem, TMember> constraint = new(
					expectationBuilder, it, grammars,
					memberAccessor,
					sortOrder,
					options,
					memberExpression,
					createIncompatibilityCheck?.Invoke(options));
				return isNegated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}
}
