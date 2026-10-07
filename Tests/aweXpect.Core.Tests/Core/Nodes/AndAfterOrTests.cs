using System.Collections.Generic;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core.Nodes;

public sealed class AndAfterOrTests
{
	private static readonly Outcome[] Outcomes =
	[
		Outcome.Success, Outcome.Failure, Outcome.Undecided,
	];

	private static readonly FurtherProcessingStrategy[] Strategies =
	[
		FurtherProcessingStrategy.Continue,
		FurtherProcessingStrategy.IgnoreResult,
		FurtherProcessingStrategy.IgnoreCompletely,
	];

	[Test]
	[Arguments(false)]
	[Arguments(true)]
	public async Task AndChainAfterOr_ShouldBeEvaluatedLikeTheSameChainOnItsOwn(bool isNegated)
	{
		List<string> differences = [];
		foreach ((Outcome, FurtherProcessingStrategy) first in Operands())
		{
			foreach ((Outcome, FurtherProcessingStrategy) second in Operands())
			{
				foreach ((Outcome, FurtherProcessingStrategy) third in Operands())
				{
					List<string> evaluated = [];
					ManualExpectationBuilder<int> chain = new();
					AddAndChain(chain, evaluated, first, second, third);
					ConstraintResult chainResult = await chain.IsMetBy(0, CancellationToken.None);
					string expected = Describe(evaluated, chainResult, isNegated, true);

					evaluated.Clear();
					ManualExpectationBuilder<int> sut = new();
					AddOperand(sut, evaluated, "x", Outcome.Failure);
					sut.Or();
					AddAndChain(sut, evaluated, first, second, third);
					ConstraintResult result = await sut.IsMetBy(0, CancellationToken.None);
					string actual = Describe(evaluated, result, isNegated, false);

					if (actual != expected)
					{
						differences.Add($"{first}, {second}, {third}: expected <{expected}>, but was <{actual}>");
					}
				}
			}
		}

		await That(differences).IsEmpty();
	}

	[Test]
	[Arguments(2)]
	[Arguments(3)]
	public async Task AndChainAfterOr_WhenOperandFailsWithIgnoreResult_ShouldNotEvaluateFollowingOperands(
		int guardPosition)
	{
		List<string> evaluated = [];
		ManualExpectationBuilder<int> sut = new();
		AddOperand(sut, evaluated, "x", Outcome.Failure);
		sut.Or();
		for (int position = 1; position <= 4; position++)
		{
			if (position > 1)
			{
				sut.And();
			}

			AddOperand(sut, evaluated, $"o{position}", position == guardPosition ? Outcome.Failure : Outcome.Success,
				position == guardPosition ? FurtherProcessingStrategy.IgnoreResult : FurtherProcessingStrategy.Continue);
		}

		ConstraintResult result = await sut.IsMetBy(0, CancellationToken.None);

		await That(evaluated).HasCount(guardPosition + 1);
		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(result.GetExpectationText()).IsEqualTo("x or o1 and o2 and o3 and o4");
		await That(result.GetResultText()).IsEqualTo($"not x and not o{guardPosition}");
	}

	[Test]
	public async Task AndChainAfterOr_WhenOperandIgnoresCompletely_ShouldOmitFollowingOperands()
	{
		List<string> evaluated = [];
		ManualExpectationBuilder<int> sut = new();
		AddOperand(sut, evaluated, "x", Outcome.Failure);
		sut.Or();
		AddOperand(sut, evaluated, "a", Outcome.Success);
		sut.And();
		AddOperand(sut, evaluated, "b", Outcome.Failure, FurtherProcessingStrategy.IgnoreCompletely);
		sut.And();
		AddOperand(sut, evaluated, "c", Outcome.Failure);

		ConstraintResult result = await sut.IsMetBy(0, CancellationToken.None);

		await That(string.Join(",", evaluated)).IsEqualTo("x,a,b");
		await That(result.GetExpectationText()).IsEqualTo("x or a and b");
		await That(result.GetResultText()).IsEqualTo("not x and not b");
	}

