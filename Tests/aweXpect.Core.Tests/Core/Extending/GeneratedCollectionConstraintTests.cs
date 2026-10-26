using System.Collections.Generic;
using System.Text;
using System.Threading;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core.Extending;

public sealed class GeneratedCollectionConstraintTests
{
#if NET8_0_OR_GREATER
	[Test]
	public async Task AsyncEnumerable_OnEvaluating_ShouldBeCalledBeforeTheItemsAreRead()
	{
		List<string> calls = [];
		IThat<IAsyncEnumerable<int>?> that = That(ReadAsync(calls, 1, 2));

		async Task Act()
			=> await new ExpectationResult(that.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new RecordingAsyncConstraint(it, grammars, calls)));

		await That(Act).DoesNotThrow();
		await That(calls).IsEqualTo(["evaluating", "read 1", "match 1", "read 2", "match 2",]);
	}

	[Test]
	public async Task AsyncEnumerable_OnEvaluating_WhenItThrows_ShouldNotReadTheItems()
	{
		List<string> calls = [];
		IThat<IAsyncEnumerable<int>?> that = That(ReadAsync(calls, 1, 2));

		async Task Act()
			=> await new ExpectationResult(that.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new RecordingAsyncConstraint(it, grammars, calls, true)));

		await That(Act).Throws<ArgumentException>().WithMessage("unusable expected value");
		await That(calls).IsEqualTo(["evaluating",]);
	}
#endif

	[Test]
	public async Task OnEvaluating_ShouldBeCalledBeforeTheItemsAreRead()
	{
		List<string> calls = [];
		IThat<IEnumerable<int>?> that = That(Read(calls, 1, 2));

		async Task Act()
			=> await new ExpectationResult(that.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new RecordingConstraint(it, grammars, calls)));

		await That(Act).DoesNotThrow();
		await That(calls).IsEqualTo(["evaluating", "read 1", "match 1", "read 2", "match 2",]);
	}

	[Test]
	public async Task OnEvaluating_WhenItThrows_ShouldNotReadTheItems()
	{
		List<string> calls = [];
		IThat<IEnumerable<int>?> that = That(Read(calls, 1, 2));

		async Task Act()
			=> await new ExpectationResult(that.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new RecordingConstraint(it, grammars, calls, true)));

		await That(Act).Throws<ArgumentException>().WithMessage("unusable expected value");
		await That(calls).IsEqualTo(["evaluating",]);
	}

	private static IEnumerable<int> Read(List<string> calls, params int[] values)
	{
		foreach (int value in values)
		{
			calls.Add($"read {value}");
			yield return value;
		}
	}

	private sealed class RecordingConstraint(
		string it,
		ExpectationGrammars grammars,
		List<string> calls,
		bool throws = false)
		: GeneratedCollectionConstraint<IEnumerable<int>?, int>(it, grammars, EnumerableQuantifier.All())
	{
		protected override string Verb => "were";

		protected override void OnEvaluating(IEvaluationContext context, CancellationToken cancellationToken)
		{
			calls.Add("evaluating");
			if (throws)
			{
				throw new ArgumentException("unusable expected value");
			}
		}

		protected override ValueTask<PredicateResult> IsMatch(int item, CancellationToken cancellationToken)
		{
			calls.Add($"match {item}");
			return new ValueTask<PredicateResult>(true);
		}

		protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
			string? indentation)
			=> stringBuilder.Append("is recorded");
	}

#if NET8_0_OR_GREATER
	private static async IAsyncEnumerable<int> ReadAsync(List<string> calls, params int[] values)
	{
		foreach (int value in values)
		{
			await Task.Yield();
			calls.Add($"read {value}");
			yield return value;
		}
	}

	private sealed class RecordingAsyncConstraint(
		string it,
		ExpectationGrammars grammars,
		List<string> calls,
		bool throws = false)
		: GeneratedAsyncCollectionConstraint<int>(it, grammars, EnumerableQuantifier.All())
	{
		protected override string Verb => "were";

		protected override void OnEvaluating(IEvaluationContext context, CancellationToken cancellationToken)
		{
			calls.Add("evaluating");
			if (throws)
			{
				throw new ArgumentException("unusable expected value");
			}
		}

		protected override ValueTask<PredicateResult> IsMatch(int item, CancellationToken cancellationToken)
		{
			calls.Add($"match {item}");
			return new ValueTask<PredicateResult>(true);
		}

		protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
			string? indentation)
			=> stringBuilder.Append("is recorded");
	}
#endif
}
