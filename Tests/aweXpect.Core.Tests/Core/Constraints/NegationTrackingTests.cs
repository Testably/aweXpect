using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Constraints;

public sealed class NegationTrackingTests
{
	[Test]
	public async Task DoesNotComplyWith_ShouldNegateTheConstraint()
	{
		SideEffectLog log = new();

		async Task Act()
			=> await That(1).DoesNotComplyWith(it => it.HasSideEffect(log, "a"));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             has no side effect a,
			             but it did
			             """);
		await That(log.Effects).IsEqualTo(["a does nothing (whole: Failure, own: Failure, negated: True)"])
			.Because("the negated text in the message and the negation tracked by the constraint must agree");
	}

	[Test]
	public async Task DoesNotComplyWith_WhenNestedTwice_ShouldNotNegateTheConstraint()
	{
		SideEffectLog log = new();

		async Task Act()
			=> await That(1).DoesNotComplyWith(it
				=> it.DoesNotComplyWith(x => x.HasSideEffect(log, "a", Outcome.Failure)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             has side effect a,
			             but it did not
			             """);
		await That(log.Effects)
			.IsEqualTo(["a writes the pending snapshot (whole: Failure, own: Failure, negated: False)"])
			.Because("two negations cancel each other out");
	}

	[Test]
	public async Task DoesNotComplyWith_WithAll_ShouldNotNegateTheConstraintOfTheItems()
	{
		SideEffectLog log = new();

		async Task Act()
			=> await That(new[] { 1, }).DoesNotComplyWith(c => c.All().ComplyWith(it => it.HasSideEffect(log, "a")));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that new[] { 1, }
			             has side effect a not for all items,
			             but all 1 did

			             Collection:
			             [1]
			             """);
		await That(log.Effects).IsEqualTo(["a does nothing (whole: Failure, own: Success, negated: False)"])
			.Because("a quantifier classifies the items by the item expectations without negating them");
	}

	[Test]
	public async Task DoesNotComplyWith_WithAnd_ShouldNegateBothConstraints()
	{
		SideEffectLog log = new();

		async Task Act()
			=> await That(1).DoesNotComplyWith(it => it.HasSideEffect(log, "a").And.HasSideEffect(log, "b"));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             has no side effect a or has no side effect b,
			             but it did
			             """);
		await That(log.Effects).IsEqualTo([
			"a does nothing (whole: Failure, own: Failure, negated: True)",
			"b does nothing (whole: Failure, own: Failure, negated: True)",
		]).Because("De Morgan negates both operands of the junction");
	}

	[Test]
	public async Task DoesNotComplyWith_WithOr_ShouldNegateBothConstraints()
	{
		SideEffectLog log = new();

		async Task Act()
			=> await That(1).DoesNotComplyWith(it
				=> it.HasSideEffect(log, "a", Outcome.Failure).Or.HasSideEffect(log, "b"));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             has no side effect a and has no side effect b,
			             but it did
			             """);
		await That(log.Effects).IsEqualTo([
			"a does nothing (whole: Failure, own: Success, negated: True)",
			"b does nothing (whole: Failure, own: Failure, negated: True)",
		]).Because("De Morgan negates both operands of the junction");
	}

	[Test]
	public async Task DoesNotComplyWith_WithWhich_ShouldNegateTheConstraintsOnTheSubjectAndTheMember()
	{
		SideEffectLog log = new();
		Pair sut = new("b");

		async Task Act()
			=> await That(sut).DoesNotComplyWith(it => it.Get().ExpectationBuilder
				.AddConstraint((itName, grammars) => new SideEffectConstraint<Pair>(itName, grammars, log, "a",
					_ => Outcome.Success, null))
				.ForWhich<Pair, string>(p => p.Second, " which ")
				.AddConstraint((itName, grammars) => new SideEffectConstraint<string>(itName, grammars, log, "b",
					_ => Outcome.Success, null)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             has no side effect a which has side effect b,
			             but it did
			             """);
		await That(log.Effects).IsEqualTo([
			"a does nothing (whole: Failure, own: Failure, negated: True)",
			"b does nothing (whole: Failure, own: Failure, negated: True)",
		]).Because("the member keeps its text, but it is negated as part of the negated phrase");
	}

	[Test]
	public async Task DoesNotComplyWith_WithWhichNegatingTheMemberOnly_ShouldOnlyNegateTheConstraintOnTheMember()
	{
		SideEffectLog log = new();
		Pair sut = new("b");

		async Task Act()
			=> await That(sut).DoesNotComplyWith(it => it.Get().ExpectationBuilder
				.AddConstraint((itName, grammars) => new SideEffectConstraint<Pair>(itName, grammars, log, "a",
					_ => Outcome.Success, null))
				.ForWhich<Pair, string>(p => p.Second, " which ", negateMemberOnly: true)
				.AddConstraint((itName, grammars) => new SideEffectConstraint<string>(itName, grammars, log, "b",
					_ => Outcome.Success, null)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             has side effect a which has no side effect b,
			             but it did
			             """);
		await That(log.Effects).IsEqualTo([
			"a does nothing (whole: Failure, own: Success, negated: False)",
			"b does nothing (whole: Failure, own: Failure, negated: True)",
		]).Because("the negation only applies to the member");
	}

	[Test]
	public async Task DoesNotComplyWith_WithWhose_ShouldNegateTheConstraintsOnTheSubjectAndTheMember()
	{
		SideEffectLog log = new();
		Pair sut = new("b");

		async Task Act()
			=> await That(sut).DoesNotComplyWith(it => it.HasSideEffect(log, "a")
				.Whose(p => p.Second, second => second.HasSideEffect(log, "b")));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that sut
			             has no side effect a whose Second has side effect b,
			             but it did
			             """);
		await That(log.Effects).IsEqualTo([
			"a does nothing (whole: Failure, own: Failure, negated: True)",
			"b does nothing (whole: Failure, own: Failure, negated: True)",
		]).Because("the member keeps its text, but it is negated as part of the negated phrase");
	}

	[Test]
	public async Task Or_ShouldNotNegateTheConstraints()
	{
		SideEffectLog log = new();

		async Task Act()
			=> await That(1).HasSideEffect(log, "a", Outcome.Failure).Or.HasSideEffect(log, "b", Outcome.Failure);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that 1
			             has side effect a or has side effect b,
			             but it did not
			             """);
		await That(log.Effects).IsEqualTo([
			"a writes the pending snapshot (whole: Failure, own: Failure, negated: False)",
			"b writes the pending snapshot (whole: Failure, own: Failure, negated: False)",
		]);
	}

	private sealed class Pair(string second)
	{
		public string Second { get; } = second;
	}
}
