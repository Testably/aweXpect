using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed partial class IsAnOperator
	{
		public sealed class WithOperatorTests
		{
			[Test]
			public async Task WhenMethodIsADifferentOperator_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!;

				async Task Act()
				{
					await That(subject).IsAnOperator(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is the operator Addition,
					              but it was not the operator Addition ClassWithOperators ClassWithOperators.op_Subtraction(ClassWithOperators left, ClassWithOperators right)
					              """);
			}

			[Test]
			public async Task WhenMethodIsNotAnOperator_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod(
						nameof(ClassWithOperators.RegularMethod))!;

				async Task Act()
				{
					await That(subject).IsAnOperator(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is the operator Addition,
					              but it was not the operator Addition void ClassWithOperators.RegularMethod()
					              """);
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsAnOperator(Operator.Addition);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is the operator Addition,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodIsTheExpectedOperator_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Addition")!;

				async Task Act()
				{
					await That(subject).IsAnOperator(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithOperatorNegatedTests
		{
			[Test]
			public async Task WhenMethodIsADifferentOperator_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnOperator(Operator.Addition));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsTheExpectedOperator_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Addition")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnOperator(Operator.Addition));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is not the operator Addition,
					              but it was the operator Addition ClassWithOperators ClassWithOperators.op_Addition(ClassWithOperators left, ClassWithOperators right)
					              """);
			}
		}
	}
}
