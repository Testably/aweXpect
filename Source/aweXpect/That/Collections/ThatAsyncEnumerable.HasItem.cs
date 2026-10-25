#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Extending;
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
		OptionsSlot<CollectionIndexOptions> indexOptions = new(CollectionIndexOptions.Default);
		OptionsSlot<PredicateOptions<TItem>> options = new(PredicateOptions<TItem>.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<IAsyncEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint((Options: options, IndexOptions: indexOptions, Negated: negated),
				static (state, it, grammars)
					=> new AsyncHasItemConstraint<TItem>(it, grammars,
						new ItemMatchingOptions<TItem>(state.Options),
						state.IndexOptions).InvertIf(state.Negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasMatchingItem, NegatedSummary = DoesNotHaveMatchingItem)]
	internal static HasItemResult<IAsyncEnumerable<TItem>>
		HasMatchingItemCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject, System.Func<TItem, bool> predicate,
			string predicateExpression,
			bool negated)
	{
		predicate.ThrowIfNull();
		OptionsSlot<CollectionIndexOptions> indexOptions = new(CollectionIndexOptions.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IAsyncEnumerable<TItem>>(
			expectationBuilder.AddConstraint(
				(Predicate: predicate, PredicateExpression: predicateExpression, IndexOptions: indexOptions,
					Negated: negated),
				static (state, it, grammars)
					=> new AsyncHasItemConstraint<TItem>(it, grammars,
						new ItemMatchingPredicate<TItem>(state.Predicate, state.PredicateExpression),
						state.IndexOptions).InvertIf(state.Negated)),
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
		OptionsSlot<CollectionIndexOptions> indexOptions = new(CollectionIndexOptions.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		OptionsSlot<ObjectMatchType<TItem>> options = new(ObjectMatchType<TItem>.Default);
		return new ObjectHasItemResult<IAsyncEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, IndexOptions: indexOptions, Negated: negated),
				static (state, it, grammars)
					=> new AsyncHasItemConstraint<TItem>(it, grammars,
						new EqualItem<TItem>(state.Options, state.Expected),
						state.IndexOptions).InvertIf(state.Negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Factory = typeof(ObjectToleranceMatchTypeFactory),
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static ObjectHasItemWithToleranceResult<IAsyncEnumerable<TItem>, TItem, TTolerance>
		HasTheItemWithToleranceCore<TItem, TTolerance>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			TItem expected,
			ObjectToleranceMatchType<TItem, TTolerance> initialOptions,
			bool negated)
	{
		OptionsSlot<ObjectMatchType<TItem>> options = new(initialOptions);
		OptionsSlot<CollectionIndexOptions> indexOptions = new(CollectionIndexOptions.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectHasItemWithToleranceResult<IAsyncEnumerable<TItem>, TItem, TTolerance>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, IndexOptions: indexOptions, Negated: negated),
				static (state, it, grammars)
					=> new AsyncHasItemConstraint<TItem>(it, grammars,
						new EqualItem<TItem>(state.Options, state.Expected),
						state.IndexOptions).InvertIf(state.Negated)),
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
		OptionsSlot<CollectionIndexOptions> indexOptions = new(CollectionIndexOptions.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		OptionsSlot<StringEqualityOptions> options = new(new StringEqualityOptions(negated ? "unexpected" : "expected"));
		return new StringHasItemResult<IAsyncEnumerable<string?>>(
			expectationBuilder.AddConstraint(
				(Options: options, Expected: expected, IndexOptions: indexOptions, Negated: negated),
				static (state, it, grammars)
					=> new AsyncHasItemConstraint<string?>(it, grammars,
						new EqualStringItem(state.Options.Value, state.Expected),
						state.IndexOptions).InvertIf(state.Negated)),
			subject,
			indexOptions,
			options);
	}
}
#endif
