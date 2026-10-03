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

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that the collection has an item that complies with the <paramref name="expectations" />…
	/// </summary>
	[GuaranteesNotNull]
	public static HasItemResult<IEnumerable<TItem>> HasItemThat<TItem>(
		this IThat<IEnumerable<TItem>?> subject, Action<IThatSubject<TItem>> expectations)
	{
		expectations.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IEnumerable<TItem>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemThatConstraint<IEnumerable<TItem>?, TItem>(it, grammars, expectations, indexOptions)),
			subject,
			indexOptions);
	}

	/// <summary>
	///     Verifies that the collection has an item that complies with the <paramref name="expectations" />…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static HasItemResult<IEnumerable> HasItemThat(
		this IThat<IEnumerable?> subject, Action<IThatSubject<object?>> expectations)
	{
		expectations.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IEnumerable>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemThatConstraint<IEnumerable, object?>(
					it, grammars, expectations, indexOptions)),
			subject,
			indexOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection has an item that complies with the <paramref name="expectations" />…
	/// </summary>
	public static HasItemResult<ImmutableArray<TItem>> HasItemThat<TItem>(
		this IThat<ImmutableArray<TItem>> subject, Action<IThatSubject<TItem>> expectations)
	{
		expectations.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<ImmutableArray<TItem>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemThatConstraint<ImmutableArray<TItem>, TItem>(
					it, grammars, expectations, indexOptions)),
			subject,
			indexOptions);
	}
#endif

	/// <summary>
	///     Verifies that the collection does not have an item that complies with the <paramref name="expectations" />…
	/// </summary>
	[GuaranteesNotNull]
	public static HasItemResult<IEnumerable<TItem>> DoesNotHaveItemThat<TItem>(
		this IThat<IEnumerable<TItem>?> subject, Action<IThatSubject<TItem>> expectations)
	{
		expectations.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IEnumerable<TItem>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemThatConstraint<IEnumerable<TItem>?, TItem>(it, grammars, expectations, indexOptions)
					.Invert()),
			subject,
			indexOptions);
	}

	/// <summary>
	///     Verifies that the collection does not have an item that complies with the <paramref name="expectations" />…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static HasItemResult<IEnumerable> DoesNotHaveItemThat(
		this IThat<IEnumerable?> subject, Action<IThatSubject<object?>> expectations)
	{
		expectations.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IEnumerable>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemThatConstraint<IEnumerable, object?>(
					it, grammars, expectations, indexOptions).Invert()),
			subject,
			indexOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not have an item that complies with the <paramref name="expectations" />…
	/// </summary>
	public static HasItemResult<ImmutableArray<TItem>> DoesNotHaveItemThat<TItem>(
		this IThat<ImmutableArray<TItem>> subject, Action<IThatSubject<TItem>> expectations)
	{
		expectations.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<ImmutableArray<TItem>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemThatConstraint<ImmutableArray<TItem>, TItem>(
					it, grammars, expectations, indexOptions).Invert()),
			subject,
			indexOptions);
	}
#endif
}
