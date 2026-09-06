using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that the collection contains the <paramref name="expected" /> value.
	/// </summary>
	public static ObjectCountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			TItem expected)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<TItem>(
					expectationBuilder,
					it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(expected)}{options}"
						: $"{g.Verb("contains", "contain")} {Formatter.Format(expected)}{options} {q}",
					a => options.AreConsideredEqual(a, expected),
					quantifier)),
			source,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection contains the <paramref name="expected" /> value.
	/// </summary>
	public static StringEqualityTypeCountResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> Contains(
		this IThat<IEnumerable<string?>?> source,
		string? expected)
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityTypeCountResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<string?>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(expected)}{options}"
						: $"{g.Verb("contains", "contain")} {Formatter.Format(expected)}{options} {q}",
					a => options.AreConsideredEqual(a, expected),
					quantifier)),
			source,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection contains an item that satisfies the <paramref name="predicate" />.
	/// </summary>
	public static CountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		Contains<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			Func<TItem, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainConstraint<TItem>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}"
						: $"{g.Verb("contains", "contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} {q}",
					predicate,
					quantifier)),
			source,
			quantifier);
	}

	/// <summary>
	///     Verifies that the collection contains the <paramref name="expected" /> value.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectCountResult<IEnumerable, IThat<IEnumerable>, object?>
		Contains(
			this IThat<IEnumerable> source,
			object? expected)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCountResult<IEnumerable, IThat<IEnumerable>, object?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainForEnumerableConstraint<IEnumerable, object?>(
					expectationBuilder,
					it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(expected)}{options}"
						: $"{g.Verb("contains", "contain")} {Formatter.Format(expected)}{options} {q}",
					a => options.AreConsideredEqual(a, expected),
					quantifier)),
			source,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection contains an item that satisfies the <paramref name="predicate" />.
	/// </summary>
	public static CountResult<IEnumerable, IThat<IEnumerable>>
		Contains(
			this IThat<IEnumerable> source,
			Func<object?, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CountResult<IEnumerable, IThat<IEnumerable>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainForEnumerableConstraint<IEnumerable, object?>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}"
						: $"{g.Verb("contains", "contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} {q}",
					predicate,
					quantifier)),
			source,
			quantifier);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection contains the <paramref name="expected" /> value.
	/// </summary>
	public static ObjectCountResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		Contains<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			TItem expected)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCountResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainForEnumerableConstraint<ImmutableArray<TItem>, TItem>(
					expectationBuilder,
					it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(expected)}{options}"
						: $"{g.Verb("contains", "contain")} {Formatter.Format(expected)}{options} {q}",
					a => options.AreConsideredEqual(a, expected),
					quantifier)),
			source,
			quantifier,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection contains the <paramref name="expected" /> value.
	/// </summary>
	public static StringEqualityTypeCountResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>> Contains(
		this IThat<ImmutableArray<string?>> source,
		string? expected)
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityTypeCountResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainForEnumerableConstraint<ImmutableArray<string?>, string?>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(expected)}{options}"
						: $"{g.Verb("contains", "contain")} {Formatter.Format(expected)}{options} {q}",
					a => options.AreConsideredEqual(a, expected),
					quantifier)),
			source,
			quantifier,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection contains an item that satisfies the <paramref name="predicate" />.
	/// </summary>
	public static CountResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		Contains<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			Func<TItem, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CountResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainForEnumerableConstraint<ImmutableArray<TItem>, TItem>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}"
						: $"{g.Verb("contains", "contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} {q}",
					predicate,
					quantifier)),
			source,
			quantifier);
	}
#endif

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectCollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options, matchOptions)),
			source,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringCollectionContainResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		Contains(this IThat<IEnumerable<string?>?> source,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringCollectionContainResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected, options, matchOptions)),
			source,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringCollectionContainResult<string?[], IThat<string?[]?>>
		Contains(this IThat<string?[]?> source,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringCollectionContainResult<string?[], IThat<string?[]?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected, options, matchOptions)),
			source,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectCollectionContainResult<IEnumerable, IThat<IEnumerable>, TItem>
		Contains<TItem>(
			this IThat<IEnumerable> source,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCollectionContainResult<IEnumerable, IThat<IEnumerable>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<IEnumerable, TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions)),
			source,
			options,
			matchOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectCollectionContainResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		Contains<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCollectionContainResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions)),
			source,
			options,
			matchOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringCollectionContainResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		Contains(this IThat<ImmutableArray<string?>> source,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringCollectionContainResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<string?>, string?, string?>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions)),
			source,
			options,
			matchOptions);
	}
