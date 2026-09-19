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
	///     Verifies that the collection is in ascending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Ascending,
					options,
					"")),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in ascending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInAscendingOrder<
			TItem, TMember>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderConstraint<TItem, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Ascending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in ascending order.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable?>>
		IsInAscendingOrder(this IThat<IEnumerable?> subject)
	{
		CollectionOrderOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<IEnumerable, object?, object?>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Ascending,
					options,
					"")),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in ascending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable?>>
		IsInAscendingOrder<TMember>(
			this IThat<IEnumerable?> subject,
			Func<object?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<IEnumerable, object?, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Ascending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is in ascending order.
	/// </summary>
	public static CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Ascending,
					options,
					"")),
			subject,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is in ascending order.
	/// </summary>
	public static CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInAscendingOrder<
			TItem, TMember>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<ImmutableArray<TItem>, TItem, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Ascending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
			subject,
			options);
	}
#endif

	/// <summary>
	///     Verifies that the collection is not in ascending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Ascending,
					options,
					"").Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in ascending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInAscendingOrder<
			TItem, TMember>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderConstraint<TItem, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Ascending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in ascending order.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable?>>
		IsNotInAscendingOrder(this IThat<IEnumerable?> subject)
	{
		CollectionOrderOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<IEnumerable, object?, object?>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Ascending,
					options,
					"").Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in ascending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable?>>
		IsNotInAscendingOrder<TMember>(
			this IThat<IEnumerable?> subject,
			Func<object?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<IEnumerable, object?, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Ascending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is not in ascending order.
	/// </summary>
	public static CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInAscendingOrder<TItem>(this IThat<ImmutableArray<TItem>> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Ascending,
					options,
					"").Invert()),
			subject,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is not in ascending order.
	/// </summary>
	public static CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInAscendingOrder<TItem, TMember>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<ImmutableArray<TItem>, TItem, TMember>(
					expectationBuilder, it, grammars,
					memberAccessor,
					aweXpect.SortOrder.Ascending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			subject,
			options);
	}
#endif

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
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>>
		IsInAscendingOrder(this IThat<IEnumerable<DateTime?>?> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
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
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
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
	public static CollectionOrderResult<DateTime, IEnumerable<DateTime>, IThat<IEnumerable<DateTime>?>>
		IsNotInAscendingOrder(this IThat<IEnumerable<DateTime>?> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>>
		IsNotInAscendingOrder(this IThat<IEnumerable<DateTime?>?> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInAscendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
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
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<DateTime>, IThat<ImmutableArray<DateTime>>>
		IsInAscendingOrder(this IThat<ImmutableArray<DateTime>> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<DateTime?>, IThat<ImmutableArray<DateTime?>>>
		IsInAscendingOrder(this IThat<ImmutableArray<DateTime?>> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<DateTime>, IThat<ImmutableArray<DateTime>>>
		IsNotInAscendingOrder(this IThat<ImmutableArray<DateTime>> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<DateTime?>, IThat<ImmutableArray<DateTime?>>>
		IsNotInAscendingOrder(this IThat<ImmutableArray<DateTime?>> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Ascending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInAscendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInAscendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Ascending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);
#endif

	private static CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInKindAwareOrder<TItem, TMember>(
			IThat<IEnumerable<TItem>?> subject,
			Func<TItem, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated,
			Func<CollectionOrderOptions<TMember>, Func<Func<TMember, string?>?>> createIncompatibilityCheck)
	{
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
					createIncompatibilityCheck(options));
				return isNegated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	private static CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInKindAwareOrder<TItem, TMember>(
			IThat<ImmutableArray<TItem>> subject,
			Func<TItem, TMember> memberAccessor,
			SortOrder sortOrder,
			string memberExpression,
			bool isNegated,
			Func<CollectionOrderOptions<TMember>, Func<Func<TMember, string?>?>> createIncompatibilityCheck)
	{
		CollectionOrderOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
			{
				IsInOrderForEnumerableConstraint<ImmutableArray<TItem>, TItem, TMember> constraint = new(
					expectationBuilder, it, grammars,
					memberAccessor,
					sortOrder,
					options,
					memberExpression,
					createIncompatibilityCheck(options));
				return isNegated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}
#endif
}
