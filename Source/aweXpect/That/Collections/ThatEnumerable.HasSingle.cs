using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
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
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	[GuaranteesNotNull]
	public static SingleItemResult<IEnumerable<TItem>, TItem> HasSingle<TItem>(
		this IThat<IEnumerable<TItem>?> subject)
	{
		PredicateOptions<TItem> options = new();
		HasSingleConstraint<IEnumerable<TItem>?, TItem> constraint = null!;
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new SingleItemResult<IEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> constraint = new HasSingleConstraint<IEnumerable<TItem>?, TItem>(it, grammars, options)),
			options,
			_ => constraint.SingleItem
		);
	}

	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static SingleItemResult<IEnumerable, object?> HasSingle(
		this IThat<IEnumerable?> subject)
	{
		PredicateOptions<object?> options = new();
		HasSingleConstraint<IEnumerable, object?> constraint = null!;
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new SingleItemResult<IEnumerable, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> constraint = new HasSingleConstraint<IEnumerable, object?>(it, grammars, options)),
			options,
			_ => constraint.SingleItem);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	public static SingleItemResult<ImmutableArray<TItem>, TItem> HasSingle<TItem>(
		this IThat<ImmutableArray<TItem>> subject)
	{
		PredicateOptions<TItem> options = new();
		HasSingleConstraint<ImmutableArray<TItem>, TItem> constraint = null!;
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new SingleItemResult<ImmutableArray<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> constraint = new HasSingleConstraint<ImmutableArray<TItem>, TItem>(it, grammars, options)),
			options,
			_ => constraint.SingleItem);
	}
#endif
}
