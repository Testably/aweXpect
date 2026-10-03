#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection has an item that complies with the <paramref name="expectations" />…
	/// </summary>
	[GuaranteesNotNull]
	public static HasItemResult<IAsyncEnumerable<TItem>> HasItemThat<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject, Action<IThatSubject<TItem>> expectations)
	{
		expectations.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IAsyncEnumerable<TItem>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncHasItemThatConstraint<TItem>(it, grammars, expectations, indexOptions)),
			subject,
			indexOptions);
	}

	/// <summary>
	///     Verifies that the collection does not have an item that complies with the <paramref name="expectations" />…
	/// </summary>
	[GuaranteesNotNull]
	public static HasItemResult<IAsyncEnumerable<TItem>> DoesNotHaveItemThat<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject, Action<IThatSubject<TItem>> expectations)
	{
		expectations.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IAsyncEnumerable<TItem>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncHasItemThatConstraint<TItem>(it, grammars, expectations, indexOptions)
					.Invert()),
			subject,
			indexOptions);
	}
}
#endif
