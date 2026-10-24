using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatType
{
	public sealed class HasOperator
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenOperatorIsOnlyInherited_Directly_ShouldFail()
			{
				Type subject = typeof(OperatorDerivedClass);

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Subtraction).Directly();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              declares the operator Subtraction,
					              but it did not declare the operator Subtraction {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenOperatorIsOnlyInherited_DoesNotHave_Directly_ShouldSucceed()
			{
				Type subject = typeof(OperatorDerivedClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator(Operator.Subtraction).Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the operators declared on the type count");
			}

			[Test]
			public async Task WhenOperatorIsOnlyInherited_DoesNotHave_ShouldFail()
			{
				Type subject = typeof(OperatorDerivedClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator(Operator.Subtraction);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have the operator Subtraction,
					              but it had the operator Subtraction {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenOperatorIsOnlyInherited_ShouldSucceed()
			{
				Type subject = typeof(OperatorDerivedClass);

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Subtraction);
				}

				await That(Act).DoesNotThrow()
					.Because("operators inherited from base types are considered by default");
			}

			[Test]
			public async Task WhenTypeDoesNotHaveTheOperator_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Modulus);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              has the operator Modulus,
					              but it did not have the operator Modulus {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenTypeHasTheOperator_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has the operator Addition,
					             but it was <null>
					             """);
			}
		}

		public sealed class WithOperandTests
		{
			[Test]
			public async Task WhenTypeDoesNotHaveTheOperatorWithOperand_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasOperator<string>(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              has the operator Addition with operand {Formatter.Format(typeof(string))},
					              but it did not have the operator Addition with operand {Formatter.Format(typeof(string))} {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenTypeHasTheOperatorWithGenericOperand_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasOperator<int>(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasTheOperatorWithOpenGenericOperand_ShouldSucceed()
			{
				Type subject = typeof(GenericBox<int>);

				async Task Act()
				{
					await That(subject).HasOperator(Operator.Addition, typeof(GenericBox<>));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
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
			[Test]
			public async Task WhenTypeDoesNotHaveTheOperator_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator(Operator.Modulus);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasTheOperator_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              does not have the operator Addition,
					              but it had the operator Addition {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class DoesNotHaveOperatorWithOperandTests
		{
			[Test]
			public async Task WhenTypeDoesNotHaveTheOperatorWithOperand_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator<string>(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasTheOperatorWithGenericOperand_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator<int>(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              does not have the operator Addition with operand {Formatter.Format(typeof(int))},
					              but it had the operator Addition with operand {Formatter.Format(typeof(int))} {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenTypeHasTheOperatorWithTypeOperand_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveOperator(Operator.Addition, typeof(Money));
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              does not have the operator Addition with operand {Formatter.Format(typeof(Money))},
					              but it had the operator Addition with operand {Formatter.Format(typeof(Money))} {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeDoesNotHaveTheOperator_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasOperator(Operator.Modulus));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasTheOperator_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasOperator(Operator.Addition));
				}

				await That(Act).Throws()
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
