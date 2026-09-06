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
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionConstraint<string?>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => ElementExpectations.Satisfies(g, doNotPopulateThisValue.TrimCommonWhiteSpace()),
						predicate,
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
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionConstraint<TItem>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => ElementExpectations.Satisfies(g, doNotPopulateThisValue.TrimCommonWhiteSpace()),
						predicate,
						"did")),
				_subject);
		}
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …satisfy the <paramref name="predicate" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			Satisfy(
				Func<object?, bool> predicate,
				[CallerArgumentExpression("predicate")]
				string doNotPopulateThisValue = "")
		{
			predicate.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionForEnumerableConstraint<TEnumerable>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => ElementExpectations.Satisfies(g, doNotPopulateThisValue.TrimCommonWhiteSpace()),
						predicate,
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
				Func<TItem?, bool> predicate,
				[CallerArgumentExpression("predicate")]
				string doNotPopulateThisValue = "")
		{
			predicate.ThrowIfNull();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new AndOrResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionForEnumerableConstraint<TEnumerable>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => ElementExpectations.Satisfies(g, doNotPopulateThisValue.TrimCommonWhiteSpace()),
						v => predicate((TItem)v!),
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
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionForEnumerableConstraint<TEnumerable>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => ElementExpectations.Satisfies(g, doNotPopulateThisValue.TrimCommonWhiteSpace()),
						v => predicate((string?)v),
						"did")),
				_subject);
		}
	}
}
