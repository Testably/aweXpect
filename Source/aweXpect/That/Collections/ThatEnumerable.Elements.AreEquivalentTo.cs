using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnumerable
{
	public partial class Elements<TItem>
	{
		/// <summary>
		///     …are equivalent to the <paramref name="expected" /> value.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
			AreEquivalentTo<TExpected>(TExpected expected,
				Func<EquivalencyOptions<TExpected>, EquivalencyOptions>? options = null,
				[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			EquivalencyOptions equivalencyOptions = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get();
			if (options != null)
			{
				equivalencyOptions = options(new EquivalencyOptions<TExpected>(equivalencyOptions));
			}

			expectationBuilder.AddEquivalencyContext(equivalencyOptions);

			ObjectEqualityOptions<TItem> equalityOptions = new();
			equalityOptions.Equivalent(equivalencyOptions);
			return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AsyncCollectionConstraint<TItem>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => (g.IsPlural(), g.IsNegated()) switch
						{
							(true, false) => $"are equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
							(false, false) => $"is equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
							(true, true) =>
								$"are not equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
							(false, true) =>
								$"is not equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
						},
						a => equalityOptions.AreConsideredEqual(a, expected),
						"were")),
				_subject,
				equalityOptions);
		}
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …are equivalent to the <paramref name="expected" /> value.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, object?>
			AreEquivalentTo<TExpected>(TExpected expected,
				Func<EquivalencyOptions<TExpected>, EquivalencyOptions>? options = null,
				[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			EquivalencyOptions equivalencyOptions = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get();
			if (options != null)
			{
				equivalencyOptions = options(new EquivalencyOptions<TExpected>(equivalencyOptions));
			}

			expectationBuilder.AddEquivalencyContext(equivalencyOptions);

			ObjectEqualityOptions<object?> equalityOptions = new();
			equalityOptions.Equivalent(equivalencyOptions);
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, object?>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AsyncCollectionForEnumerableConstraint<TEnumerable>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => (g.IsPlural(), g.IsNegated()) switch
						{
							(true, false) => $"are equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
							(false, false) => $"is equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
							(true, true) =>
								$"are not equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
							(false, true) =>
								$"is not equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
						},
						a => equalityOptions.AreConsideredEqual(a, expected),
						"were")),
				_subject,
				equalityOptions);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …are equivalent to the <paramref name="expected" /> value.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>
			AreEquivalentTo<TExpected>(TExpected expected,
				Func<EquivalencyOptions<TExpected>, EquivalencyOptions>? options = null,
				[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			EquivalencyOptions equivalencyOptions = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get();
			if (options != null)
			{
				equivalencyOptions = options(new EquivalencyOptions<TExpected>(equivalencyOptions));
			}

			expectationBuilder.AddEquivalencyContext(equivalencyOptions);

			ObjectEqualityOptions<TItem> equalityOptions = new();
			equalityOptions.Equivalent(equivalencyOptions);
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AsyncCollectionForEnumerableConstraint<TEnumerable>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => (g.IsPlural(), g.IsNegated()) switch
						{
							(true, false) => $"are equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
							(false, false) => $"is equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
							(true, true) =>
								$"are not equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
							(false, true) =>
								$"is not equivalent to {doNotPopulateThisValue.TrimCommonWhiteSpace()}",
						},
						a => equalityOptions.AreConsideredEqual((TItem)a!, expected),
						"were")),
				_subject,
				equalityOptions);
		}
	}
}
