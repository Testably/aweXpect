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
		///     …are exactly of type <typeparamref name="TType" />.
		/// </summary>
		public AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
			AreExactly<TType>()
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint(_quantifier,
					static (quantifier, it, grammars)
						=> new AsyncCollectionConstraint<TItem>(
							it, grammars,
							quantifier,
							g => ElementExpectations.IsExactlyOfType(g, Formatter.Format(typeof(TType))),
							a => a?.GetType() == typeof(TType),
							"were")),
				_subject);
		}

		/// <summary>
		///     …are exactly of type <paramref name="type" />.
		/// </summary>
		public AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
			AreExactly(Type type)
		{
			type.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint((Quantifier: _quantifier, Type: type),
					static (state, it, grammars)
						=> new AsyncCollectionConstraint<TItem>(
							it, grammars,
							state.Quantifier,
							g => ElementExpectations.IsExactlyOfType(g, Formatter.Format(state.Type)),
							a => a?.GetType() == state.Type,
							"were")),
				_subject);
		}
	}
}
#endif
