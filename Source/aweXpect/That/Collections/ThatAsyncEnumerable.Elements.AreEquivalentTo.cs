#if NET8_0_OR_GREATER
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

public static partial class ThatAsyncEnumerable
{
	public partial class Elements<TItem>
	{
		/// <summary>
		///     …are equivalent to the <paramref name="expected" /> value.
		/// </summary>
		public ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
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
			return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AsyncCollectionConstraint<TItem>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsEquivalentTo(g, doNotPopulateThisValue.TrimCommonWhiteSpace()),
						a => equalityOptions.AreConsideredEqual(a, expected),
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
		public ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
			AreEquivalentTo(object? expected,
				Func<EquivalencyOptions<object?>, EquivalencyOptions>? options = null,
				[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
			=> AreEquivalentTo<object?>(expected, options, doNotPopulateThisValue);
	}
}
#endif
