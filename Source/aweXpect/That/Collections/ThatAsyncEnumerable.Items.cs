#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     The items of an <see cref="IAsyncEnumerable{TItem}" /> of <typeparamref name="TItem" /> that a quantifier
	///     selected.
	/// </summary>
	internal sealed partial class Items<TItem> : IQuantifiedAsyncEnumerableItems<TItem>
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<IAsyncEnumerable<TItem>?> _subject;

		internal Items(IThat<IAsyncEnumerable<TItem>?> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IAsyncEnumerableItems<TItem>.Quantifier => _quantifier;
		IThat<IAsyncEnumerable<TItem>?> IAsyncEnumerableItems<TItem>.Subject => _subject;
	}
}
#endif
