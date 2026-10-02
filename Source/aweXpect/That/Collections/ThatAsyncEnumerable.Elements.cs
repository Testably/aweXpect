#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Result class for expectations on the elements of a <see cref="IAsyncEnumerable{T}" /> of <see langword="string" />.
	/// </summary>
	public partial class Elements : IAsyncEnumerableStringElements
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<IAsyncEnumerable<string?>?> _subject;

		internal Elements(IThat<IAsyncEnumerable<string?>?> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IAsyncEnumerableStringElements.Quantifier => _quantifier;
		IThat<IAsyncEnumerable<string?>?> IAsyncEnumerableStringElements.Subject => _subject;
	}

	/// <summary>
	///     Result class for expectations on the elements of a <see cref="IAsyncEnumerable{TItem}" /> of
	///     <typeparamref name="TItem" />.
	/// </summary>
	public partial class Elements<TItem> : IAsyncEnumerableElements<TItem>
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<IAsyncEnumerable<TItem>?> _subject;

		internal Elements(IThat<IAsyncEnumerable<TItem>?> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IAsyncEnumerableElements<TItem>.Quantifier => _quantifier;
		IThat<IAsyncEnumerable<TItem>?> IAsyncEnumerableElements<TItem>.Subject => _subject;
	}
}
#endif
