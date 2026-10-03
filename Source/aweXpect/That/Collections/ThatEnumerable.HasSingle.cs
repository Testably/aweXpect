using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new SingleItemResult<IEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint(options, static (predicateOptions, it, grammars)
				=> new HasSingleConstraint<IEnumerable<TItem>?, TItem>(it, grammars, predicateOptions)),
			options,
			f => f.FirstOrDefault(item => options.Matches(item))
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
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new SingleItemResult<IEnumerable, object?>(
			expectationBuilder.AddConstraint(options, static (predicateOptions, it, grammars)
				=> new HasSingleConstraint<IEnumerable, object?>(it, grammars,
					predicateOptions)),
			options,
			f =>
			{
				return f.Cast<object?>().FirstOrDefault(item => options.Matches(item));
			});
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	public static SingleItemResult<ImmutableArray<TItem>, TItem> HasSingle<TItem>(
		this IThat<ImmutableArray<TItem>> subject)
	{
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new SingleItemResult<ImmutableArray<TItem>, TItem>(
			expectationBuilder.AddConstraint(options, static (predicateOptions, it, grammars)
				=> new HasSingleConstraint<ImmutableArray<TItem>, TItem>(it, grammars,
					predicateOptions)),
			options,
			f =>
			{
				return f.FirstOrDefault(item => options.Matches(item));
			});
	}
#endif
}
