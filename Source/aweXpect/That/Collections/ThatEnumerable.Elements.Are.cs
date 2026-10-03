using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnumerable
{
	public partial class Elements<TItem>
	{
		/// <summary>
		///     …are of type <typeparamref name="TType" />.
		/// </summary>
		public AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
			Are<TType>()
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint(_quantifier,
					static (quantifier, it, grammars)
						=> new CollectionConstraint<IEnumerable<TItem>?, TItem>(
							it, grammars,
							quantifier,
							g => ElementExpectations.IsOfType(g, Formatter.Format(typeof(TType))),
							a => a is TType,
							"were")),
				_subject);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
			Are(Type type)
		{
			type.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint((Quantifier: _quantifier, Type: type),
					static (state, it, grammars)
						=> new CollectionConstraint<IEnumerable<TItem>?, TItem>(
							it, grammars,
							state.Quantifier,
							g => ElementExpectations.IsOfType(g, Formatter.Format(state.Type)),
							a => state.Type.IsInstanceOfType(a),
							"were")),
				_subject);
		}
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …are of type <typeparamref name="TType" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable?>>
			Are<TType>()
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable?>>(
				expectationBuilder.AddConstraint(_quantifier,
					static (quantifier, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							quantifier,
							g => ElementExpectations.IsOfType(g, Formatter.Format(typeof(TType))),
							a => a is TType,
							"were")),
				_subject);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable?>>
			Are(Type type)
		{
			type.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable?>>(
				expectationBuilder.AddConstraint((Quantifier: _quantifier, Type: type),
					static (state, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							state.Quantifier,
							g => ElementExpectations.IsOfType(g, Formatter.Format(state.Type)),
							a => state.Type.IsInstanceOfType(a),
							"were")),
				_subject);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …are of type <typeparamref name="TType" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			Are<TType>()
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint(_quantifier,
					static (quantifier, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							quantifier,
							g => ElementExpectations.IsOfType(g, Formatter.Format(typeof(TType))),
							a => a is TType,
							"were")),
				_subject);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			Are(Type type)
		{
			type.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint((Quantifier: _quantifier, Type: type),
					static (state, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							state.Quantifier,
							g => ElementExpectations.IsOfType(g, Formatter.Format(state.Type)),
							a => state.Type.IsInstanceOfType(a),
							"were")),
				_subject);
		}
	}
}