#endif

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection of predicates.
	/// </summary>
	public static CollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions)),
			source,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection of expectations.
	/// </summary>
	public static CollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions)),
			source,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	public static ObjectCountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			TItem unexpected)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<TItem>(expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options}"
						: $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options} {q.ToNegatedString()}",
					a => options.AreConsideredEqual(a, unexpected),
					quantifier).Invert()),
			source,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	public static StringEqualityTypeCountResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		DoesNotContain(
			this IThat<IEnumerable<string?>?> source,
			string? unexpected)
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityTypeCountResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<string?>(expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options}"
						: $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options} {q.ToNegatedString()}",
					a => options.AreConsideredEqual(a, unexpected),
					quantifier).Invert()),
			source,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection contains no item that satisfies the <paramref name="predicate" />.
	/// </summary>
	public static CountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
		DoesNotContain<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			Func<TItem, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CountResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainConstraint<TItem>(expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}"
						: $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} {q.ToNegatedString()}",
					predicate,
					quantifier).Invert()),
			source,
			quantifier);
	}

	/// <summary>
	///     Verifies that the collection does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectCountResult<IEnumerable, IThat<IEnumerable>, object?>
		DoesNotContain(
			this IThat<IEnumerable> source,
			object? unexpected)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCountResult<IEnumerable, IThat<IEnumerable>, object?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainForEnumerableConstraint<IEnumerable, object?>(
					expectationBuilder,
					it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options}"
						: $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options} {q.ToNegatedString()}",
					a => options.AreConsideredEqual(a, unexpected),
					quantifier).Invert()),
			source,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection contains no item that satisfies the <paramref name="predicate" />.
	/// </summary>
	public static CountResult<IEnumerable, IThat<IEnumerable>>
		DoesNotContain(
			this IThat<IEnumerable> source,
			Func<object?, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CountResult<IEnumerable, IThat<IEnumerable>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainForEnumerableConstraint<IEnumerable, object?>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}"
						: $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} {q.ToNegatedString()}",
					predicate,
					quantifier).Invert()),
			source,
			quantifier);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	public static ObjectCountResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		DoesNotContain<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			TItem unexpected)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCountResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainForEnumerableConstraint<ImmutableArray<TItem>, TItem>(
					expectationBuilder,
					it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options}"
						: $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options} {q.ToNegatedString()}",
					a => options.AreConsideredEqual(a, unexpected),
					quantifier).Invert()),
			source,
			quantifier,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	public static StringEqualityTypeCountResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>> DoesNotContain(
		this IThat<ImmutableArray<string?>> source,
		string? unexpected)
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityTypeCountResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainForEnumerableConstraint<ImmutableArray<string?>, string?>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options}"
						: $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options} {q.ToNegatedString()}",
					a => options.AreConsideredEqual(a, unexpected),
					quantifier).Invert()),
			source,
			quantifier,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection contains no item that satisfies the <paramref name="predicate" />.
	/// </summary>
	public static CountResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>
		DoesNotContain<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			Func<TItem, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CountResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainForEnumerableConstraint<ImmutableArray<TItem>, TItem>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}"
						: $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} {q.ToNegatedString()}",
					predicate,
					quantifier).Invert()),
			source,
			quantifier);
	}
