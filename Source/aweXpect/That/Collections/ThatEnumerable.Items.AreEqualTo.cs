using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;
using StringEqualityOptions = aweXpect.Options.StringEqualityOptions;

namespace aweXpect;

public static partial class ThatEnumerable
{
	private const string ItemsAreEqualTo = "…are equal to the <paramref name=\"expected\" /> value.";

	[CreateExpectationFamily("AreEqualTo", Factory = typeof(ObjectToleranceMatchTypeFactory),
		Summary = ItemsAreEqualTo, Remarks = SetItemComparerRemarks)]
	internal static ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>
		AreEqualToWithToleranceCore<TItem, TTolerance>(
			IEnumerableItems<TItem> items,
			TItem expected, Options.ObjectToleranceMatchType<TItem, TTolerance> initialOptions)
	{
		OptionsSlot<ObjectMatchType<TItem>> options = new(initialOptions);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>(
			expectationBuilder.AddConstraint((Options: options, Expected: expected, items.Quantifier),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<TItem, TItem> itemOptions = new(state.Options, state.Expected is not null);
					return new CollectionConstraint<IEnumerable<TItem>?, TItem>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<TItem, TItem>(itemOptions, state.Expected),
						"were");
				}),
			items.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ItemsAreEqualTo, Remarks = SetItemComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		AreEqualToCore<TItem>(
			IEnumerableItems<TItem> items,
			TItem expected)
	{
		OptionsSlot<ObjectMatchType<TItem>> options = new(ItemEqualsMatchType<TItem>.Instance);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((Options: options, Expected: expected, items.Quantifier),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<TItem, TItem> itemOptions =
						new(state.Options, state.Expected is not null);
					return new CollectionConstraint<IEnumerable<TItem>?, TItem>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<TItem, TItem>(itemOptions, state.Expected, true),
						"were");
				}),
			items.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Priority = -1, Summary = ItemsAreEqualTo,
		Remarks = UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, object?>
		AreEqualToForEnumerableCore(
			INonGenericEnumerableItems<IEnumerable> items,
			object? expected)
	{
		OptionsSlot<ObjectMatchType<object?>> options = new(ItemEqualsMatchType<object?>.Instance);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, object?>(
			expectationBuilder.AddConstraint((Options: options, Expected: expected, items.Quantifier),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<object?, object?> itemOptions =
						new(state.Options, state.Expected is not null);
					return new CollectionConstraint<IEnumerable, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<object?, object?>(itemOptions, state.Expected, true),
						"were");
				}),
			items.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ItemsAreEqualTo)]
	internal static ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>
		AreEqualToForStructCore<TCollection, TItem>(
			IStructEnumerableItems<TCollection, TItem> items,
			TItem expected)
		where TCollection : struct, IEnumerable<TItem>
	{
		OptionsSlot<ObjectMatchType<TItem>> options = new(ItemEqualsMatchType<TItem>.Instance);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint((items.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new CollectionConstraint<TCollection, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<object?, TItem>(state.Options, state.Expected, true),
						"were")),
			items.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Factory = typeof(ObjectToleranceMatchTypeFactory),
		Summary = ItemsAreEqualTo)]
	internal static ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>
		AreEqualToWithToleranceForStructCore<TCollection, TItem, TTolerance>(
			IStructEnumerableItems<TCollection, TItem> items,
			TItem expected, Options.ObjectToleranceMatchType<TItem, TTolerance> initialOptions)
		where TCollection : struct, IEnumerable<TItem>
	{
		OptionsSlot<ObjectMatchType<TItem>> options = new(initialOptions);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>(
			expectationBuilder.AddConstraint((items.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new CollectionConstraint<TCollection, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<object?, TItem>(state.Options, state.Expected),
						"were")),
			items.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ItemsAreEqualTo, Remarks = SetItemComparerRemarks)]
	internal static StringEqualityTypeResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		AreEqualToForStringsCore(
			IEnumerableItems<string?> items,
			string? expected)
	{
		OptionsSlot<StringEqualityOptions> options = new(new StringEqualityOptions(nameof(expected)));
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((Options: options, Expected: expected, items.Quantifier),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<string?, string?> itemOptions =
						new(state.Options, state.Expected is not null);
					return new CollectionConstraint<IEnumerable<string?>?, string?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualToString<string?>(state.Options.Value, state.Expected, itemOptions),
						"were");
				}),
			items.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Summary = ItemsAreEqualTo)]
	internal static StringEqualityTypeResult<TCollection, IThat<TCollection>>
		AreEqualToForStructStringsCore<TCollection>(
			IStructEnumerableItems<TCollection, string?> items,
			string? expected)
		where TCollection : struct, IEnumerable<string?>
	{
		OptionsSlot<StringEqualityOptions> options = new(new StringEqualityOptions(nameof(expected)));
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint((items.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new CollectionConstraint<TCollection, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualToString<object?>(state.Options.Value, state.Expected),
						"were")),
			items.Subject,
			options);
	}
}
