using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core;

public sealed class ResultContextCollectorExtensionsTests
{
	[Test]
	public async Task AddCollectionContext_ShouldListTheItemsLikeTheBuiltInExpectations()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _) => contexts.AddCollectionContext(actual));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2, 3]
			             """);
		await That(await ContextsOf(Act)).IsEqualTo(await ContextsOf(async () => await That(subject).Contains(4)))
			.Because("the context is the same as the one of the built-in expectations");
	}

	[Test]
	public async Task AddCollectionContext_Untyped_ShouldListTheItemsLikeTheBuiltInExpectations()
	{
		IEnumerable subject = new ArrayList
		{
			1,
			2,
			3,
		};

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _) => contexts.AddCollectionContext(actual));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2, 3]
			             """);
		await That(await ContextsOf(Act)).IsEqualTo(await ContextsOf(async () => await That(subject).Contains(4)))
			.Because("the context is the same as the one of the built-in expectations");
	}

	[Test]
	public async Task AddCollectionContext_Untyped_WhenMaterializedItemsAreEmpty_ShouldNotAddAContext()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new UntypedMaterializedEnumerable([])));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not
			             """);
	}

	[Test]
	public async Task AddCollectionContext_Untyped_WhenMaterializedItemsDidNotReachTheEnd_ShouldListThemAsIncomplete()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new UntypedMaterializedEnumerable([1, 2,])));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2, (… and maybe more)]
			             """);
	}

	[Test]
	public async Task AddCollectionContext_Untyped_WhenTheFirstListedItemsAreNull_ShouldFollowTheTypeOfALaterItem()
	{
		int[] subject = [1, 2, 3,];
		IEnumerable collection = new ReadOnlyNullableItems([null, null, 1,]);

		async Task Act()
		{
			using (Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(2))
			{
				await That(subject).ShowsContexts((contexts, _, _) => contexts.AddCollectionContext(collection));
			}
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [<null>, <null>, (… and 1 more)]
			             """)
			.Because("all items of a collection that knows its count are searched, like the ones of a list");
	}

	[Test]
	public async Task AddCollectionContext_Untyped_WhenTheCountIsKnown_ShouldListTheItems()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new UntypedMaterializedEnumerable([1, 2,], 2)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2]
			             """);
	}

	[Test]
	public async Task AddCollectionContext_Untyped_WhenMultiDimensionalArray_ShouldListTheItemsOfEachDimension()
	{
		IEnumerable subject = new[,]
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _) => contexts.AddCollectionContext(actual));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [[1, 2], [3, 4]]
			             """);
		await That(await ContextsOf(Act)).IsEqualTo(await ContextsOf(async () => await That(subject).Contains(9)))
			.Because("the context is the same as the one of the built-in expectations");
	}

	[Test]
	public async Task AddCollectionContext_Untyped_WhenTheEnumerationThrows_ShouldListTheException()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new ThrowingEnumerable()));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [the enumeration did throw an InvalidOperationException: enumeration failed]
			             """);
	}

	[Test]
	public async Task AddCollectionContext_Untyped_WithoutCount_ShouldListTheItemsOnSeparateLines()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext((IEnumerable)Iterate(1, 2)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [
			               1,
			               2
			             ]
			             """)
			.Because("without a count the single line layout for few items cannot be chosen");
	}

	[Test]
	public async Task AddCollectionContext_WhenIncomplete_ShouldMarkTheItemsAsIncomplete()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _)
				=> contexts.AddCollectionContext(actual, true));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2, 3, (… and maybe more)]
			             """);
	}

	[Test]
	public async Task AddCollectionContext_WhenMaterializedItemsAreEmpty_ShouldNotAddAContext()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new MaterializedEnumerable([])));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not
			             """);
	}

	[Test]
	public async Task AddCollectionContext_WhenMaterializedItemsDidNotReachTheEnd_ShouldListThemAsIncomplete()
	{
		MaterializedEnumerable subject = new([1, 2,]);

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _)
				=> contexts.AddCollectionContext(actual));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2, (… and maybe more)]
			             """);
	}

	[Test]
	public async Task AddCollectionContext_WhenNull_ShouldNotAddAContext()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext((IEnumerable<int>?)null));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not
			             """);
	}

	[Test]
	public async Task AddCollectionContext_WhenTheCollectionIsKeyed_ShouldListTheItemsWithTheirKeys()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new KeyedCollection()));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [a: 1, b: 2]
			             """);
	}

	[Test]
	public async Task AddCollectionContext_WhenTheCountIsKnown_ShouldListTheItems()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new MaterializedEnumerable([1, 2,], 2)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2]
			             """);
	}

	[Test]
	public async Task AddCollectionContext_WithoutCount_ShouldListTheItemsOnSeparateLines()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(Iterate(1, 2)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [
			               1,
			               2
			             ]
			             """)
			.Because("without a count the single line layout for few items cannot be chosen");
	}

	[Test]
	public async Task AddCollectionContext_WithTotalCount_ShouldNameTheItemsThatAreNotListed()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(Enumerable.Range(1, 11).ToList(), totalCount: 20));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [
			               1,
			               2,
			               3,
			               4,
			               5,
			               6,
			               7,
			               8,
			               9,
			               10,
			               (… and 10 more)
			             ]
			             """);
	}

	[Test]
	public async Task AddCollectionContext_WithTotalCount_WhenTheItemsAreAReadOnlyCollection_ShouldNameTheItemsThatAreNotListed()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new ReadOnlyItems(Enumerable.Range(1, 11).ToArray()), totalCount: 20));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [
			               1,
			               2,
			               3,
			               4,
			               5,
			               6,
			               7,
			               8,
			               9,
			               10,
			               (… and 10 more)
			             ]
			             """);
	}

	[Test]
	public async Task AddCollectionContext_WithTotalCount_WhenTheItemsAreNoCollection_ShouldNameTheItemsThatAreNotListed()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(Iterate(Enumerable.Range(1, 11).ToArray()), totalCount: 20));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [
			               1,
			               2,
			               3,
			               4,
			               5,
			               6,
			               7,
			               8,
			               9,
			               10,
			               (… and 10 more)
			             ]
			             """);
	}

	[Test]
	public async Task AddCollectionContext_Untyped_WhenAGenericCollectionOfValueTypes_ShouldListTheItemsOnASingleLine()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext((IEnumerable)new HashSet<int>([1, 2,])));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2]
			             """)
			.Because("a collection that knows its count is laid out like a list");
	}

	[Test]
	public async Task AddCollectionContext_Untyped_WhenOnlyAReadOnlyCollection_ShouldListTheItemsOnASingleLine()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext((IEnumerable)new ReadOnlyItems([1, 2,])));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2]
			             """)
			.Because("a collection that knows its count is laid out like a list");
	}

	[Test]
	public async Task AddCollectionContext_Untyped_WhenTheCountThrows_ShouldListTheItemsOnSeparateLines()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext((IEnumerable)new ThrowingCountItems([1, 2,])));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [
			               1,
			               2
			             ]
			             """)
			.Because("a count that throws is an unknown count");
	}

	[Test]
	public async Task AddCollectionContext_WhenAQueue_ShouldListTheItemsOnASingleLine()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new Queue<int>([1, 2,])));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2]
			             """)
			.Because("a collection that knows its count is laid out like a list");
	}

	[Test]
	public async Task AddCollectionContext_WhenOnlyAReadOnlyCollection_ShouldListTheItemsOnASingleLine()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new ReadOnlyItems([1, 2,])));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2]
			             """)
			.Because("a collection that knows its count is laid out like a list");
	}

	[Test]
	public async Task AddCollectionContext_WhenTheCountThrows_ShouldListTheItemsOnSeparateLines()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new ThrowingCountItems([1, 2,])));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [
			               1,
			               2
			             ]
			             """)
			.Because("a count that throws is an unknown count");
	}

	[Test]
	public async Task AddCollectionContext_WithTotalCount_WhenTheCountThrows_ShouldListTheItemsOnSeparateLines()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new ThrowingCountItems([1, 2,]), totalCount: 20));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [
			               1,
			               2
			             ]
			             """)
			.Because("a count that throws is an unknown count");
	}

	[Test]
	public async Task AddCollectionContext_WithTotalCount_WhenTheEnumerationThrows_ShouldListTheException()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(Throwing(), totalCount: 20));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [
			               1,
			               (the enumeration did throw an InvalidOperationException: enumeration failed)
			             ]
			             """);
	}

	[Test]
	public async Task AddDictionaryContext_ShouldListTheEntriesLikeTheBuiltInExpectations()
	{
		Dictionary<string, int> subject = new()
		{
			{
				"a", 1
			},
			{
				"b", 2
			},
		};

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _) => contexts.AddDictionaryContext(actual));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Dictionary:
			             {["a"] = 1, ["b"] = 2}
			             """);
		await That(await ContextsOf(Act)).IsEqualTo(await ContextsOf(async () => await That(subject).ContainsKey("c")))
			.Because("the context is the same as the one of the built-in expectations");
	}

	[Test]
	public async Task AddDictionaryContext_WhenAlsoACollectionContextIsAdded_ShouldComeAfterIt()
	{
		Dictionary<string, int> subject = new()
		{
			{
				"a", 1
			},
		};

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _) =>
			{
				contexts.AddDictionaryContext(actual);
				contexts.AddCollectionContext(actual.Values);
			});

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1]

			             Dictionary:
			             {["a"] = 1}
			             """);
	}

	[Test]
	public async Task AddDictionaryContext_WhenOnlyAReadOnlyDictionary_ShouldListTheEntries()
	{
		ReadOnlyDictionaryOnly dictionary = new(new Dictionary<string, int>
		{
			{
				"a", 1
			},
			{
				"b", 2
			},
		});

		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, _) => contexts.AddDictionaryContext(dictionary));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not

			             Dictionary:
			             {["a"] = 1, ["b"] = 2}
			             """);
	}

	[Test]
	public async Task AddDictionaryContext_WhenTheCountOfAReadOnlyDictionaryThrows_ShouldListTheEntriesOnSeparateLines()
	{
		ThrowingCountReadOnlyDictionary dictionary = new(new Dictionary<string, int>
		{
			{
				"a", 1
			},
			{
				"b", 2
			},
		});

		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, _) => contexts.AddDictionaryContext(dictionary));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not

			             Dictionary:
			             {
			               ["a"] = 1,
			               ["b"] = 2
			             }
			             """)
			.Because("a count that throws is an unknown count");
	}

	[Test]
	public async Task AddDictionaryContext_WhenTheCountThrows_ShouldListTheEntriesOnSeparateLines()
	{
		ThrowingCountDictionary dictionary = new(new Dictionary<string, int>
		{
			{
				"a", 1
			},
			{
				"b", 2
			},
		});

		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, _) => contexts.AddDictionaryContext(dictionary));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not

			             Dictionary:
			             {
			               ["a"] = 1,
			               ["b"] = 2
			             }
			             """)
			.Because("a count that throws is an unknown count");
	}

	[Test]
	public async Task AddEqualityOptionsContexts_ShouldAddTheContextsOfTheMatchType()
	{
		ObjectEqualityOptions<int> options = new();
		options.SetMatchType(new EquivalencyMatchType(new EquivalencyOptions()), "Equivalent");

		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, _) => contexts.AddEqualityOptionsContexts(options));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not

			             Equivalency options:
			              - include public fields and properties
			             """);
	}

	[Test]
	public async Task AddEqualityOptionsContexts_WhenProvidedByAnOptionsProvider_ShouldAddTheContextsOfTheProvidedOptions()
	{
		ObjectEqualityOptions<int> options = new();
		options.SetMatchType(new EquivalencyMatchType(new EquivalencyOptions()), "Equivalent");
		EqualityOptionsProvider provider = new(options);

		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, _) => contexts.AddEqualityOptionsContexts(provider));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not

			             Equivalency options:
			              - include public fields and properties
			             """);
	}

	[Test]
	public async Task AddEqualityOptionsContexts_WhenTheMatchTypeHasNoContexts_ShouldNotAddAContext()
	{
		ObjectEqualityOptions<int> options = new();

		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, _) => contexts.AddEqualityOptionsContexts(options));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not
			             """);
	}

	[Test]
	public async Task AddEquivalencyContext_ShouldListTheOptionsLikeTheBuiltInExpectations()
	{
		int subject = 1;

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddEquivalencyContext(new EquivalencyOptions()));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Equivalency options:
			              - include public fields and properties
			             """);
		await That(await ContextsOf(Act)).IsEqualTo(await ContextsOf(async () => await That(subject).IsEquivalentTo(2)))
			.Because("the context is the same as the one of the built-in expectations");
	}

	[Test]
	public async Task AddExpectedValuesContext_ShouldListTheValuesLikeTheBuiltInExpectations()
	{
		int subject = 1;
		int[] values = [2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, result)
				=> contexts.AddExpectedValuesContext("values", values, result.Grammars));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Expected values:
			             [2, 3]
			             """);
		await That(await ContextsOf(Act)).IsEqualTo(await ContextsOf(async () => await That(subject).IsOneOf(values)))
			.Because("the context is the same as the one of the built-in expectations");
	}

	[Test]
	public async Task AddExpectedValuesContext_WhenNegated_ShouldListTheUnexpectedValuesLikeTheBuiltInExpectations()
	{
		int subject = 2;
		int[] values = [2, 3,];

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.ShowsContexts((contexts, _, result)
				=> contexts.AddExpectedValuesContext("values", values, result.Grammars), true));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not show contexts,
			             but it did

			             Unexpected values:
			             [2, 3]
			             """);
		await That(await ContextsOf(Act)).IsEqualTo(await ContextsOf(async () => await That(subject).IsNotOneOf(values)))
			.Because("the context is the same as the one of the built-in expectations");
	}

	[Test]
	public async Task AddExpectedValuesContext_WhenTheExpressionIsNull_ShouldNotAddAContext()
	{
		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, result)
				=> contexts.AddExpectedValuesContext(null, [2, 3,], result.Grammars));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not
			             """);
	}

	[Test]
	public async Task AddStringContext_ShouldShowTheFullValuesLikeTheBuiltInExpectations()
	{
		string subject = new('a', 101);
		string expected = new('b', 101);

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, result) =>
			{
				contexts.AddStringContext("Actual", actual, result);
				contexts.AddStringContext("Expected", expected, result);
			});

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that subject
			              shows contexts,
			              but it did not

			              Actual:
			              {subject}

			              Expected:
			              {expected}
			              """);
		await That(await ContextsOf(Act)).IsEqualTo(await ContextsOf(async () => await That(subject).IsEqualTo(expected)))
			.Because("the contexts are the same as the ones of the built-in expectations");
	}

	[Test]
	[Arguments("")]
	[Arguments(null)]
	public async Task AddStringContext_WhenEmpty_ShouldNotAddAContext(string? value)
	{
		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, result)
				=> contexts.AddStringContext("Value", value, result));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not
			             """);
	}

	[Test]
	public async Task AddStringContext_WhenTheMessageShowsTheValueCompletely_ShouldNotAddAContext()
	{
		async Task Act()
			=> await That("foo").ShowsContexts((contexts, _, result)
				=> contexts.AddStringContext("Expected", "bar", result), expectation: "is \"bar\"");

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that "foo"
			             is "bar",
			             but it did not
			             """);
	}

	private static async Task<string> ContextsOf(Func<Task> act)
	{
		string separator = Environment.NewLine + Environment.NewLine;
		try
		{
			await act();
		}
		catch (FailException exception)
		{
			int index = exception.Message.IndexOf(separator, StringComparison.Ordinal);
			return index < 0 ? "" : exception.Message.Substring(index + separator.Length);
		}

		throw new InvalidOperationException("The expectation did not fail.");
	}

	private static IEnumerable<int> Iterate(params int[] items)
	{
		foreach (int item in items)
		{
			yield return item;
		}
	}

	private sealed class EqualityOptionsProvider(ObjectEqualityOptions<int> options)
		: IOptionsEquality<int>, IOptionsProvider<IOptionsEquality<int>>
	{
		public ValueTask<bool> AreConsideredEqual<TExpected>(int actual, TExpected expected)
			=> options.AreConsideredEqual(actual, expected);

		public IOptionsEquality<int> Options => options;
	}

	private sealed class KeyedCollection : IEnumerable<int>, IKeyedCollection
	{
		public IEnumerator<int> GetEnumerator()
			=> throw new InvalidOperationException("The context must use the format of the keyed collection.");

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public string Format() => "[a: 1, b: 2]";

		public string Format(IEnumerable<int> indices, int? totalCount)
			=> throw new InvalidOperationException("The context must list all items.");
	}

	private sealed class MaterializedEnumerable(IReadOnlyList<int> items, int? count = null)
		: IMaterializedEnumerable<int>
	{
		public int? Count => count;
		public IReadOnlyList<int> MaterializedItems => items;

		public IEnumerator<int> GetEnumerator()
			=> count is null
				? throw new InvalidOperationException("The context must not read further items.")
				: items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private sealed class ReadOnlyDictionaryOnly(Dictionary<string, int> inner) : IReadOnlyDictionary<string, int>
	{
		public int Count => inner.Count;
		public int this[string key] => inner[key];
		public IEnumerable<string> Keys => inner.Keys;
		public IEnumerable<int> Values => inner.Values;
		public bool ContainsKey(string key) => inner.ContainsKey(key);
		public bool TryGetValue(string key, out int value) => inner.TryGetValue(key, out value);
		public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => inner.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private sealed class ReadOnlyItems(IReadOnlyList<int> items) : IReadOnlyCollection<int>
	{
		public int Count => items.Count;
		public IEnumerator<int> GetEnumerator() => items.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private sealed class ReadOnlyNullableItems(IReadOnlyList<int?> items) : IReadOnlyCollection<int?>
	{
		public int Count => items.Count;
		public IEnumerator<int?> GetEnumerator() => items.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private static IEnumerable<int> Throwing()
	{
		yield return 1;
		throw new InvalidOperationException("enumeration failed");
	}

	private sealed class ThrowingCountDictionary(Dictionary<string, int> inner) : IDictionary<string, int>
	{
		private ICollection<KeyValuePair<string, int>> Pairs => inner;
		public int Count => throw new InvalidOperationException("count failed");
		public bool IsReadOnly => false;
		public ICollection<string> Keys => inner.Keys;
		public ICollection<int> Values => inner.Values;

		public int this[string key]
		{
			get => inner[key];
			set => inner[key] = value;
		}

		public void Add(string key, int value) => inner.Add(key, value);
		public void Add(KeyValuePair<string, int> item) => Pairs.Add(item);
		public void Clear() => inner.Clear();
		public bool Contains(KeyValuePair<string, int> item) => Pairs.Contains(item);
		public bool ContainsKey(string key) => inner.ContainsKey(key);
		public void CopyTo(KeyValuePair<string, int>[] array, int arrayIndex) => Pairs.CopyTo(array, arrayIndex);
		public bool Remove(string key) => inner.Remove(key);
		public bool Remove(KeyValuePair<string, int> item) => Pairs.Remove(item);
		public bool TryGetValue(string key, out int value) => inner.TryGetValue(key, out value);
		public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => inner.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private sealed class ThrowingCountItems(int[] items) : ICollection<int>, ICollection
	{
		public bool IsSynchronized => false;
		public object SyncRoot => this;
		public void CopyTo(Array array, int index) => items.CopyTo(array, index);
		public int Count => throw new InvalidOperationException("count failed");
		public bool IsReadOnly => true;
		public void Add(int item) => throw new NotSupportedException();
		public void Clear() => throw new NotSupportedException();
		public bool Contains(int item) => Array.IndexOf(items, item) >= 0;
		public void CopyTo(int[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);
		public bool Remove(int item) => throw new NotSupportedException();
		public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)items).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private sealed class ThrowingCountReadOnlyDictionary(Dictionary<string, int> inner)
		: IReadOnlyDictionary<string, int>
	{
		public int Count => throw new InvalidOperationException("count failed");
		public int this[string key] => inner[key];
		public IEnumerable<string> Keys => inner.Keys;
		public IEnumerable<int> Values => inner.Values;
		public bool ContainsKey(string key) => inner.ContainsKey(key);
		public bool TryGetValue(string key, out int value) => inner.TryGetValue(key, out value);
		public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => inner.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private sealed class ThrowingEnumerable : IEnumerable
	{
		public IEnumerator GetEnumerator() => throw new InvalidOperationException("enumeration failed");
	}

	private sealed class UntypedMaterializedEnumerable(IReadOnlyList<object?> items, int? count = null)
		: IMaterializedEnumerable
	{
		public int? Count => count;
		public IReadOnlyList<object?> MaterializedItems => items;

		public IEnumerator GetEnumerator()
			=> count is null
				? throw new InvalidOperationException("The context must not read further items.")
				: items.GetEnumerator();
	}

#if NET8_0_OR_GREATER
	private sealed class MaterializedAsyncEnumerable(IReadOnlyList<int> items, int? count = null)
		: IMaterializedAsyncEnumerable<int>
	{
		public int? Count => count;
		public IReadOnlyList<int> MaterializedItems => items;

		public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
			=> throw new InvalidOperationException("The context must not receive further items.");

		public Task<IMaterializedAsyncEnumerable<int>> MaterializeItems(int? numberOfItems)
			=> throw new InvalidOperationException("The context must not receive further items.");
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task AddCollectionContext_WithMaterializedAsyncEnumerable_ShouldListTheReceivedItems()
	{
		MaterializedAsyncEnumerable subject = new([1, 2,]);

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _)
				=> contexts.AddCollectionContext(actual));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2, (… and maybe more)]
			             """);
	}

	[Test]
	public async Task AddCollectionContext_WithMaterializedAsyncEnumerable_WhenNothingWasReceived_ShouldNotAddAContext()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new MaterializedAsyncEnumerable([])));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not
			             """);
	}

	[Test]
	public async Task AddCollectionContext_WithMaterializedAsyncEnumerable_WhenTheCountIsKnown_ShouldListTheItems()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(new MaterializedAsyncEnumerable([1, 2,], 2)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2]
			             """);
	}
#endif
}

internal static class ResultContextCollectorExtensionsTestExtensions
{
	/// <summary>
	///     An expectation of a Core-only extension, which adds the contexts with the <paramref name="appendContexts" />
	///     callback when it fails.
	/// </summary>
	public static AndOrResult<T, IThat<T>> ShowsContexts<T>(this IThat<T> subject,
		Action<ResultContextCollector, T, ConstraintResult> appendContexts, bool isMet = false,
		string expectation = "shows contexts")
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new ShowsContextsConstraint<T>(it, grammars, appendContexts, isMet, expectation)),
			subject);

	private sealed class ShowsContextsConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		Action<ResultContextCollector, T, ConstraintResult> appendContexts,
		bool isMet,
		string expectation)
		: ConstraintResult.WithValue<T>(it, grammars),
			IValueConstraint<T>
	{
		public ConstraintResult IsMetBy(T actual)
		{
			Actual = actual;
			Outcome = isMet ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
			=> appendContexts(contexts, Actual!, this);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectation);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not show contexts");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
