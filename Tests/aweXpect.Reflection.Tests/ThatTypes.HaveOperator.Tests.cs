using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatTypes
{
	public sealed class HaveOperator
	{
		public sealed class Tests
		{
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all have the operator Addition,
					             but it contained types without the operator Addition [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class WithOperandTests
		{
			[Fact]
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

			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              all have the operator Addition with operand {Formatter.Format(typeof(string))},
					              but it contained types without the operator Addition with operand {Formatter.Format(typeof(string))} [
					                *
					              ]
					              """).AsWildcard();
			}
		}

		public sealed class DoNotHaveOperatorTests
		{
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all do not have the operator Addition,
					             but it contained types with the operator Addition [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class DoNotHaveOperatorWithOperandTests
		{
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              all do not have the operator Addition with operand {Formatter.Format(typeof(int))},
					              but it contained types with the operator Addition with operand {Formatter.Format(typeof(int))} [
					                *
					              ]
					              """).AsWildcard();
			}

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              all do not have the operator Addition with operand {Formatter.Format(typeof(Money))},
					              but it contained types with the operator Addition with operand {Formatter.Format(typeof(Money))} [
					                *
					              ]
					              """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             not all have the operator Addition,
					             but it only contained types with the operator Addition [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a type with the operator Addition,
					             but it only contained types without the operator Addition [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
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

			[Fact]
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

			[Fact]
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

			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all have the operator Addition,
					             but it contained types without the operator Addition [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all do not have the operator Addition,
					             but it contained types with the operator Addition [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}

#pragma warning restore CA2263 // Prefer generic overload when type is known