#endif

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectCollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options, matchOptions).Invert()),
			source,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringCollectionContainResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		DoesNotContain(this IThat<IEnumerable<string?>?> source,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringCollectionContainResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected, options, matchOptions).Invert()),
			source,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringCollectionContainResult<string?[], IThat<string?[]?>>
		DoesNotContain(this IThat<string?[]?> source,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringCollectionContainResult<string?[], IThat<string?[]?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected, options, matchOptions).Invert()),
			source,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectCollectionContainResult<IEnumerable, IThat<IEnumerable>, TItem>
		DoesNotContain<TItem>(
			this IThat<IEnumerable> source,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCollectionContainResult<IEnumerable, IThat<IEnumerable>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<IEnumerable, TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions).Invert()),
			source,
			options,
			matchOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectCollectionContainResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		DoesNotContain<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCollectionContainResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions).Invert()),
			source,
			options,
			matchOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringCollectionContainResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		DoesNotContain(this IThat<ImmutableArray<string?>> source,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringCollectionContainResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<string?>, string?, string?>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions).Invert()),
			source,
			options,
			matchOptions);
	}
#endif

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection of predicates.
	/// </summary>
	public static CollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			IEnumerable<Expression<Func<TItem, bool>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					matchOptions).Invert()),
			source,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection of expectations.
	/// </summary>
	public static CollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			IEnumerable<Action<IThatSubject<TItem?>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionContainResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					matchOptions).Invert()),
			source,
			matchOptions);
	}

	private sealed class ContainConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		Quantifier quantifier)
		: ConstraintResult(grammars),
			IContextConstraint<IEnumerable<TItem>?>
	{
		private IEnumerable<TItem>? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;
		private IEnumerable<TItem>? _materializedEnumerable;

		public ConstraintResult IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_materializedEnumerable =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			_count = 0;
			foreach (TItem _ in _materializedEnumerable.Where(predicate))
			{
				_count++;
				bool? check = quantifier.Check(_count, false);
				switch (check)
				{
					case false:
						Outcome = Outcome.Failure;
						expectationBuilder.AddCollectionContext(_materializedEnumerable);
						return this;
					case true:
						Outcome = Outcome.Success;
						return this;
				}
			}

			expectationBuilder.AddCollectionContext(_materializedEnumerable);
			if (quantifier.Check(_count, true) ?? _isNegated)
			{
				Outcome = Outcome.Success;
				return this;
			}

			_isFinished = true;
			Outcome = Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectationText.Invoke(quantifier, Grammars));

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual == null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_isFinished)
			{
				if (_count == 0)
				{
					stringBuilder.Append(it).Append(" did not contain it");
				}
				else if (_count == 1)
				{
					stringBuilder.Append(it).Append(" contained it once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append(it).Append(" contained it twice");
				}
				else
				{
					stringBuilder.Append(it).Append(" contained it ").Append(_count).Append(" times");
				}
			}
			else
			{
				stringBuilder.Append(it).Append(" contained it at least ");
				if (_count == 1)
				{
					stringBuilder.Append("once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append("twice");
				}
				else
				{
					stringBuilder.Append(_count).Append(" times");
				}
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(IEnumerable<TItem>));
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			quantifier.Negate();
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}

	private sealed class AsyncContainConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
#if NET8_0_OR_GREATER
		Func<TItem, ValueTask<bool>> predicate,
#else
		Func<TItem, Task<bool>> predicate,
#endif
		Quantifier quantifier)
		: ConstraintResult(grammars),
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private IEnumerable<TItem>? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;
		private IEnumerable<TItem>? _materializedEnumerable;

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_materializedEnumerable =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			_count = 0;
			foreach (TItem item in _materializedEnumerable)
			{
				if (!await predicate(item))
				{
					continue;
				}

				_count++;
				bool? check = quantifier.Check(_count, false);
				switch (check)
				{
					case false:
						Outcome = Outcome.Failure;
						expectationBuilder.AddCollectionContext(_materializedEnumerable);
						return this;
					case true:
						Outcome = Outcome.Success;
						return this;
				}
			}

			expectationBuilder.AddCollectionContext(_materializedEnumerable);
			if (quantifier.Check(_count, true) ?? _isNegated)
			{
				Outcome = Outcome.Success;
				return this;
			}

			_isFinished = true;
			Outcome = Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectationText.Invoke(quantifier, Grammars));

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual == null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_isFinished)
			{
				if (_count == 0)
				{
					stringBuilder.Append(it).Append(" did not contain it");
				}
				else if (_count == 1)
				{
					stringBuilder.Append(it).Append(" contained it once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append(it).Append(" contained it twice");
				}
				else
				{
					stringBuilder.Append(it).Append(" contained it ").Append(_count).Append(" times");
				}
			}
			else
			{
				stringBuilder.Append(it).Append(" contained it at least ");
				if (_count == 1)
				{
					stringBuilder.Append("once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append("twice");
				}
				else
				{
					stringBuilder.Append(_count).Append(" times");
				}
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(IEnumerable<TItem>));
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			quantifier.Negate();
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}

	private sealed class ContainForEnumerableConstraint<TEnumerable, TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		Quantifier quantifier)
		: ConstraintResult(grammars),
			IContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable
	{
		private IEnumerable? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;
		private IEnumerable? _materializedEnumerable;

		public ConstraintResult IsMetBy(TEnumerable? actual, IEvaluationContext context)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_materializedEnumerable = context.UseMaterializedEnumerable(actual);
			_count = 0;
			foreach (object? item in _materializedEnumerable)
			{
				if (item is TItem typedItem && predicate(typedItem))
				{
					_count++;
					bool? check = quantifier.Check(_count, false);
					switch (check)
					{
						case false:
							Outcome = Outcome.Failure;
							expectationBuilder.AddCollectionContext(_materializedEnumerable);
							return this;
						case true:
							Outcome = Outcome.Success;
							return this;
					}
				}
			}

			expectationBuilder.AddCollectionContext(_materializedEnumerable);
			if (quantifier.Check(_count, true) ?? _isNegated)
			{
				Outcome = Outcome.Success;
				return this;
			}

			_isFinished = true;
			Outcome = Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectationText.Invoke(quantifier, Grammars));

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual == null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_isFinished)
			{
				if (_count == 0)
				{
					stringBuilder.Append(it).Append(" did not contain it");
				}
				else if (_count == 1)
				{
					stringBuilder.Append(it).Append(" contained it once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append(it).Append(" contained it twice");
				}
				else
				{
					stringBuilder.Append(it).Append(" contained it ").Append(_count).Append(" times");
				}
			}
			else
			{
				stringBuilder.Append(it).Append(" contained it at least ");
				if (_count == 1)
				{
					stringBuilder.Append("once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append("twice");
				}
				else
				{
					stringBuilder.Append(_count).Append(" times");
				}
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(IEnumerable<TItem>));
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			quantifier.Negate();
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}

	private sealed class AsyncContainForEnumerableConstraint<TEnumerable, TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
#if NET8_0_OR_GREATER
		Func<TItem, ValueTask<bool>> predicate,
#else
		Func<TItem, Task<bool>> predicate,
#endif
		Quantifier quantifier)
		: ConstraintResult(grammars),
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable
	{
		private IEnumerable? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;
		private IEnumerable? _materializedEnumerable;

		public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_materializedEnumerable = context.UseMaterializedEnumerable(actual);
			_count = 0;
			foreach (object? item in _materializedEnumerable)
			{
				if (item is TItem typedItem && await predicate(typedItem))
				{
					_count++;
					bool? check = quantifier.Check(_count, false);
					switch (check)
					{
						case false:
							Outcome = Outcome.Failure;
							expectationBuilder.AddCollectionContext(_materializedEnumerable);
							return this;
						case true:
							Outcome = Outcome.Success;
							return this;
					}
				}
			}

			expectationBuilder.AddCollectionContext(_materializedEnumerable);
			if (quantifier.Check(_count, true) ?? _isNegated)
			{
				Outcome = Outcome.Success;
				return this;
			}

			_isFinished = true;
			Outcome = Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectationText.Invoke(quantifier, Grammars));

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual == null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_isFinished)
			{
				if (_count == 0)
				{
					stringBuilder.Append(it).Append(" did not contain it");
				}
				else if (_count == 1)
				{
					stringBuilder.Append(it).Append(" contained it once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append(it).Append(" contained it twice");
				}
				else
				{
					stringBuilder.Append(it).Append(" contained it ").Append(_count).Append(" times");
				}
			}
			else
			{
				stringBuilder.Append(it).Append(" contained it at least ");
				if (_count == 1)
				{
					stringBuilder.Append("once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append("twice");
				}
				else
				{
					stringBuilder.Append(_count).Append(" times");
				}
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(IEnumerable<TItem>));
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			quantifier.Negate();
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}
}
