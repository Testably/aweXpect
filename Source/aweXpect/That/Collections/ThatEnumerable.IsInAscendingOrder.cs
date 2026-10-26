using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
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

	private const string MixedDateTimeKinds =
		"Fails when the collection contains both <see cref=\"System.DateTimeKind.Utc\" /> and <see cref=\"System.DateTimeKind.Local\" /> values, unless a custom comparer is used.";

	[CreateExpectationFamily("Is{Not}InAscendingOrder", GuaranteesNotNull = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsInAscendingOrderCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			bool negated)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", negated);

	[CreateExpectationFamily("Is{Not}InAscendingOrder", GuaranteesNotNull = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember>
		IsInAscendingOrderByMemberCore<TItem, TMember>(
			IThat<IEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			string memberExpression,
			bool negated)
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {memberExpression.TrimCommonWhiteSpace()}", negated);

	[CreateExpectationFamily("Is{Not}InAscendingOrder", GuaranteesNotNull = true, Priority = -1,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder, Remarks = MixedDateTimeKinds)]
	internal static CollectionOrderResult<IEnumerable, IThat<IEnumerable?>, object?>
		IsInAscendingOrderForEnumerableCore(
			IThat<IEnumerable?> subject,
			bool negated)
		=> IsInOrderForEnumerable(subject, x => x, SortOrder.Ascending, "", negated);

	[CreateExpectationFamily("Is{Not}InAscendingOrder", GuaranteesNotNull = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder, Remarks = MixedDateTimeKinds)]
	internal static CollectionOrderResult<IEnumerable, IThat<IEnumerable?>, TMember>
		IsInAscendingOrderForEnumerableByMemberCore<TMember>(
			IThat<IEnumerable?> subject,
			Func<object?, TMember> memberAccessor,
			string memberExpression,
			bool negated)
		=> IsInOrderForEnumerable(subject, memberAccessor, SortOrder.Ascending,
			$" by {memberExpression.TrimCommonWhiteSpace()}", negated);

	[CreateExpectationFamily("Is{Not}InAscendingOrder", PerSubject = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<TCollection, IThat<TCollection>, TItem>
		IsInAscendingOrderForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			bool negated)
		where TCollection : IEnumerable<TItem>
		=> IsInOrderForCollection<TCollection, TItem, TItem>(subject, x => x, SortOrder.Ascending, "",
			negated);

	[CreateExpectationFamily("Is{Not}InAscendingOrder", PerSubject = true,
		Summary = InAscendingOrder, NegatedSummary = NotInAscendingOrder)]
	internal static CollectionOrderResult<TCollection, IThat<TCollection>, TMember>
		IsInAscendingOrderForCollectionByMemberCore<TCollection, TItem, TMember>(
			IThat<TCollection> subject,
			Func<TItem, TMember> memberAccessor,
			string memberExpression,
			bool negated)
		where TCollection : IEnumerable<TItem>
		=> IsInOrderForCollection(subject, memberAccessor, SortOrder.Ascending,
			$" by {memberExpression.TrimCommonWhiteSpace()}", negated);

	/// <summary>
	///     Verifies that the collection is in ascending order.
	/// </summary>
	/// <remarks>
	///     Fails when the collection contains both <see cref="DateTimeKind.Utc" /> and <see cref="DateTimeKind.Local" />
	///     values, unless a custom comparer is used.
	/// </remarks>
	[GuaranteesNotNull]
	public static CollectionOrderResult<IEnumerable<DateTime>, IThat<IEnumerable<DateTime>?>, DateTime>
		IsInAscendingOrder(this IThat<IEnumerable<DateTime>?> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>, DateTime?>
		IsInAscendingOrder(this IThat<IEnumerable<DateTime?>?> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, DateTime>
		IsInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, DateTime?>
		IsInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<ImmutableArray<DateTime>, IThat<ImmutableArray<DateTime>>, DateTime>
		IsInAscendingOrder(this IThat<ImmutableArray<DateTime>> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<ImmutableArray<DateTime?>, IThat<ImmutableArray<DateTime?>>, DateTime?>
		IsInAscendingOrder(this IThat<ImmutableArray<DateTime?>> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, DateTime>
		IsInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, DateTime?>
		IsInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
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
	public static CollectionOrderResult<IEnumerable<DateTime>, IThat<IEnumerable<DateTime>?>, DateTime>
		IsNotInAscendingOrder(this IThat<IEnumerable<DateTime>?> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>, DateTime?>
		IsNotInAscendingOrder(this IThat<IEnumerable<DateTime?>?> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, DateTime>
		IsNotInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, DateTime?>
		IsNotInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<ImmutableArray<DateTime>, IThat<ImmutableArray<DateTime>>, DateTime>
		IsNotInAscendingOrder(this IThat<ImmutableArray<DateTime>> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<ImmutableArray<DateTime?>, IThat<ImmutableArray<DateTime?>>, DateTime?>
		IsNotInAscendingOrder(this IThat<ImmutableArray<DateTime?>> subject)
		=> IsInOrder(subject, x => x, SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, DateTime>
		IsNotInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, DateTime?>
		IsNotInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInOrder(subject, memberAccessor, SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);
#endif

	private static CollectionOrderResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember>
		IsInOrder<TItem, TMember>(
			IThat<IEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated,
			Func<OptionsHolder<CollectionOrderOptions<TMember>>, Func<Func<TMember, string?>?>>? createIncompatibilityCheck = null)
	{
		memberAccessor.ThrowIfNull();
		OptionsHolder<CollectionOrderOptions<TMember>> options = new(CollectionOrderOptions<TMember>.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember>(
			expectationBuilder.AddConstraint(
				(MemberAccessor: memberAccessor, SortOrder: sortOrder, Options: options,
					MemberExpression: memberExpression, CreateIncompatibilityCheck: createIncompatibilityCheck,
					IsNegated: isNegated),
				static (state, it, grammars) =>
				{
					IsInOrderConstraint<IEnumerable<TItem>?, TItem, TMember> constraint = new(
						it, grammars,
						state.MemberAccessor,
						state.SortOrder,
						state.Options,
						state.MemberExpression,
						state.CreateIncompatibilityCheck?.Invoke(state.Options));
					return state.IsNegated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <remarks>
	///     The kind-aware <see cref="DateTime" /> overloads cannot infer the element for the collection helper.
	/// </remarks>
	private static CollectionOrderResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TMember>
		IsInOrder<TItem, TMember>(
			IThat<ImmutableArray<TItem>> subject,
			Func<TItem, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated,
			Func<OptionsHolder<CollectionOrderOptions<TMember>>, Func<Func<TMember, string?>?>>? createIncompatibilityCheck = null)
		=> IsInOrderForCollection(subject, memberAccessor, sortOrder, memberExpression, isNegated,
			createIncompatibilityCheck);
#endif

	private static CollectionOrderResult<IEnumerable, IThat<IEnumerable?>, TMember>
		IsInOrderForEnumerable<TMember>(
			IThat<IEnumerable?> subject,
			Func<object?, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated)
	{
		memberAccessor.ThrowIfNull();
		OptionsHolder<CollectionOrderOptions<TMember>> options = new(CollectionOrderOptions<TMember>.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<IEnumerable, IThat<IEnumerable?>, TMember>(
			expectationBuilder.AddConstraint(
				(MemberAccessor: memberAccessor, SortOrder: sortOrder, Options: options,
					MemberExpression: memberExpression, IsNegated: isNegated),
				static (state, it, grammars) =>
				{
					IsInOrderConstraint<IEnumerable, object?, TMember> constraint = new(
						it, grammars,
						state.MemberAccessor,
						state.SortOrder,
						state.Options,
						state.MemberExpression,
						DateTimeKindHelpers.CreateIncompatibleRuntimeKindCheck(state.Options));
					return state.IsNegated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}

	private static CollectionOrderResult<TCollection, IThat<TCollection>, TMember>
		IsInOrderForCollection<TCollection, TItem, TMember>(
			IThat<TCollection> subject,
			Func<TItem, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated,
			Func<OptionsHolder<CollectionOrderOptions<TMember>>, Func<Func<TMember, string?>?>>? createIncompatibilityCheck = null)
		where TCollection : IEnumerable<TItem>
	{
		memberAccessor.ThrowIfNull();
		OptionsHolder<CollectionOrderOptions<TMember>> options = new(CollectionOrderOptions<TMember>.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TCollection, IThat<TCollection>, TMember>(
			expectationBuilder.AddConstraint(
				(MemberAccessor: memberAccessor, SortOrder: sortOrder, Options: options,
					MemberExpression: memberExpression, CreateIncompatibilityCheck: createIncompatibilityCheck,
					IsNegated: isNegated),
				static (state, it, grammars) =>
				{
					IsInOrderConstraint<TCollection, TItem, TMember> constraint = new(
						it, grammars,
						state.MemberAccessor,
						state.SortOrder,
						state.Options,
						state.MemberExpression,
						state.CreateIncompatibilityCheck?.Invoke(state.Options));
					return state.IsNegated ? constraint.Invert() : constraint;
				}),
			subject,
			options);
	}
}
