using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Constraints;

public partial class ConstraintResultTests
{
	public sealed class ExpectationOnlyTests
	{
		[Test]
		public async Task NormalCase_ShouldNotHaveNegatedGrammarsFlagAndSuccessOutcome()
		{
			ConstraintResult sut = new ConstraintResult.ExpectationOnly<int>(ExpectationGrammars.Plural);

			await That(sut.Grammars).HasFlag(ExpectationGrammars.Plural);
			await That(sut.Grammars).DoesNotHaveFlag(ExpectationGrammars.Negated);
			await That(sut.Outcome).IsEqualTo(Outcome.Success);
		}


		[Test]
		public async Task SetOutcome_AfterInvert_ShouldBeInverted()
		{
			MyExpectationOnlyConstraintResult<int> sut = new(
				ExpectationGrammars.None, "(note)", "(not note)");
			sut.Invert();

			sut.SetOutcome(Outcome.Success);

			await That(sut.Outcome).IsEqualTo(Outcome.Failure)
				.Because("the outcome is set for the expectation that is not negated, like for ConstraintResult.WithValue<T>");
		}

		[Test]
		[Arguments(Outcome.Success)]
		[Arguments(Outcome.Failure)]
		[Arguments(Outcome.Undecided)]
		public async Task SetOutcome_ShouldBeForwardedToInner(Outcome outcome)
		{
			MyExpectationOnlyConstraintResult<int> sut = new(
				ExpectationGrammars.None, null, "negated-foo");

			sut.SetOutcome(outcome);

			await That(sut.Outcome).IsEqualTo(outcome);
		}

		[Test]
		[Arguments(Outcome.Success, Outcome.Failure)]
		[Arguments(Outcome.Failure, Outcome.Success)]
		[Arguments(Outcome.FailureBothWays, Outcome.FailureBothWays)]
		[Arguments(Outcome.Undecided, Outcome.Undecided)]
		public async Task SetOutcome_WhenInverted_ShouldInvertSuccessAndFailure(Outcome outcome, Outcome expected)
		{
			MyExpectationOnlyConstraintResult<int> sut = new(
				ExpectationGrammars.None, "(note)", "(not note)");
			sut.SetOutcome(outcome);

			sut.Invert();

			await That(sut.Outcome).IsEqualTo(expected)
				.Because("the negated text is rendered, so the verdict must be negated as well");
		}

		[Test]
		public async Task TryGetValue_ShouldReturnFalse()
		{
			ConstraintResult sut = new ConstraintResult.ExpectationOnly<int>(ExpectationGrammars.None);

			bool result = sut.TryGetValue(out int value);

			await That(result).IsFalse();
			await That(value).IsEqualTo(0);
		}

		[Test]
		public async Task WhenInverted_ShouldHaveNegatedGrammarsFlagAndSuccessOutcome()
		{
			ConstraintResult sut = new ConstraintResult.ExpectationOnly<int>(ExpectationGrammars.Nested);

			sut = sut.Invert();

			await That(sut.Grammars).HasFlag(ExpectationGrammars.Nested);
			await That(sut.Grammars).HasFlag(ExpectationGrammars.Negated);
			await That(sut.Outcome).IsEqualTo(Outcome.Success);
		}

		[Test]
		public async Task WhenInverted_WithExpectationText_ShouldAppendNegatedExpectationText()
		{
			ConstraintResult sut = new ConstraintResult.ExpectationOnly<int>(
				ExpectationGrammars.None, "foo", "negated-foo").Invert();

			string expectationText = sut.GetExpectationText();
			string resultText = sut.GetResultText();

			await That(expectationText).IsEqualTo("negated-foo");
			await That(resultText).IsEmpty();
		}

		[Test]
		public async Task WhenInverted_WithNullAsExpectationText_ShouldAppendNegatedExpectationText()
		{
			ConstraintResult sut = new ConstraintResult.ExpectationOnly<int>(
				ExpectationGrammars.None, "foo").Invert();

			string expectationText = sut.GetExpectationText();
			string resultText = sut.GetResultText();

			await That(expectationText).IsEmpty();
			await That(resultText).IsEmpty();
		}

		[Test]
		public async Task WithExpectationText_ShouldAppendExpectationText()
		{
			ConstraintResult sut = new ConstraintResult.ExpectationOnly<int>(
				ExpectationGrammars.None, "foo", "negated-foo");

			string expectationText = sut.GetExpectationText();
			string resultText = sut.GetResultText();

			await That(expectationText).IsEqualTo("foo");
			await That(resultText).IsEmpty();
		}

