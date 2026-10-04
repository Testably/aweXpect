using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Metadata;
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
			AreEquivalentTo<TExpected>([RequiresMemberMetadata] TExpected expected,
				Func<EquivalencyOptions<TExpected>, EquivalencyOptions>? options = null,
				[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			EquivalencyOptions equivalencyOptions = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get();
			if (options != null)
			{
				equivalencyOptions = options(new EquivalencyOptions<TExpected>(equivalencyOptions));
			}

			ObjectEqualityOptions<TItem> equalityOptions = new();
			equalityOptions.Equivalent(equivalencyOptions);
			return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, Expected: expected, DoNotPopulateThisValue: doNotPopulateThisValue,
						EqualityOptions: equalityOptions),
					static (state, it, grammars)
						=> new CollectionConstraint<IEnumerable<TItem>?, TItem>(
							it, grammars,
							state.Quantifier,
							new ElementEquivalentTo<TItem, TItem, TExpected>(state.EqualityOptions, state.Expected,
								state.DoNotPopulateThisValue),
							"were")),
				_subject,
				equalityOptions);
		}

		/// <summary>
		///     …are equivalent to the <paramref name="expected" /> value.
		/// </summary>
		/// <remarks>
		///     This overload allows passing a literal <see langword="null" />, for which the generic type cannot be inferred.
		/// </remarks>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
			AreEquivalentTo([RequiresMemberMetadata] object? expected,
				Func<EquivalencyOptions<object?>, EquivalencyOptions>? options = null,
				[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
			=> AreEquivalentTo<object?>(expected, options, doNotPopulateThisValue);
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …are equivalent to the <paramref name="expected" /> value.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?>
			AreEquivalentTo<TExpected>([RequiresMemberMetadata] TExpected expected,
				Func<EquivalencyOptions<TExpected>, EquivalencyOptions>? options = null,
				[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			EquivalencyOptions equivalencyOptions = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get();
			if (options != null)
			{
				equivalencyOptions = options(new EquivalencyOptions<TExpected>(equivalencyOptions));
			}

			ObjectEqualityOptions<object?> equalityOptions = new();
			equalityOptions.Equivalent(equivalencyOptions);
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?>(
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, Expected: expected, DoNotPopulateThisValue: doNotPopulateThisValue,
						EqualityOptions: equalityOptions),
					static (state, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							state.Quantifier,
							new ElementEquivalentTo<object?, object?, TExpected>(state.EqualityOptions, state.Expected,
								state.DoNotPopulateThisValue),
							"were")),
				_subject,
				equalityOptions);
		}

		/// <summary>
		///     …are equivalent to the <paramref name="expected" /> value.
		/// </summary>
		/// <remarks>
		///     This overload allows passing a literal <see langword="null" />, for which the generic type cannot be inferred.
		/// </remarks>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?>
			AreEquivalentTo([RequiresMemberMetadata] object? expected,
				Func<EquivalencyOptions<object?>, EquivalencyOptions>? options = null,
				[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
			=> AreEquivalentTo<object?>(expected, options, doNotPopulateThisValue);
	}

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …are equivalent to the <paramref name="expected" /> value.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>
			AreEquivalentTo<TExpected>([RequiresMemberMetadata] TExpected expected,
				Func<EquivalencyOptions<TExpected>, EquivalencyOptions>? options = null,
				[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		{
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			EquivalencyOptions equivalencyOptions = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get();
			if (options != null)
			{
				equivalencyOptions = options(new EquivalencyOptions<TExpected>(equivalencyOptions));
			}

			ObjectEqualityOptions<TItem> equalityOptions = new();
			equalityOptions.Equivalent(equivalencyOptions);
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>(
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, Expected: expected, DoNotPopulateThisValue: doNotPopulateThisValue,
						EqualityOptions: equalityOptions),
					static (state, it, grammars)
						=> new CollectionConstraint<TEnumerable, object?>(
							it, grammars,
							state.Quantifier,
							new ElementEquivalentTo<object?, TItem, TExpected>(state.EqualityOptions, state.Expected,
								state.DoNotPopulateThisValue),
							"were")),
				_subject,
				equalityOptions);
		}

		/// <summary>
		///     …are equivalent to the <paramref name="expected" /> value.
		/// </summary>
		/// <remarks>
		///     This overload allows passing a literal <see langword="null" />, for which the generic type cannot be inferred.
		/// </remarks>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>
			AreEquivalentTo([RequiresMemberMetadata] object? expected,
				Func<EquivalencyOptions<object?>, EquivalencyOptions>? options = null,
				[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
			=> AreEquivalentTo<object?>(expected, options, doNotPopulateThisValue);
	}
}