	[Test]
	public async Task AndWhoseAfterOr_WhenPrecedingMemberFails_ShouldContinueFromTheStoredValue()
	{
		async Task Act()
		{
			IThat<string?> subject = That("foo").IsEqualTo("bar").Or;
			await new AndOrWhoseResult<int, IThat<string?>>(((IExpectThat<string?>)subject).ExpectationBuilder
					.AddConstraint((_, _) => new DummyValueConstraint<string?>(value
						=> new DummyConstraintResult<int>(Outcome.Success, value!.Length, "has a length"))), subject)
				.Whose(length => length + 1, member => member.IsEqualTo(4))
				.AndWhose(length => length + 2, member => member.IsEqualTo(0))
				.AndWhose(length => length + 3, member => member.IsEqualTo(0));
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that "foo"
			             is equal to "bar" or has a length whose length + 1 is equal to 4 and whose length + 2 is equal to 0 and whose length + 3 is equal to 0,
			             but it was "foo", which differs at index 0:
			                ↓ (actual)
			               "foo"
			               "bar"
			                ↑ (expected)
			             and length + 2 was 5, which differs by 5 and length + 3 was 6, which differs by 6
			             """);
	}

	[Test]
	public async Task InsideWhose_WhenSubjectIsNull_ShouldNotEvaluateOperandAfterFailedNullCheck()
	{
		bool isEvaluated = false;
		MyClass subject = new();

		async Task Act()
			=> await That(subject).Whose(x => x.Value, value => value.IsEqualTo("foo").Or.IsNotEqualTo("bar").And
				.IsNotNull().And.Satisfies(_ => isEvaluated = true));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             whose Value is equal to "foo" or is not equal to "bar" and is not null and satisfies _ => isEvaluated = true,
			             but Value was <null>
			             """);
		await That(isEvaluated).IsFalse();
	}

	[Test]
	public async Task Negated_WhenSubjectIsNull_ShouldNotEvaluateOperandAfterFailedNullCheck()
	{
		bool isEvaluated = false;
		string? subject = null;

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo("foo").Or.IsNotEqualTo("bar").And.IsNotNull()
				.And.Satisfies(_ => isEvaluated = true));

		await That(Act).DoesNotThrow();
		await That(isEvaluated).IsFalse();
	}

	[Test]
	public async Task WhenSubjectIsNull_ShouldNotEvaluateOperandAfterFailedNullCheck()
	{
		bool isEvaluated = false;
		string? subject = null;

		async Task Act()
			=> await That(subject).IsEqualTo("foo").Or.IsNotEqualTo("bar").And.IsNotNull().And
				.Satisfies(_ => isEvaluated = true);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is equal to "foo" or is not equal to "bar" and is not null and satisfies _ => isEvaluated = true,
			             but it was <null>
			             """);
		await That(isEvaluated).IsFalse();
	}

	[Test]
	public async Task WithAndBeforeOr_WhenSubjectIsNull_ShouldNotEvaluateOperandAfterFailedNullCheck()
	{
		bool isEvaluated = false;
		string? subject = null;

		async Task Act()
			=> await That(subject).IsEqualTo("foo").And.IsEqualTo("baz").Or.IsNotEqualTo("bar").And.IsNotNull().And
				.Satisfies(_ => isEvaluated = true);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is equal to "foo" and is equal to "baz" or is not equal to "bar" and is not null and satisfies _ => isEvaluated = true,
			             but it was <null>
			             """);
		await That(isEvaluated).IsFalse();
	}

	private static IEnumerable<(Outcome, FurtherProcessingStrategy)> Operands()
	{
		foreach (Outcome outcome in Outcomes)
		{
			foreach (FurtherProcessingStrategy strategy in Strategies)
			{
				yield return (outcome, strategy);
			}
		}
	}

	/// <summary>
	///     Describes which operands were evaluated and the <paramref name="result" />. For the chain on its own it is
	///     described as it is expected after a failed <c>x.Or</c>.
	/// </summary>
	private static string Describe(List<string> evaluated, ConstraintResult result, bool isNegated, bool isChainOnly)
	{
		if (isNegated)
		{
			result.Negate();
		}

		string operands = string.Join(",", evaluated);
		string expectation = result.GetExpectationText();
		string resultText = result.Outcome == Outcome.Success ? "" : result.GetResultText();
		if (!isChainOnly)
		{
			return $"{operands}|{result.Outcome}|{expectation}|{resultText}";
		}

		if (!isNegated)
		{
			resultText = result.Outcome == Outcome.Success ? "" : $"not x and {resultText}";
			return $"x,{operands}|{result.Outcome}|x or {expectation}|{resultText}";
		}

		if (expectation.Contains(" or "))
		{
			expectation = $"({expectation})";
		}

		return $"x,{operands}|{result.Outcome}|x and {expectation}|{resultText}";
	}

	private static void AddAndChain(ExpectationBuilder builder, List<string> evaluated,
		params (Outcome Outcome, FurtherProcessingStrategy Strategy)[] operands)
	{
		for (int index = 0; index < operands.Length; index++)
		{
			if (index > 0)
			{
				builder.And();
			}

			AddOperand(builder, evaluated, $"o{index + 1}", operands[index].Outcome, operands[index].Strategy);
		}
	}

	private static void AddOperand(ExpectationBuilder builder, List<string> evaluated, string name, Outcome outcome,
		FurtherProcessingStrategy strategy = FurtherProcessingStrategy.Continue)
		=> builder.AddConstraint((_, _) => new DummyConstraint(name, () =>
		{
			evaluated.Add(name);
			return new DummyConstraintResult(outcome, name, $"not {name}", strategy);
		}));

	private sealed class MyClass
	{
		public string? Value { get; set; }
	}
}
