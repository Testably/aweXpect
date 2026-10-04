using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Equivalency;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core;

public sealed class ResultContextCollectorExtensionsTests
{
	[Fact]
	public async Task AddCollectionContext_ShouldListTheItemsLikeTheBuiltInExpectations()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _) => contexts.AddCollectionContext(actual));

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task AddCollectionContext_Untyped_ShouldListTheItemsLikeTheBuiltInExpectations()
	{
		IEnumerable subject = new ArrayList { 1, 2, 3, };

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _) => contexts.AddCollectionContext(actual));

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task AddCollectionContext_WhenIncomplete_ShouldMarkTheItemsAsIncomplete()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _)
				=> contexts.AddCollectionContext(actual, true));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2, 3, (… and maybe more)]
			             """);
	}

	[Fact]
	public async Task AddCollectionContext_WhenMaterializedItemsDidNotReachTheEnd_ShouldListThemAsIncomplete()
	{
		MaterializedEnumerable subject = new([1, 2,]);

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _)
				=> contexts.AddCollectionContext<int>(actual));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2, (… and maybe more)]
			             """);
	}

	[Fact]
	public async Task AddCollectionContext_WhenNull_ShouldNotAddAContext()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext((IEnumerable<int>?)null));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not
			             """);
	}

#if NET8_0_OR_GREATER
	[Fact]
	public async Task AddCollectionContext_WithMaterializedAsyncEnumerable_ShouldListTheReceivedItems()
	{
		MaterializedAsyncEnumerable subject = new([1, 2,]);

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _)
				=> contexts.AddCollectionContext<int>(actual));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             shows contexts,
			             but it did not

			             Collection:
			             [1, 2, (… and maybe more)]
			             """);
	}
#endif

	[Fact]
	public async Task AddCollectionContext_WithTotalCount_ShouldNameTheItemsThatAreNotListed()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddCollectionContext(Enumerable.Range(1, 11).ToList(), totalCount: 20));

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task AddDictionaryContext_ShouldListTheEntriesLikeTheBuiltInExpectations()
	{
		Dictionary<string, int> subject = new()
		{
			{ "a", 1 },
			{ "b", 2 },
		};

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _) => contexts.AddDictionaryContext(actual));

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task AddDictionaryContext_WhenAlsoACollectionContextIsAdded_ShouldComeAfterIt()
	{
		Dictionary<string, int> subject = new()
		{
			{ "a", 1 },
		};

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, actual, _) =>
			{
				contexts.AddDictionaryContext(actual);
				contexts.AddCollectionContext(actual.Values);
			});

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task AddEqualityOptionsContexts_ShouldAddTheContextsOfTheMatchType()
	{
		ObjectEqualityOptions<int> options = new();
		options.SetMatchType(new EquivalencyMatchType(new EquivalencyOptions()), "Equivalent");

		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, _) => contexts.AddEqualityOptionsContexts(options));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not

			             Equivalency options:
			              - include public fields and properties
			             """);
	}

	[Fact]
	public async Task AddEqualityOptionsContexts_WhenTheMatchTypeHasNoContexts_ShouldNotAddAContext()
	{
		ObjectEqualityOptions<int> options = new();

		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, _) => contexts.AddEqualityOptionsContexts(options));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not
			             """);
	}

	[Fact]
	public async Task AddEquivalencyContext_ShouldListTheOptionsLikeTheBuiltInExpectations()
	{
		int subject = 1;

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, _)
				=> contexts.AddEquivalencyContext(new EquivalencyOptions()));

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task AddExpectedValuesContext_ShouldListTheValuesLikeTheBuiltInExpectations()
	{
		int subject = 1;
		int[] values = [2, 3,];

		async Task Act()
			=> await That(subject).ShowsContexts((contexts, _, result)
				=> contexts.AddExpectedValuesContext("values", values, result.Grammars));

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task AddExpectedValuesContext_WhenNegated_ShouldListTheUnexpectedValuesLikeTheBuiltInExpectations()
	{
		int subject = 2;
		int[] values = [2, 3,];

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.ShowsContexts((contexts, _, result)
				=> contexts.AddExpectedValuesContext("values", values, result.Grammars), true));

		await That(Act).Throws<XunitException>()
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

	[Fact]
	public async Task AddExpectedValuesContext_WhenTheExpressionIsNull_ShouldNotAddAContext()
	{
		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, result)
				=> contexts.AddExpectedValuesContext(null, [2, 3,], result.Grammars));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not
			             """);
	}

	[Fact]
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

		await That(Act).Throws<XunitException>()
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

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public async Task AddStringContext_WhenEmpty_ShouldNotAddAContext(string? value)
	{
		async Task Act()
			=> await That(1).ShowsContexts((contexts, _, result)
				=> contexts.AddStringContext("Value", value, result));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that 1
			             shows contexts,
			             but it did not
			             """);
	}

	[Fact]
	public async Task AddStringContext_WhenTheMessageShowsTheValueCompletely_ShouldNotAddAContext()
	{
		async Task Act()
			=> await That("foo").ShowsContexts((contexts, _, result)
				=> contexts.AddStringContext("Expected", "bar", result), expectation: "is \"bar\"");

		await That(Act).Throws<XunitException>()
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
		catch (XunitException exception)
		{
			int index = exception.Message.IndexOf(separator, StringComparison.Ordinal);
			return index < 0 ? "" : exception.Message.Substring(index + separator.Length);
		}

		throw new InvalidOperationException("The expectation did not fail.");
	}

	private sealed class MaterializedEnumerable(IReadOnlyList<int> items) : IMaterializedEnumerable<int>
	{
		public int? Count => null;
		public IReadOnlyList<int> MaterializedItems => items;

		public IEnumerator<int> GetEnumerator()
			=> throw new InvalidOperationException("The context must not read further items.");

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

#if NET8_0_OR_GREATER
	private sealed class MaterializedAsyncEnumerable(IReadOnlyList<int> items) : IMaterializedAsyncEnumerable<int>
	{
		public int? Count => null;
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
