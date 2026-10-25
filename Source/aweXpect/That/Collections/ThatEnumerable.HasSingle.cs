using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
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
		OptionsSlot<PredicateOptions<TItem>> options = new(new PredicateOptions<TItem>());
		SingleItemSource<TItem> source = new();
		return new SingleItemResult<IEnumerable<TItem>, TItem>(
			subject.Get().ExpectationBuilder.AddConstraint((Options: options, Source: source),
				static (state, it, grammars) => state.Source.Use(
					new HasSingleConstraint<IEnumerable<TItem>?, TItem>(it, grammars, state.Options))),
			options,
			source);
	}

	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static SingleItemResult<IEnumerable, object?> HasSingle(
		this IThat<IEnumerable?> subject)
	{
		OptionsSlot<PredicateOptions<object?>> options = new(new PredicateOptions<object?>());
		SingleItemSource<object?> source = new();
		return new SingleItemResult<IEnumerable, object?>(
			subject.Get().ExpectationBuilder.AddConstraint((Options: options, Source: source),
				static (state, it, grammars) => state.Source.Use(
					new HasSingleConstraint<IEnumerable, object?>(it, grammars, state.Options))),
			options,
			source);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	public static SingleItemResult<ImmutableArray<TItem>, TItem> HasSingle<TItem>(
		this IThat<ImmutableArray<TItem>> subject)
	{
		OptionsSlot<PredicateOptions<TItem>> options = new(new PredicateOptions<TItem>());
		SingleItemSource<TItem> source = new();
		return new SingleItemResult<ImmutableArray<TItem>, TItem>(
			subject.Get().ExpectationBuilder.AddConstraint((Options: options, Source: source),
				static (state, it, grammars) => state.Source.Use(
					new HasSingleConstraint<ImmutableArray<TItem>, TItem>(it, grammars, state.Options))),
			options,
			source);
	}
#endif
}
