using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     The items of an <see cref="IEnumerable{TItem}" /> of <typeparamref name="TItem" /> that a quantifier selected.
	/// </summary>
	internal sealed partial class Items<TItem> : IQuantifiedEnumerableItems<TItem>
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<IEnumerable<TItem>?> _subject;

		internal Items(IThat<IEnumerable<TItem>?> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IEnumerableItems<TItem>.Quantifier => _quantifier;
		IThat<IEnumerable<TItem>?> IEnumerableItems<TItem>.Subject => _subject;
	}

	/// <summary>
	///     The items of a struct collection of <typeparamref name="TItem" /> that a quantifier selected.
	/// </summary>
	internal sealed partial class
		StructItems<TCollection, TItem> : IStructEnumerableItems<TCollection, TItem>
		where TCollection : struct, IEnumerable<TItem>
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<TCollection> _subject;

		internal StructItems(IThat<TCollection> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IStructEnumerableItems<TCollection, TItem>.Quantifier => _quantifier;
		IThat<TCollection> IStructEnumerableItems<TCollection, TItem>.Subject => _subject;
	}

	/// <summary>
	///     The items of a non-generic <see cref="IEnumerable" /> that a quantifier selected.
	/// </summary>
	internal sealed partial class NonGenericItems<TCollection> : INonGenericEnumerableItems<TCollection>
		where TCollection : IEnumerable?
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<TCollection?> _subject;

		internal NonGenericItems(IThat<TCollection?> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier INonGenericEnumerableItems<TCollection>.Quantifier => _quantifier;
		IThat<TCollection?> INonGenericEnumerableItems<TCollection>.Subject => _subject;
	}
}
