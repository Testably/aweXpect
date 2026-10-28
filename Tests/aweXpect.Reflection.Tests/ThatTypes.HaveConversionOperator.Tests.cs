using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatTypes
{
	public sealed partial class HaveConversionOperator
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesHaveTheExplicitConversion_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).HaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllTypesHaveTheImplicitConversion_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).HaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConversionIsOnlyInherited_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(OperatorBaseClass), typeof(OperatorDerivedClass),
				};

				async Task Act()
				{
					await That(subject).HaveImplicitConversionOperator<OperatorBaseClass, int>();
				}

				await That(Act).DoesNotThrow()
					.Because("conversion operators inherited from base types are considered by default");
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(Money), null,];

				async Task Act()
				{
					await That(subject).HaveImplicitConversionOperator<Money, decimal>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveImplicitConversionOperator<Money, decimal>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have an implicit conversion operator from Money to decimal,
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
					             not all have an implicit conversion operator from Money to decimal,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               Money,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenSomeTypeDoesNotHaveTheExplicitConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).HaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              all have an explicit conversion operator from Money to int,
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
			public async Task WhenSomeTypeDoesNotHaveTheImplicitConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).HaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              all have an implicit conversion operator from Money to decimal,
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
			public async Task WithTypeOverloads_ShouldBehaveLikeGeneric()
			{
				IEnumerable<Type?> money = new[]
				{
					typeof(Money),
				};
				IEnumerable<Type?> without = new[]
				{
					typeof(ClassWithoutOperators),
				};

				await That(money).HaveImplicitConversionOperator(typeof(Money), typeof(decimal));
				await That(money).HaveExplicitConversionOperator(typeof(Money), typeof(int));
				await That(without).DoNotHaveImplicitConversionOperator(typeof(Money), typeof(decimal));
				await That(without).DoNotHaveExplicitConversionOperator(typeof(Money), typeof(int));
			}
		}

		public sealed class DoNotHaveTests
		{
			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(ClassWithoutOperators), null,];

				async Task Act()
				{
					await That(subject).DoNotHaveImplicitConversionOperator<Money, decimal>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveImplicitConversionOperator<Money, decimal>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have an implicit conversion operator from Money to decimal,
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
					             at least one has an implicit conversion operator from Money to decimal,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ClassWithoutOperators,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoTypeHasTheExplicitConversion_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoTypeHasTheImplicitConversion_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypeHasTheExplicitConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              none have an explicit conversion operator from Money to int,
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
			public async Task WhenSomeTypeHasTheImplicitConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              none have an implicit conversion operator from Money to decimal,
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
			public async Task WhenTypeOnlyInheritsTheConversion_Directly_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(OperatorDerivedClass),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveImplicitConversionOperator<OperatorBaseClass, int>().Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the conversion operators declared on the types count");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesHaveTheConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(they => they.HaveImplicitConversionOperator<Money, decimal>());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              not all have an implicit conversion operator from Money to decimal,
					              but all * did

					              Collection:
					              [
					                *
					              ]
					              """).AsWildcard();
			}

			[Test]
			public async Task WhenNoTypeHasTheConversion_DoNotHave_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(they => they.DoNotHaveImplicitConversionOperator<Money, decimal>());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              at least one has an implicit conversion operator from Money to decimal,
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
			public async Task WhenAllTypesHaveTheExplicitConversion_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllTypesHaveTheImplicitConversion_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoTypeHasTheImplicitConversion_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypeDoesNotHaveTheImplicitConversion_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              all have an implicit conversion operator from Money to decimal,
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
			public async Task WhenSomeTypeHasTheExplicitConversion_DoNotHave_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              none have an explicit conversion operator from Money to int,
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
			public async Task WithTypeOverloads_ShouldBehaveLikeGeneric()
			{
				IAsyncEnumerable<Type?> money = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();
				IAsyncEnumerable<Type?> without = new[]
				{
					typeof(ClassWithoutOperators),
				}.ToTestAsyncEnumerable<Type?>();

				await That(money).HaveImplicitConversionOperator(typeof(Money), typeof(decimal));
				await That(money).HaveExplicitConversionOperator(typeof(Money), typeof(int));
				await That(without).DoNotHaveImplicitConversionOperator(typeof(Money), typeof(decimal));
				await That(without).DoNotHaveExplicitConversionOperator(typeof(Money), typeof(int));
			}
		}
#endif
	}
}

#pragma warning restore CA2263 // Prefer generic overload when type is known
