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
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionConstraint<TItem>(
						expectationBuilder,
						it, grammars,
						_quantifier,
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
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionConstraint<TItem>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsExactlyOfType(g, Formatter.Format(type)),
						a => a?.GetType() == type,
						"were")),
				_subject);
		}
	}
}
#endif
