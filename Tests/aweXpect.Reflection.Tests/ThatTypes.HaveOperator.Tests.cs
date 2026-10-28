using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatTypes
{
	public sealed partial class HaveOperator
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesHaveTheOperator_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).HaveOperator(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(Money), null,];

				async Task Act()
				{
					await That(subject).HaveOperator(Operator.Addition);
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveOperator(Operator.Addition));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have the operator Addition,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               Money,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have the operator Addition,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               Money,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenOperatorIsOnlyInherited_Directly_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(OperatorBaseClass), typeof(OperatorDerivedClass),
				};

				async Task Act()
				{
					await That(subject).HaveOperator(Operator.Subtraction).Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all declare the operator Subtraction,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               OperatorDerivedClass
					             ]

					             Collection:
					             [
					               OperatorBaseClass,
					               OperatorDerivedClass
					             ]
					             """);
			}

			[Test]
			public async Task WhenOperatorIsOnlyInherited_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(OperatorBaseClass), typeof(OperatorDerivedClass),
				};

				async Task Act()
				{
					await That(subject).HaveOperator(Operator.Subtraction);
				}

				await That(Act).DoesNotThrow()
					.Because("operators inherited from base types are considered by default");
			}

			[Test]
			public async Task WhenSomeTypeDoesNotHaveTheOperator_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).HaveOperator(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all have the operator Addition,
					             but only * of * did

					             Not matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class WithOperandTests
		{
			[Test]
			public async Task WhenAllTypesHaveTheOperatorWithOperand_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).HaveOperator<int>(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllTypesHaveTheOperatorWithTypeOperand_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).HaveOperator(Operator.Addition, typeof(Money));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypeDoesNotHaveTheOperatorWithOperand_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).HaveOperator<string>(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              all have the operator Addition with operand string,
					              but none of * did

					              Not matching items:
					              [
					                *
					              ]

					              Collection:
					              [
					                *
					              ]
					              """).AsWildcard();
			}
		}

		public sealed class DoNotHaveOperatorTests
		{
			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(ClassWithoutOperators), null,];

				async Task Act()
				{
					await That(subject).DoNotHaveOperator(Operator.Addition);
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveOperator(Operator.Addition));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have the operator Addition,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ClassWithoutOperators,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has the operator Addition,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ClassWithoutOperators,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoTypeHasTheOperator_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveOperator(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypeHasTheOperator_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveOperator(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have the operator Addition,
					             but * of * did

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTypeOnlyInheritsTheOperator_Directly_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(OperatorDerivedClass),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveOperator(Operator.Subtraction).Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the operators declared on the types count");
			}
		}

		public sealed class DoNotHaveOperatorWithOperandTests
		{
			[Test]
			public async Task WhenNoTypeHasTheOperatorWithOperand_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveOperator<string>(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypeHasTheOperatorWithOperand_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveOperator<int>(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              none have the operator Addition with operand int,
					              but * of * did

					              Matching items:
					              [
					                *
					              ]

					              Collection:
					              [
					                *
					              ]
					              """).AsWildcard();
			}

			[Test]
			public async Task WhenSomeTypeHasTheOperatorWithTypeOperand_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveOperator(Operator.Addition, typeof(Money));
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              none have the operator Addition with operand Money,
					              but * of * did

					              Matching items:
					              [
					                *
					              ]

					              Collection:
					              [
					                *
					              ]
					              """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesHaveTheOperator_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveOperator(Operator.Addition));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all have the operator Addition,
					             but all * did

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNoTypeHasTheOperator_DoNotHave_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveOperator(Operator.Addition));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one has the operator Addition,
					             but none of * did

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllTypesHaveTheOperator_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveOperator(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllTypesHaveTheOperatorWithOperand_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveOperator<int>(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoTypeHasTheOperator_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveOperator(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoTypeHasTheOperatorWithOperand_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveOperator<string>(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypeDoesNotHaveTheOperator_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveOperator(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all have the operator Addition,
					             but only * of * did

					             Not matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSomeTypeHasTheOperator_DoNotHave_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveOperator(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have the operator Addition,
					             but * of * did

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}

#pragma warning restore CA2263 // Prefer generic overload when type is known
