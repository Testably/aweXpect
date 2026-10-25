using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class AreNotOperators
	{
		public sealed class WithOperatorTests
		{
			[Test]
			public async Task WhenMethodsContainTheOperator_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Addition")!,
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!,
				];

				async Task Act()
				{
					await That(subject).AreNotOperators(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are the operator Addition,
					             but 1 of 2 were

					             Matching items:
					             [
					               ClassWithOperators ClassWithOperators.op_Addition(ClassWithOperators left, ClassWithOperators right)
					             ]

					             Collection:
					             [
					               ClassWithOperators ClassWithOperators.op_Addition(ClassWithOperators left, ClassWithOperators right),
					               ClassWithOperators ClassWithOperators.op_Subtraction(ClassWithOperators left, ClassWithOperators right)
					             ]
					             """);
			}

			[Test]
			public async Task WhenMethodsContainNull_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Subtraction"),
					null,
				];

				async Task Act()
				{
					await That(subject).AreNotOperators(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are the operator Addition,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ClassWithOperators ClassWithOperators.op_Subtraction(ClassWithOperators left, ClassWithOperators right),
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenNoMethodIsTheOperator_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!,
					typeof(ClassWithOperators).GetMethod(nameof(ClassWithOperators.RegularMethod))!,
				];

				async Task Act()
				{
					await That(subject).AreNotOperators(Operator.Addition);
				}

				await That(Act).DoesNotThrow()
					.Because("other operators and regular methods are not the operator Addition");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<MethodInfo?>? subject = null;

				async Task Act()
				{
					await That(subject).AreNotOperators(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are the operator Addition,
					             but it was <null>
					             """);
			}
		}

		public sealed class WithOperatorNegatedTests
		{
			[Test]
			public async Task WhenMethodsContainTheOperator_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Addition")!,
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOperators(Operator.Addition));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoMethodIsTheOperator_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOperators(Operator.Addition));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is the operator Addition,
					             but none of 1 were

					             Collection:
					             [
					               ClassWithOperators ClassWithOperators.op_Subtraction(ClassWithOperators left, ClassWithOperators right)
					             ]
					             """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class WithOperatorAsyncEnumerableTests
		{
			[Test]
			public async Task WhenMethodsContainTheOperator_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(ClassWithOperators).GetMethod("op_Addition")!,
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!,
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOperators(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are the operator Addition,
					             but 1 of 2 were

					             Matching items:
					             [
					               ClassWithOperators ClassWithOperators.op_Addition(ClassWithOperators left, ClassWithOperators right)
					             ]

					             Collection:
					             [
					               ClassWithOperators ClassWithOperators.op_Addition(ClassWithOperators left, ClassWithOperators right),
					               ClassWithOperators ClassWithOperators.op_Subtraction(ClassWithOperators left, ClassWithOperators right)
					             ]
					             """);
			}

			[Test]
			public async Task WhenNoMethodIsTheOperator_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!,
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOperators(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
