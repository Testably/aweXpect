#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	public partial class Elements<TItem>
	{
		/// <summary>
		///     …are of type <typeparamref name="TType" />.
		/// </summary>
		public ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
			Are<TType>()
		{
			ObjectEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionConstraint<TItem>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => (g.IsPlural(), g.IsNegated()) switch
						{
							(true, false) => $"are of type {Formatter.Format(typeof(TType))}",
							(false, false) => $"is of type {Formatter.Format(typeof(TType))}",
							(true, true) => $"are not of type {Formatter.Format(typeof(TType))}",
							(false, true) => $"is not of type {Formatter.Format(typeof(TType))}",
						},
						a => typeof(TType).IsAssignableFrom(a?.GetType()),
						"were")),
				_subject,
				options);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
			Are(Type type)
		{
			type.ThrowIfNull();
			ObjectEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionConstraint<TItem>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => (g.IsPlural(), g.IsNegated()) switch
						{
							(true, false) => $"are of type {Formatter.Format(type)}",
							(false, false) => $"is of type {Formatter.Format(type)}",
							(true, true) => $"are not of type {Formatter.Format(type)}",
							(false, true) => $"is not of type {Formatter.Format(type)}",
						},
						a => type.IsAssignableFrom(a?.GetType()),
						"were")),
				_subject,
				options);
		}
	}
}
#endif
