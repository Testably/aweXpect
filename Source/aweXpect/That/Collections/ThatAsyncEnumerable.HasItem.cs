#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	private const string HasAnItem = "Verifies that the collection has an item…";
	private const string DoesNotHaveAnItem = "Verifies that the collection does not have an item…";

	private const string HasMatchingItem =
		"Verifies that the collection has an item matching the <paramref name=\"predicate\" />…";

	private const string DoesNotHaveMatchingItem =
		"Verifies that the collection does not have an item matching the <paramref name=\"predicate\" />…";

	private const string HasTheItem = "Verifies that the collection has the <paramref name=\"expected\" /> item…";

	private const string DoesNotHaveTheItem =
		"Verifies that the collection does not have the <paramref name=\"unexpected\" /> item…";

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasAnItem, NegatedSummary = DoesNotHaveAnItem)]
	internal static HasItemWithConditionResult<IAsyncEnumerable<TItem>, TItem>
		HasItemCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<IAsyncEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint((Options: options, IndexOptions: indexOptions, Negated: negated),
				static (state, it, grammars)
					=> new AsyncHasItemConstraint<TItem>(it, grammars,
						x => state.Options.Matches(x),
						state.Options.GetDescription,
						state.IndexOptions).InvertIf(state.Negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasMatchingItem, NegatedSummary = DoesNotHaveMatchingItem)]
	internal static HasItemResult<IAsyncEnumerable<TItem>>
		HasMatchingItemCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, bool> predicate,
			string predicateExpression,
			bool negated)
	{
		predicate.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IAsyncEnumerable<TItem>>(
			expectationBuilder.AddConstraint(
				(Predicate: predicate, PredicateExpression: predicateExpression, IndexOptions: indexOptions,
					Negated: negated),
				static (state, it, grammars)
					=> new AsyncHasItemConstraint<TItem>(it, grammars, state.Predicate,
						() => $"matching {state.PredicateExpression}", state.IndexOptions).InvertIf(state.Negated)),
			subject,
			indexOptions);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static ObjectHasItemResult<IAsyncEnumerable<TItem>, TItem>
		HasTheItemCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			TItem expected,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ObjectEqualityOptions<TItem> options = new();
		return new ObjectHasItemResult<IAsyncEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, IndexOptions: indexOptions, Negated: negated),
				static (state, it, grammars)
					=> new AsyncHasItemConstraint<TItem>(it, grammars,
						a => state.Options.AreConsideredEqual(a, state.Expected),
						() => state.Options.GetItemExpectation(Formatter.Format(state.Expected),
							comparison: "equal to"),
						state.IndexOptions,
						appendOptionsContexts: state.Options.AppendContexts).InvertIf(state.Negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static ObjectHasItemWithToleranceResult<IAsyncEnumerable<TItem>, TItem, TTolerance>
		HasTheItemWithToleranceCore<TItem, TTolerance>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			TItem expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectHasItemWithToleranceResult<IAsyncEnumerable<TItem>, TItem, TTolerance>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, IndexOptions: indexOptions, Negated: negated),
				static (state, it, grammars)
					=> new AsyncHasItemConstraint<TItem>(it, grammars,
						a => state.Options.AreConsideredEqual(a, state.Expected),
						() => state.Options.GetItemExpectation(Formatter.Format(state.Expected),
							comparison: "equal to"),
						state.IndexOptions,
						appendOptionsContexts: state.Options.AppendContexts).InvertIf(state.Negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static StringHasItemResult<IAsyncEnumerable<string?>>
		HasTheItemForStringsCore(
			IThat<IAsyncEnumerable<string?>?> subject,
			string? expected,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		return new StringHasItemResult<IAsyncEnumerable<string?>>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, IndexOptions: indexOptions, Negated: negated),
				static (state, it, grammars)
					=> new AsyncHasItemConstraint<string?>(it, grammars,
						a => state.Options.AreConsideredEqual(a, state.Expected),
						() => state.Options.GetExpectation(state.Expected, grammars),
						state.IndexOptions).InvertIf(state.Negated)),
			subject,
			indexOptions,
			options);
	}
}
#endif
