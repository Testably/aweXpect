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
		///     …are exactly of type <typeparamref name="TType" />.
		/// </summary>
		public AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
			AreExactly<TType>()
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint(_quantifier,
					static (quantifier, it, grammars)
						=> new CollectionConstraint<IEnumerable<TItem>?, TItem>(
							it, grammars,
							quantifier,
							ElementOfType<TItem, TType>.ExactlyInstance,
							"were")),
				_subject);
		}

		/// <summary>
		///     …are exactly of type <paramref name="type" />.
		/// </summary>
		public AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
			AreExactly(Type type)
		{
			type.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint((Quantifier: _quantifier, Type: type),
					static (state, it, grammars)
						=> new CollectionConstraint<IEnumerable<TItem>?, TItem>(
							it, grammars,
							state.Quantifier,
							new ElementOfType<TItem>(state.Type, true),
							"were")),
				_subject);
		}
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …are exactly of type <typeparamref name="TType" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable?>>
			AreExactly<TType>()
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable?>>(
				expectationBuilder.AddConstraint(_quantifier,
					static (quantifier, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							quantifier,
							ElementOfType<object?, TType>.ExactlyInstance,
							"were")),
				_subject);
		}

		/// <summary>
		///     …are exactly of type <paramref name="type" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable?>>
			AreExactly(Type type)
		{
			type.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable?>>(
				expectationBuilder.AddConstraint((Quantifier: _quantifier, Type: type),
					static (state, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							state.Quantifier,
							new ElementOfType<object?>(state.Type, true),
							"were")),
				_subject);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …are exactly of type <typeparamref name="TType" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			AreExactly<TType>()
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint(_quantifier,
					static (quantifier, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							quantifier,
							ElementOfType<object?, TType>.ExactlyInstance,
							"were")),
				_subject);
		}

		/// <summary>
		///     …are exactly of type <paramref name="type" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			AreExactly(Type type)
		{
			type.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint((Quantifier: _quantifier, Type: type),
					static (state, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							state.Quantifier,
							new ElementOfType<object?>(state.Type, true),
							"were")),
				_subject);
		}
	}
}
