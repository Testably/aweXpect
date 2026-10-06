using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Equivalency;
using aweXpect.Formatting;
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
				=> contexts.AddCollectionContext<int>(new MaterializedEnumerable([])));

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
				=> contexts.AddCollectionContext<int>(new MaterializedEnumerable([1, 2,], 2)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2]
			             """);
	}

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
				=> contexts.AddCollectionContext<int>(new MaterializedAsyncEnumerable([])));

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
				=> contexts.AddCollectionContext<int>(new MaterializedAsyncEnumerable([1, 2,], 2)));

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
			{ "a", 1 },
			{ "b", 2 },
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
