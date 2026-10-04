using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnumerable
{
	public partial class Elements
	{
		/// <summary>
		///     …satisfy the <paramref name="predicate" />.
		/// </summary>
		public AndOrResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
			Satisfy(
				Func<string?, bool> predicate,
				[CallerArgumentExpression("predicate")]
				string doNotPopulateThisValue = "")
		{
			predicate.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, DoNotPopulateThisValue: doNotPopulateThisValue, Predicate: predicate),
					static (state, it, grammars)
						=> new CollectionConstraint<IEnumerable<string?>?, string?>(
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
		public AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
			Satisfy(
				Func<TItem, bool> predicate,
				[CallerArgumentExpression("predicate")]
				string doNotPopulateThisValue = "")
		{
			predicate.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, DoNotPopulateThisValue: doNotPopulateThisValue, Predicate: predicate),
					static (state, it, grammars)
						=> new CollectionConstraint<IEnumerable<TItem>?, TItem>(
							it, grammars,
							state.Quantifier,
							new ElementSatisfying<TItem, TItem>(state.Predicate, state.DoNotPopulateThisValue),
							"did")),
				_subject);
		}
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …satisfy the <paramref name="predicate" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable?>>
			Satisfy(
				Func<object?, bool> predicate,
				[CallerArgumentExpression("predicate")]
				string doNotPopulateThisValue = "")
		{
			predicate.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable?>>(
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, DoNotPopulateThisValue: doNotPopulateThisValue, Predicate: predicate),
					static (state, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							state.Quantifier,
							new ElementSatisfying<object?, object?>(state.Predicate, state.DoNotPopulateThisValue),
							"did")),
				_subject);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …satisfy the <paramref name="predicate" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			Satisfy(
				Func<TItem, bool> predicate,
				[CallerArgumentExpression("predicate")]
				string doNotPopulateThisValue = "")
		{
			predicate.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, DoNotPopulateThisValue: doNotPopulateThisValue, Predicate: predicate),
					static (state, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							state.Quantifier,
							new ElementSatisfying<object?, TItem>(state.Predicate, state.DoNotPopulateThisValue),
							"did")),
				_subject);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable>
	{
		/// <summary>
		///     …satisfy the <paramref name="predicate" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			Satisfy(
				Func<string?, bool> predicate,
				[CallerArgumentExpression("predicate")]
				string doNotPopulateThisValue = "")
		{
			predicate.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, DoNotPopulateThisValue: doNotPopulateThisValue, Predicate: predicate),
					static (state, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							state.Quantifier,
							new ElementSatisfying<object?, string?>(state.Predicate, state.DoNotPopulateThisValue),
							"did")),
				_subject);
		}
	}
}
