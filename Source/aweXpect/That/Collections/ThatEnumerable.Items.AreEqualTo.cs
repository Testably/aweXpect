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

	[CreateExpectationFamily("AreEqualTo", Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ItemsAreEqualTo, Remarks = SetItemComparerRemarks)]
	internal static ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>
		AreEqualToWithToleranceCore<TItem, TTolerance>(
			IEnumerableItems<TItem> items,
			TItem expected, Options.ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
	{
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
		ItemEqualityOptions<TItem> options = new();
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
						new ElementEqualTo<TItem, TItem>(itemOptions, state.Expected, state.Options),
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
		ItemEqualityOptions<object?> options = new();
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
						new ElementEqualTo<object?, object?>(itemOptions, state.Expected, state.Options),
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
		ItemEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint((items.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new CollectionConstraint<TCollection, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualTo<object?, TItem>(state.Options, state.Expected, state.Options),
						"were")),
			items.Subject,
			options);
	}

	[CreateExpectationFamily("AreEqualTo", Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = ItemsAreEqualTo)]
	internal static ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>
		AreEqualToWithToleranceForStructCore<TCollection, TItem, TTolerance>(
			IStructEnumerableItems<TCollection, TItem> items,
			TItem expected, Options.ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
		where TCollection : struct, IEnumerable<TItem>
	{
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
		StringEqualityOptions options = new(nameof(expected));
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
						new ElementEqualToString<string?>(state.Options, state.Expected, itemOptions),
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
		StringEqualityOptions options = new(nameof(expected));
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint((items.Quantifier, Expected: expected, Options: options),
				static (state, it, grammars)
					=> new CollectionConstraint<TCollection, object?>(
						it, grammars,
						state.Quantifier,
						new ElementEqualToString<object?>(state.Options, state.Expected),
						"were")),
			items.Subject,
			options);
	}
}
