#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	[GuaranteesNotNull]
	public static AsyncSingleItemResult<IAsyncEnumerable<TItem>, TItem> HasSingle<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject)
	{
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AsyncSingleItemResult<IAsyncEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint(options, static (predicateOptions, it, grammars) =>
				new AsyncHasSingleConstraint<TItem>(it, grammars, predicateOptions)),
			options,
			async f =>
			{
#pragma warning disable S3267 // net8.0 has no LINQ over IAsyncEnumerable
				await foreach (TItem item in f)
				{
					if (options.Matches(item))
					{
						return item;
					}
				}
#pragma warning restore S3267

				return default;
			});
	}
}
#endif
