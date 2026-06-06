using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatType
{
	public sealed class HasOperator
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenOperatorIsOnlyInherited_WithInherit_ShouldSucceed()
			{
				Type subject = typeof(OperatorDerivedClass);

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Subtraction, true);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenOperatorIsOnlyInherited_WithoutInherit_ShouldFail()
			{
				Type subject = typeof(OperatorDerivedClass);

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Subtraction);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              has the operator Subtraction,
					              but it did not have the operator Subtraction {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeDoesNotHaveTheOperator_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Modulus);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              has the operator Modulus,
					              but it did not have the operator Modulus {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeHasTheOperator_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Addition);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has the operator Addition,
					             but it was <null>
					             """);
			}
		}

		public sealed class WithOperandTests
		{
			[Fact]
			public async Task WhenTypeDoesNotHaveTheOperatorWithOperand_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasOperator<string>(Operator.Addition);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              has the operator Addition with operand {Formatter.Format(typeof(string))},
					              but it did not have the operator Addition with operand {Formatter.Format(typeof(string))} {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeHasTheOperatorWithGenericOperand_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasOperator<int>(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasTheOperatorWithOpenGenericOperand_ShouldSucceed()
			{
				Type subject = typeof(GenericBox<int>);

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Addition, typeof(GenericBox<>));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasTheOperatorWithTypeOperand_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Addition, typeof(Money));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class DoesNotHaveOperatorTests
		{
			[Fact]
			public async Task WhenTypeDoesNotHaveTheOperator_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator(Operator.Modulus);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasTheOperator_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator(Operator.Addition);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not have the operator Addition,
					              but it had the operator Addition {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class DoesNotHaveOperatorWithOperandTests
		{
			[Fact]
			public async Task WhenTypeDoesNotHaveTheOperatorWithOperand_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator<string>(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasTheOperatorWithGenericOperand_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator<int>(Operator.Addition);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not have the operator Addition with operand {Formatter.Format(typeof(int))},
					              but it had the operator Addition with operand {Formatter.Format(typeof(int))} {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeHasTheOperatorWithTypeOperand_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator(Operator.Addition, typeof(Money));
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not have the operator Addition with operand {Formatter.Format(typeof(Money))},
					              but it had the operator Addition with operand {Formatter.Format(typeof(Money))} {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeDoesNotHaveTheOperator_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasOperator(Operator.Modulus));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasTheOperator_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasOperator(Operator.Addition));
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not have the operator Addition,
					              but it had the operator Addition {Formatter.Format(subject)}
					              """);
			}
		}
	}
}

#pragma warning restore CA2263 // Prefer generic overload when type is known
