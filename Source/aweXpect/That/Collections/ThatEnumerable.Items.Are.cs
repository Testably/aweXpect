using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnumerable
{
	internal sealed partial class Items<TItem>
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
							ElementOfType<TItem, TType>.Instance,
							"were")),
				_subject);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
			Are(System.Type type)
		{
			type.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint((Quantifier: _quantifier, Type: type),
					static (state, it, grammars)
						=> new CollectionConstraint<IEnumerable<TItem>?, TItem>(
							it, grammars,
							state.Quantifier,
							new ElementOfType<TItem>(state.Type, false),
							"were")),
				_subject);
		}
	}

	internal sealed partial class NonGenericItems<TCollection>
	{
		/// <summary>
		///     …are of type <typeparamref name="TType" />.
		/// </summary>
		public AndOrResult<TCollection, IThat<TCollection?>>
			Are<TType>()
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TCollection, IThat<TCollection?>>(
				expectationBuilder.AddConstraint(_quantifier,
					static (quantifier, it, grammars)
						=> new CollectionConstraint<TCollection, object?>(
							it, grammars,
							quantifier,
							ElementOfType<object?, TType>.Instance,
							"were")),
				_subject);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public AndOrResult<TCollection, IThat<TCollection?>>
			Are(System.Type type)
		{
			type.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TCollection, IThat<TCollection?>>(
				expectationBuilder.AddConstraint((Quantifier: _quantifier, Type: type),
					static (state, it, grammars)
						=> new CollectionConstraint<TCollection, object?>(
							it, grammars,
							state.Quantifier,
							new ElementOfType<object?>(state.Type, false),
							"were")),
				_subject);
		}
	}

	internal sealed partial class StructItems<TCollection, TItem>
	{
		/// <summary>
		///     …are of type <typeparamref name="TType" />.
		/// </summary>
		public AndOrResult<TCollection, IThat<TCollection>>
			Are<TType>()
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TCollection, IThat<TCollection>>(
				expectationBuilder.AddConstraint(_quantifier,
					static (quantifier, it, grammars)
						=> new CollectionConstraint<TCollection, object?>(
							it, grammars,
							quantifier,
							ElementOfType<object?, TType>.Instance,
							"were")),
				_subject);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public AndOrResult<TCollection, IThat<TCollection>>
			Are(System.Type type)
		{
			type.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TCollection, IThat<TCollection>>(
				expectationBuilder.AddConstraint((Quantifier: _quantifier, Type: type),
					static (state, it, grammars)
						=> new CollectionConstraint<TCollection, object?>(
							it, grammars,
							state.Quantifier,
							new ElementOfType<object?>(state.Type, false),
							"were")),
				_subject);
		}
	}
}
