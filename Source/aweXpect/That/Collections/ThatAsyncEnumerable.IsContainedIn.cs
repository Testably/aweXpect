#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection is contained in the provided <paramref name="expected" /> collection.
	/// </summary>
	/// <remarks>
	///     The subject items must appear in the expected collection in the same order and contiguous, i.e. without
	///     other items in between. Use <c>IgnoringInterspersedItems()</c> to allow other items in between or
	///     <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsContainedIn<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new
			ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars) =>
					new IsEqualToConstraint<TItem, TItem>(expectationBuilder, it, grammars,
						doNotPopulateThisValue.TrimCommonWhiteSpace(),
						expected,
						options,
						matchOptions, failsForNullSubject: true)),
				subject,
				options,
				matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	/// <summary>
	///     Verifies that the collection is contained in the provided <paramref name="expected" /> collection.
	/// </summary>
	/// <remarks>
	///     The subject items must appear in the expected collection in the same order and contiguous, i.e. without
	///     other items in between. Use <c>IgnoringInterspersedItems()</c> to allow other items in between or
	///     <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringProperCollectionMatchResult<IAsyncEnumerable<string?>,
			IThat<IAsyncEnumerable<string?>?>>
		IsContainedIn(
			this IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		StringEqualityOptions options = new(nameof(expected));
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<IAsyncEnumerable<string?>,
			IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions, failsForNullSubject: true)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	/// <summary>
	///     Verifies that the collection is contained in the provided <paramref name="expected" /> collection of predicates.
	/// </summary>
	/// <remarks>
	///     The subject items must satisfy the expected predicates in the same order and contiguous, i.e. without other
	///     predicates in between. Use <c>IgnoringInterspersedItems()</c> to allow other predicates in between or
	///     <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsContainedIn<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			[CallerArgumentExpression("expected")]
			string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions, failsForNullSubject: true)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	/// <summary>
	///     Verifies that the collection is contained in the provided <paramref name="expected" /> collection of expectations.
	/// </summary>
	/// <remarks>
	///     The subject items must satisfy the expectations in the same order and contiguous, i.e. without other
	///     expectations in between. Use <c>IgnoringInterspersedItems()</c> to allow other expectations in between or
	///     <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsContainedIn<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			[CallerArgumentExpression("expected")]
			string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions, failsForNullSubject: true)),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	/// <summary>
	///     Verifies that the collection is not contained in the provided <paramref name="unexpected" /> collection.
	/// </summary>
	/// <remarks>
	///     The subject is only considered contained when its items appear in the unexpected collection in the same order
	///     and contiguous, i.e. without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider it
	///     contained with other items in between or <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsNotContainedIn<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")] string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNull();
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new
			ObjectProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars) =>
					new IsEqualToConstraint<TItem, TItem>(expectationBuilder, it, grammars,
						doNotPopulateThisValue.TrimCommonWhiteSpace(),
						unexpected,
						options,
						matchOptions, failsForNullSubject: true).Invert()),
				subject,
				options,
				matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	/// <summary>
	///     Verifies that the collection is not contained in the provided <paramref name="unexpected" /> collection.
	/// </summary>
	/// <remarks>
	///     The subject is only considered contained when its items appear in the unexpected collection in the same order
	///     and contiguous, i.e. without other items in between. Use <c>IgnoringInterspersedItems()</c> to also consider it
	///     contained with other items in between or <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringProperCollectionMatchResult<IAsyncEnumerable<string?>,
			IThat<IAsyncEnumerable<string?>?>>
		IsNotContainedIn(
			this IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")] string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNull();
		StringEqualityOptions options = new(nameof(unexpected));
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<IAsyncEnumerable<string?>,
			IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions, failsForNullSubject: true).Invert()),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	/// <summary>
	///     Verifies that the collection is not contained in the provided <paramref name="unexpected" /> collection of predicates.
	/// </summary>
	/// <remarks>
	///     The subject is only considered contained when its items satisfy the expected predicates in the same order and
	///     contiguous, i.e. without other predicates in between. Use <c>IgnoringInterspersedItems()</c> to also consider it
	///     contained with other predicates in between or <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsNotContainedIn<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNull();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					matchOptions, failsForNullSubject: true).Invert()),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	/// <summary>
	///     Verifies that the collection is not contained in the provided <paramref name="unexpected" /> collection of expectations.
	/// </summary>
	/// <remarks>
	///     The subject is only considered contained when its items satisfy the expectations in the same order and
	///     contiguous, i.e. without other expectations in between. Use <c>IgnoringInterspersedItems()</c> to also consider
	///     it contained with other expectations in between or <c>InAnyOrder()</c> to also ignore the order.
	/// </remarks>
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsNotContainedIn<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNull();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					matchOptions, failsForNullSubject: true).Invert()),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}
}
#endif
