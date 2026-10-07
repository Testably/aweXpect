#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	public partial class Elements
	{
		/// <summary>
		///     …satisfy the <paramref name="predicate" />.
		/// </summary>
		public AndOrResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
			Satisfy(System.Func<string?, bool> predicate,
				[CallerArgumentExpression("predicate")]
				string doNotPopulateThisValue = "")
		{
			predicate.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, DoNotPopulateThisValue: doNotPopulateThisValue, Predicate: predicate),
					static (state, it, grammars)
						=> new AsyncCollectionConstraint<string?>(
							it, grammars,
							state.Quantifier,
							new ElementSatisfying<string?, string?>(state.Predicate, state.DoNotPopulateThisValue),
							"did")),
				_subject);
		}
	}

	public partial class Elements<TItem>
	{
		/// <summary>
		///     …satisfy the <paramref name="predicate" />.
		/// </summary>
		public AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
			Satisfy(System.Func<TItem, bool> predicate,
				[CallerArgumentExpression("predicate")]
				string doNotPopulateThisValue = "")
		{
			predicate.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, DoNotPopulateThisValue: doNotPopulateThisValue, Predicate: predicate),
					static (state, it, grammars)
						=> new AsyncCollectionConstraint<TItem>(
							it, grammars,
							state.Quantifier,
							new ElementSatisfying<TItem, TItem>(state.Predicate, state.DoNotPopulateThisValue),
							"did")),
				_subject);
		}
	}
}
#endif
