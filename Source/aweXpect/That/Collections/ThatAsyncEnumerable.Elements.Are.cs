#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	public partial class Elements<TItem>
	{
		/// <summary>
		///     …are of type <typeparamref name="TType" />.
		/// </summary>
		public AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
			Are<TType>()
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint(_quantifier,
					static (quantifier, it, grammars)
						=> new AsyncCollectionConstraint<TItem>(
							it, grammars,
							quantifier,
							g => ElementExpectations.IsOfType(g, Formatter.Format(typeof(TType))),
							a => typeof(TType).IsAssignableFrom(a?.GetType()),
							"were")),
				_subject);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
			Are(Type type)
		{
			type.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint((Quantifier: _quantifier, Type: type),
					static (state, it, grammars)
						=> new AsyncCollectionConstraint<TItem>(
							it, grammars,
							state.Quantifier,
							g => ElementExpectations.IsOfType(g, Formatter.Format(state.Type)),
							a => state.Type.IsAssignableFrom(a?.GetType()),
							"were")),
				_subject);
		}
	}
}
#endif
