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
			this IThat<IEnumerable<TItem>?> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"")),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInDescendingOrder<
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
					aweXpect.SortOrder.Descending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable?>>
		IsInDescendingOrder(this IThat<IEnumerable?> subject)
	{
		CollectionOrderOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<IEnumerable, object?, object?>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"")),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable?>>
		IsInDescendingOrder<TMember>(
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
					aweXpect.SortOrder.Descending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	public static CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInDescendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"")),
			subject,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is in descending order.
	/// </summary>
	public static CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInDescendingOrder<
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
					aweXpect.SortOrder.Descending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}")),
			subject,
			options);
	}
#endif

	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInDescendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"").Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInDescendingOrder<
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
					aweXpect.SortOrder.Descending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable?>>
		IsNotInDescendingOrder(this IThat<IEnumerable?> subject)
	{
		CollectionOrderOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<object?, IEnumerable, IThat<IEnumerable?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<IEnumerable, object?, object?>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"").Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionOrderResult<TMember, IEnumerable, IThat<IEnumerable?>>
		IsNotInDescendingOrder<TMember>(
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
					aweXpect.SortOrder.Descending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	public static CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInDescendingOrder<TItem>(this IThat<ImmutableArray<TItem>> subject)
	{
		CollectionOrderOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionOrderResult<TItem, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsInOrderForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(
					expectationBuilder, it, grammars,
					x => x,
					aweXpect.SortOrder.Descending,
					options,
					"").Invert()),
			subject,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is not in descending order.
	/// </summary>
	public static CollectionOrderResult<TMember, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInDescendingOrder<TItem, TMember>(
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
					aweXpect.SortOrder.Descending,
					options,
					$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}").Invert()),
			subject,
			options);
	}
#endif

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
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>>
		IsInDescendingOrder(this IThat<IEnumerable<DateTime?>?> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsInDescendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
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
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
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
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime?, IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>>
		IsNotInDescendingOrder(this IThat<IEnumerable<DateTime?>?> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	[GuaranteesNotNull]
	public static CollectionOrderResult<DateTime, IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		IsNotInDescendingOrder<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
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
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<DateTime>, IThat<ImmutableArray<DateTime>>>
		IsInDescendingOrder(this IThat<ImmutableArray<DateTime>> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<DateTime?>, IThat<ImmutableArray<DateTime?>>>
		IsInDescendingOrder(this IThat<ImmutableArray<DateTime?>> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Descending, "", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInDescendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsInDescendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", false,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<DateTime>, IThat<ImmutableArray<DateTime>>>
		IsNotInDescendingOrder(this IThat<ImmutableArray<DateTime>> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<DateTime?>, IThat<ImmutableArray<DateTime?>>>
		IsNotInDescendingOrder(this IThat<ImmutableArray<DateTime?>> subject)
		=> IsInKindAwareOrder(subject, x => x, aweXpect.SortOrder.Descending, "", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInDescendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);

	/// <inheritdoc cref="IsNotInDescendingOrder(IThat{IEnumerable{DateTime}?})" />
	public static CollectionOrderResult<DateTime?, ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		IsNotInDescendingOrder<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			Func<TItem, DateTime?> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> IsInKindAwareOrder(subject, memberAccessor, aweXpect.SortOrder.Descending,
			$" by {doNotPopulateThisValue.TrimCommonWhiteSpace()}", true,
			DateTimeKindHelpers.CreateIncompatibleKindCheck);
#endif
}
