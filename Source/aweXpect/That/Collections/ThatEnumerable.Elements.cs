using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Result class for expectations on the elements of a <see cref="IEnumerable{T}" /> of <see langword="string" />.
	/// </summary>
	public partial class Elements : IEnumerableStringElements
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<IEnumerable<string?>?> _subject;

		internal Elements(IThat<IEnumerable<string?>?> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IEnumerableStringElements.Quantifier => _quantifier;
		IThat<IEnumerable<string?>?> IEnumerableStringElements.Subject => _subject;
	}

	/// <summary>
	///     Result class for expectations on the elements of a <see cref="IEnumerable{TItem}" /> of
	///     <typeparamref name="TItem" />.
	/// </summary>
	public partial class Elements<TItem> : IEnumerableElements<TItem>
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<IEnumerable<TItem>?> _subject;

		internal Elements(IThat<IEnumerable<TItem>?> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IEnumerableElements<TItem>.Quantifier => _quantifier;
		IThat<IEnumerable<TItem>?> IEnumerableElements<TItem>.Subject => _subject;
	}

	/// <summary>
	///     Result class for expectations on the elements of a <see cref="IEnumerable{TItem}" /> of
	///     <typeparamref name="TItem" />.
	/// </summary>
	public partial class
		ElementsForStructEnumerable<TEnumerable, TItem> : IStructEnumerableElements<TEnumerable, TItem>
		where TEnumerable : struct, IEnumerable<TItem>
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<TEnumerable> _subject;

		internal ElementsForStructEnumerable(IThat<TEnumerable> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IStructEnumerableElements<TEnumerable, TItem>.Quantifier => _quantifier;
		IThat<TEnumerable> IStructEnumerableElements<TEnumerable, TItem>.Subject => _subject;
	}

	/// <summary>
	///     Result class for expectations on the elements of a <see cref="IEnumerable{TItem}" />
	///     of <see langword="string" />.
	/// </summary>
	public partial class ElementsForStructEnumerable<TEnumerable> : IStructEnumerableStringElements<TEnumerable>
		where TEnumerable : struct, IEnumerable<string?>
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<TEnumerable> _subject;

		internal ElementsForStructEnumerable(IThat<TEnumerable> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IStructEnumerableStringElements<TEnumerable>.Quantifier => _quantifier;
		IThat<TEnumerable> IStructEnumerableStringElements<TEnumerable>.Subject => _subject;
	}

	/// <summary>
	///     Result class for expectations on the elements of an <see cref="IEnumerable" />.
	/// </summary>
	public partial class ElementsForEnumerable<TEnumerable> : INonGenericEnumerableElements<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<TEnumerable?> _subject;

		internal ElementsForEnumerable(IThat<TEnumerable?> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier INonGenericEnumerableElements<TEnumerable>.Quantifier => _quantifier;
		IThat<TEnumerable?> INonGenericEnumerableElements<TEnumerable>.Subject => _subject;
	}
}
