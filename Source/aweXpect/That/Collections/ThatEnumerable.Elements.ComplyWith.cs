using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	public partial class Elements<TItem>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
			ComplyWith(Action<IThatSubject<TItem>> expectations)
		{
			expectations.ThrowIfNull();
			return new AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
				_subject.Get().ExpectationBuilder.AddConstraint((Quantifier: _quantifier, Expectations: expectations),
					static (state, it, grammars)
						=> new ComplyWithConstraint<IEnumerable<TItem>?, TItem>(it, grammars, state.Quantifier,
							state.Expectations)),
				_subject);
		}
	}

	public partial class Elements
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
			ComplyWith(Action<IThatSubject<string?>> expectations)
		{
			expectations.ThrowIfNull();
			return new AndOrResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
				_subject.Get().ExpectationBuilder.AddConstraint((Quantifier: _quantifier, Expectations: expectations),
					static (state, it, grammars)
						=> new ComplyWithConstraint<IEnumerable<string?>?, string?>(it, grammars, state.Quantifier,
							state.Expectations)),
				_subject);
		}
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable?>>
			ComplyWith(Action<IThatSubject<object?>> expectations)
		{
			expectations.ThrowIfNull();
			return new AndOrResult<TEnumerable, IThat<TEnumerable?>>(
				_subject.Get().ExpectationBuilder.AddConstraint((Quantifier: _quantifier, Expectations: expectations),
					static (state, it, grammars)
						=> new ComplyWithConstraint<TEnumerable?, object?>(it, grammars,
							state.Quantifier, state.Expectations)),
				_subject);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			ComplyWith(Action<IThatSubject<TItem>> expectations)
		{
			expectations.ThrowIfNull();
			return new AndOrResult<TEnumerable, IThat<TEnumerable>>(
				_subject.Get().ExpectationBuilder.AddConstraint((Quantifier: _quantifier, Expectations: expectations),
					static (state, it, grammars)
						=> new ComplyWithConstraint<TEnumerable, TItem>(it,
							grammars, state.Quantifier, state.Expectations)),
				_subject);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			ComplyWith(Action<IThatSubject<string?>> expectations)
			=> new ElementsForStructEnumerable<TEnumerable, string?>(_subject, _quantifier).ComplyWith(expectations);
	}
}
