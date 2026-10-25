using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class AreOperators
	{
		public sealed class WithOperatorTests
		{
			[Test]
			public async Task WhenAllMethodsAreTheOperator_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Addition")!,
				];

				async Task Act()
				{
					await That(subject).AreOperators(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainADifferentOperator_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Addition")!,
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!,
				];

				async Task Act()
				{
					await That(subject).AreOperators(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are the operator Addition,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               ClassWithOperators ClassWithOperators.op_Subtraction(ClassWithOperators left, ClassWithOperators right)
					             ]

					             Collection:
					             [
					               ClassWithOperators ClassWithOperators.op_Addition(ClassWithOperators left, ClassWithOperators right),
					               ClassWithOperators ClassWithOperators.op_Subtraction(ClassWithOperators left, ClassWithOperators right)
					             ]
					             """);
			}

			[Test]
			public async Task WhenMethodsContainANonOperator_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Addition")!,
					typeof(ClassWithOperators).GetMethod(nameof(ClassWithOperators.RegularMethod))!,
				];

				async Task Act()
				{
					await That(subject).AreOperators(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are the operator Addition,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               void ClassWithOperators.RegularMethod()
					             ]

					             Collection:
					             [
					               ClassWithOperators ClassWithOperators.op_Addition(ClassWithOperators left, ClassWithOperators right),
					               void ClassWithOperators.RegularMethod()
					             ]
					             """);
			}

			[Test]
			public async Task WhenMethodsContainNull_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Addition"),
					null,
				];

				async Task Act()
				{
					await That(subject).AreOperators(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are the operator Addition,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ClassWithOperators ClassWithOperators.op_Addition(ClassWithOperators left, ClassWithOperators right),
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<MethodInfo?>? subject = null;

				async Task Act()
				{
					await That(subject).AreOperators(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are the operator Addition,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUsingAQuantifier_ShouldApplyIt()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Addition")!,
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!,
				];

				async Task Act()
				{
					await That(subject).Exactly(1).AreOperators(Operator.Subtraction);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithOperatorNegatedTests
		{
			[Test]
			public async Task WhenAllMethodsAreTheOperator_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Addition")!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOperators(Operator.Addition));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are the operator Addition,
					             but all 1 were

					             Collection:
					             [
					               ClassWithOperators ClassWithOperators.op_Addition(ClassWithOperators left, ClassWithOperators right)
					             ]
					             """);
			}

			[Test]
			public async Task WhenMethodsContainADifferentOperator_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Addition")!,
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOperators(Operator.Addition));
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class WithOperatorAsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllMethodsAreTheOperator_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(ClassWithOperators).GetMethod("op_Addition")!,
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreOperators(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainADifferentOperator_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(ClassWithOperators).GetMethod("op_Addition")!,
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!,
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreOperators(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are the operator Addition,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               ClassWithOperators ClassWithOperators.op_Subtraction(ClassWithOperators left, ClassWithOperators right)
					             ]

					             Collection:
					             [
					               ClassWithOperators ClassWithOperators.op_Addition(ClassWithOperators left, ClassWithOperators right),
					               ClassWithOperators ClassWithOperators.op_Subtraction(ClassWithOperators left, ClassWithOperators right)
					             ]
					             """);
			}
		}
#endif
	}
}