		[Test]
		public async Task WithNullAsExpectationText_ShouldKeepEmptyExpectationText()
		{
			ConstraintResult sut = new ConstraintResult.ExpectationOnly<int>(
				ExpectationGrammars.None, null, "negated-foo");

			string expectationText = sut.GetExpectationText();
			string resultText = sut.GetResultText();

			await That(expectationText).IsEmpty();
			await That(resultText).IsEmpty();
		}

		[Test]
		public async Task InAnd_WhenNegatedAndOtherExpectationIsMet_ShouldFail()
		{
			async Task Act()
				=> await That(1).DoesNotComplyWith(it =>
				{
					it.IsEqualTo(1);
					AddNote(((IExpectThat<int>)it).ExpectationBuilder.And(" "));
				});

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             is not equal to 1 (negated note),
				             but it was 1
				             """);
		}

		[Test]
		public async Task InAnd_WhenOtherExpectationIsNotMet_ShouldFail()
		{
			async Task Act()
				=> await That(1).CompliesWith(it =>
				{
					it.IsEqualTo(2);
					AddNote(((IExpectThat<int>)it).ExpectationBuilder.And(" "));
				});

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             is equal to 2 (note),
				             but it was 1, which differs by -1
				             """);
		}

		[Test]
		public async Task InAnd_WithOutcomeSetByDerivedClass_ShouldTakePartInTheCombination()
		{
			MyExpectationOnlyConstraintResult<int> derived = new(ExpectationGrammars.None, "(note)");
			derived.SetOutcome(Outcome.Failure);
			AndNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Success)));
			node.AddNode(new DummyNode("", () => derived));

			ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

			await That(result.Outcome).IsEqualTo(Outcome.Failure);
		}

		[Test]
		public async Task InAnd_WhenNegated_WithOutcomeSetByDerivedClass_ShouldTakePartInTheCombination()
		{
			MyExpectationOnlyConstraintResult<int> derived = new(ExpectationGrammars.None, "(note)", "(not note)");
			derived.SetOutcome(Outcome.Failure);
			AndNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Success)));
			node.AddNode(new DummyNode("", () => derived));

			ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
			result.Negate();

			await That(result.Outcome).IsEqualTo(Outcome.Success)
				.Because("the negation of a failed operand is met");
		}

		[Test]
		public async Task InAnd_WhenNegated_WithSuccessSetByDerivedClass_ShouldFail()
		{
			async Task Act()
				=> await That(1).DoesNotComplyWith(it =>
				{
					it.IsEqualTo(1);
					((IExpectThat<int>)it).ExpectationBuilder.And(" ").AddConstraint((_, grammars) =>
					{
						MyExpectationOnlyConstraintResult<int> note = new(grammars, "(note)", "(not note)");
						note.SetOutcome(Outcome.Success);
						return note;
					});
				});

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             is not equal to 1 (not note),
				             but it was 1*
				             """).AsWildcard()
				.Because("both operands are met, so their negation fails");
		}

		[Test]
		public async Task InOr_WhenNegatedAndOtherExpectationIsNotMet_ShouldSucceed()
		{
			async Task Act()
				=> await That(1).DoesNotComplyWith(it =>
				{
					it.IsEqualTo(2);
					ExpectationBuilder expectationBuilder = ((IExpectThat<int>)it).ExpectationBuilder;
					expectationBuilder.Or();
					AddNote(expectationBuilder);
				});

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task InOr_WhenOtherExpectationIsNotMet_ShouldFail()
		{
			async Task Act()
				=> await That(1).CompliesWith(it =>
				{
					it.IsEqualTo(2);
					ExpectationBuilder expectationBuilder = ((IExpectThat<int>)it).ExpectationBuilder;
					expectationBuilder.Or();
					AddNote(expectationBuilder);
				});

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             is equal to 2 or (note),
				             but it was 1, which differs by -1
				             """);
		}

		private static void AddNote(ExpectationBuilder expectationBuilder)
			=> expectationBuilder.AddConstraint((_, grammars)
				=> new ConstraintResult.ExpectationOnly<int>(grammars, "(note)", "(negated note)"));

		private class MyExpectationOnlyConstraintResult<T>(
			ExpectationGrammars grammars,
			string? expectation = null,
			string? negatedExpectation = null)
			: ConstraintResult.ExpectationOnly<T>(grammars, expectation, negatedExpectation)
		{
			public void SetOutcome(Outcome outcome) => Outcome = outcome;
		}
	}
}
