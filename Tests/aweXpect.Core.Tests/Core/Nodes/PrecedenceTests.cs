namespace aweXpect.Core.Tests.Core.Nodes;

public sealed class PrecedenceTests
{
	public sealed class Negated
	{
		[Fact]
		public async Task Not_F_and_T_and_F_ShouldSucceed()
		{
			async Task Act()
				=> await That(true).DoesNotComplyWith(it => it.IsFalse().And.IsTrue().And.IsFalse());

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Not_F_and_T_or_F_ShouldSucceed()
		{
			async Task Act()
				=> await That(true).DoesNotComplyWith(it => it.IsFalse().And.IsTrue().Or.IsFalse());

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Not_F_and_T_or_T_and_T_ShouldGroupBothAndOperands()
		{
			async Task Act()
				=> await That(true).DoesNotComplyWith(it => it.IsFalse().And.IsTrue().Or.IsTrue().And.IsTrue());

			await That(Act).Throws()
				.WithMessage("""
				             Expected that true
				             (is not False or is not True) and (is not True or is not True),
				             but it was True
				             """);
		}

		[Fact]
		public async Task Not_F_and_T_or_T_or_F_ShouldGroupTheAndOperandOnly()
		{
			async Task Act()
				=> await That(true).DoesNotComplyWith(it => it.IsFalse().And.IsTrue().Or.IsTrue().Or.IsFalse());

			await That(Act).Throws()
				.WithMessage("""
				             Expected that true
				             (is not False or is not True) and is not True and is not False,
				             but it was True
				             """);
		}

		[Fact]
		public async Task Not_F_and_T_or_T_ShouldGroupTheLeftAndOperand()
		{
			async Task Act()
				=> await That(true).DoesNotComplyWith(it => it.IsFalse().And.IsTrue().Or.IsTrue());

			await That(Act).Throws()
				.WithMessage("""
				             Expected that true
				             (is not False or is not True) and is not True,
				             but it was True
				             """)
				.Because("a negated `And` reads as `or`, which binds weaker than the `and` of the negated `Or`");
		}

		[Fact]
		public async Task Not_T_and_T_and_T_ShouldNotGroup()
		{
			async Task Act()
				=> await That(true).DoesNotComplyWith(it => it.IsTrue().And.IsTrue().And.IsTrue());

			await That(Act).Throws()
				.WithMessage("""
				             Expected that true
				             is not True or is not True or is not True,
				             but it was True
				             """);
		}

		[Fact]
		public async Task Not_T_or_F_and_T_ShouldGroupTheRightAndOperand()
		{
			async Task Act()
				=> await That(true).DoesNotComplyWith(it => it.IsTrue().Or.IsFalse().And.IsTrue());

			await That(Act).Throws()
				.WithMessage("""
				             Expected that true
				             is not True and (is not False or is not True),
				             but it was True
				             """);
		}

		[Fact]
		public async Task Not_T_or_F_or_F_ShouldNotGroup()
		{
			async Task Act()
				=> await That(true).DoesNotComplyWith(it => it.IsTrue().Or.IsFalse().Or.IsFalse());

			await That(Act).Throws()
				.WithMessage("""
				             Expected that true
				             is not True and is not False and is not False,
				             but it was True
				             """);
		}
	}

	public sealed class OrOverAnd
	{
		[Fact]
		public async Task F_and_T_or_F_ShouldFail()
		{
			async Task Act()
				=> await That(true).IsFalse().And.IsTrue().Or.IsFalse();

			await That(Act).Throws()
				.WithMessage("""
				             Expected that true
				             is False and is True or is False,
				             but it was True
				             """);
		}

		[Fact]
		public async Task F_and_T_or_T_and_F_ShouldFail()
		{
			async Task Act()
				=> await That(true).IsFalse().And.IsTrue().Or.IsTrue().And.IsFalse();

			await That(Act).Throws()
				.WithMessage("""
				             Expected that true
				             is False and is True or is True and is False,
				             but it was True
				             """);
		}

		[Fact]
		public async Task F_and_T_or_T_ShouldSucceed()
		{
			async Task Act()
				=> await That(true).IsFalse().And.IsTrue().Or.IsTrue();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task F_or_T_and_F_ShouldFail()
		{
			async Task Act()
				=> await That(true).IsFalse().Or.IsTrue().And.IsFalse();

			await That(Act).Throws()
				.WithMessage("""
				             Expected that true
				             is False or is True and is False,
				             but it was True
				             """);
		}

		[Fact]
		public async Task T_and_F_or_F_ShouldFail()
		{
			async Task Act()
				=> await That(true).IsTrue().And.IsFalse().Or.IsFalse();

			await That(Act).Throws()
				.WithMessage("""
				             Expected that true
				             is True and is False or is False,
				             but it was True
				             """);
		}

		[Fact]
		public async Task T_and_F_or_T_ShouldSucceed()
		{
			async Task Act()
				=> await That(true).IsTrue().And.IsFalse().Or.IsTrue();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task T_and_T_or_X_ShouldNotEvaluateX()
		{
			bool isEvaluated = false;

			async Task Act()
				=> await That(true).IsTrue().And.IsTrue().Or.Satisfies(_ => isEvaluated = true);

			await That(Act).DoesNotThrow();
			await That(isEvaluated).IsFalse()
				.Because("`And` binds tighter than `Or`, so the whole left branch already succeeded");
		}

		[Fact]
		public async Task T_or_T_and_F_ShouldSucceed()
		{
			async Task Act()
				=> await That(true).IsTrue().Or.IsTrue().And.IsFalse();

			await That(Act).DoesNotThrow();
		}
	}
}
