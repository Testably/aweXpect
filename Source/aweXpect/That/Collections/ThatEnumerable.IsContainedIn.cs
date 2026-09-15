using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
	///     Verifies that the collection is contained in the provided <paramref name="expected" /> collection.
	/// </summary>
	[GuaranteesNotNull]
	public static ObjectProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsContainedIn<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
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
	[GuaranteesNotNull]
	public static StringProperCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		IsContainedIn(this IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
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
	///     Verifies that the collection is contained in the provided <paramref name="expected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>
		IsContainedIn<TItem>(
			this IThat<IEnumerable?> subject,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<IEnumerable, TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions, failsForNullSubject: true)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is contained in the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectProperCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		IsContainedIn<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions, failsForNullSubject: true)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is contained in the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringProperCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		IsContainedIn(this IThat<ImmutableArray<string?>> subject,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<string?>, string?, string?>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions, failsForNullSubject: true)),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}
#endif

	/// <summary>
	///     Verifies that the collection is contained in the provided <paramref name="expected" /> collection of predicates.
	/// </summary>
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsContainedIn<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
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
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsContainedIn<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
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
	[GuaranteesNotNull]
	public static ObjectProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsNotContainedIn<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNull();
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
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
	[GuaranteesNotNull]
	public static StringProperCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		IsNotContainedIn(this IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNull();
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
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
	///     Verifies that the collection is not contained in the provided <paramref name="unexpected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>
		IsNotContainedIn<TItem>(
			this IThat<IEnumerable?> subject,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNull();
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<IEnumerable, TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions, failsForNullSubject: true).Invert()),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is not contained in the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectProperCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		IsNotContainedIn<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNull();
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectProperCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions, failsForNullSubject: true).Invert()),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection is not contained in the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringProperCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		IsNotContainedIn(this IThat<ImmutableArray<string?>> subject,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		unexpected.ThrowIfNull();
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringProperCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<string?>, string?, string?>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions, failsForNullSubject: true).Invert()),
			subject,
			options,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}
#endif

	/// <summary>
	///     Verifies that the collection is not contained in the provided <paramref name="expected" /> collection of predicates.
	/// </summary>
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsNotContainedIn<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions, failsForNullSubject: true).Invert()),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}

	/// <summary>
	///     Verifies that the collection is not contained in the provided <paramref name="expected" /> collection of expectations.
	/// </summary>
	[GuaranteesNotNull]
	public static ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsNotContainedIn<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		expected.ThrowIfNull();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ProperCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions, failsForNullSubject: true).Invert()),
			subject,
			matchOptions,
			CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly);
	}
}
