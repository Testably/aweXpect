using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Interface for <see cref="Elements" /> to get access to the <see cref="Quantifier" />
	///     and the <see cref="Subject" />.
	/// </summary>
	public interface IElements
	{
		/// <summary>
		///     The quantifier for the elements.
		/// </summary>
		public EnumerableQuantifier Quantifier { get; }

		/// <summary>
		///     The subject of the expectation.
		/// </summary>
		public IThat<IEnumerable<string?>?> Subject { get; }
	}

	/// <summary>
	///     Interface for <see cref="Elements{TItem}" /> to get access to the <see cref="Quantifier" />
	///     and the <see cref="Subject" />.
	/// </summary>
	public interface IElements<out TItem>
	{
		/// <summary>
		///     The quantifier for the elements.
		/// </summary>
		public EnumerableQuantifier Quantifier { get; }

		/// <summary>
		///     The subject of the expectation.
		/// </summary>
		public IThat<IEnumerable<TItem>?> Subject { get; }
	}

	/// <summary>
	///     Interface for <see cref="Elements" /> to get access to the <see cref="Quantifier" />
	///     and the <see cref="Subject" />.
	/// </summary>
	public interface IElementsForEnumerable<out TEnumerable>
		where TEnumerable : IEnumerable?
	{
		/// <summary>
		///     The quantifier for the elements.
		/// </summary>
		public EnumerableQuantifier Quantifier { get; }

		/// <summary>
		///     The subject of the expectation.
		/// </summary>
		public IThat<TEnumerable> Subject { get; }
	}

	/// <summary>
	///     Interface for <see cref="Elements{TItem}" /> to get access to the <see cref="Quantifier" />
	///     and the <see cref="Subject" />.
	/// </summary>
	public interface IElementsForStructEnumerable<out TEnumerable, TItem>
		where TEnumerable : struct, IEnumerable<TItem>
	{
		/// <summary>
		///     The quantifier for the elements.
		/// </summary>
		public EnumerableQuantifier Quantifier { get; }

		/// <summary>
		///     The subject of the expectation.
		/// </summary>
		public IThat<TEnumerable> Subject { get; }
	}

	/// <summary>
	///     Interface for <see cref="Elements{TItem}" /> to get access to the <see cref="Quantifier" />
	///     and the <see cref="Subject" />.
	/// </summary>
	public interface IElementsForStructEnumerable<out TEnumerable>
		where TEnumerable : struct, IEnumerable<string?>
	{
		/// <summary>
		///     The quantifier for the elements.
		/// </summary>
		public EnumerableQuantifier Quantifier { get; }

		/// <summary>
		///     The subject of the expectation.
		/// </summary>
		public IThat<TEnumerable> Subject { get; }
	}

	/// <summary>
	///     Result class for expectations on the elements of a <see cref="IEnumerable{T}" /> of <see langword="string" />.
	/// </summary>
	public partial class Elements : IElements
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<IEnumerable<string?>?> _subject;

		internal Elements(IThat<IEnumerable<string?>?> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IElements.Quantifier => _quantifier;
		IThat<IEnumerable<string?>?> IElements.Subject => _subject;
	}

	/// <summary>
	///     Result class for expectations on the elements of a <see cref="IEnumerable{TItem}" /> of
	///     <typeparamref name="TItem" />.
	/// </summary>
	public partial class Elements<TItem> : IElements<TItem>
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<IEnumerable<TItem>?> _subject;

		internal Elements(IThat<IEnumerable<TItem>?> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IElements<TItem>.Quantifier => _quantifier;
		IThat<IEnumerable<TItem>?> IElements<TItem>.Subject => _subject;
	}

	/// <summary>
	///     Result class for expectations on the elements of a <see cref="IEnumerable{TItem}" /> of
	///     <typeparamref name="TItem" />.
	/// </summary>
	public partial class
		ElementsForStructEnumerable<TEnumerable, TItem> : IElementsForStructEnumerable<TEnumerable, TItem>
		where TEnumerable : struct, IEnumerable<TItem>
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<TEnumerable> _subject;

		internal ElementsForStructEnumerable(IThat<TEnumerable> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IElementsForStructEnumerable<TEnumerable, TItem>.Quantifier => _quantifier;
		IThat<TEnumerable> IElementsForStructEnumerable<TEnumerable, TItem>.Subject => _subject;
	}

	/// <summary>
	///     Result class for expectations on the elements of a <see cref="IEnumerable{TItem}" />
	///     of <see langword="string" />.
	/// </summary>
	public partial class ElementsForStructEnumerable<TEnumerable> : IElementsForStructEnumerable<TEnumerable>
		where TEnumerable : struct, IEnumerable<string?>
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<TEnumerable> _subject;

		internal ElementsForStructEnumerable(IThat<TEnumerable> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IElementsForStructEnumerable<TEnumerable>.Quantifier => _quantifier;
		IThat<TEnumerable> IElementsForStructEnumerable<TEnumerable>.Subject => _subject;
	}

	/// <summary>
	///     Result class for expectations on the elements of an <see cref="IEnumerable" />.
	/// </summary>
	public partial class ElementsForEnumerable<TEnumerable> : IElementsForEnumerable<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private readonly EnumerableQuantifier _quantifier;
		private readonly IThat<TEnumerable> _subject;

		internal ElementsForEnumerable(IThat<TEnumerable> subject, EnumerableQuantifier quantifier)
		{
			_subject = subject;
			_quantifier = quantifier;
		}

		EnumerableQuantifier IElementsForEnumerable<TEnumerable>.Quantifier => _quantifier;
		IThat<TEnumerable> IElementsForEnumerable<TEnumerable>.Subject => _subject;
	}
}
