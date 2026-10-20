using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed partial class IsAnOperator
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodIsAnOperator_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Addition")!;

				async Task Act()
				{
					await That(subject).IsAnOperator();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsAPropertyAccessor_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("get_Value")!;

				async Task Act()
				{
					await That(subject).IsAnOperator();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is an operator,
					              but it was not an operator int ClassWithOperators.get_Value()
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
					await That(subject).IsAnOperator();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is an operator,
					              but it was not an operator void ClassWithOperators.RegularMethod()
					              """);
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsAnOperator();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is an operator,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodIsAnOperator_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Addition")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnOperator());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is not an operator,
					              but it was an operator ClassWithOperators ClassWithOperators.op_Addition(ClassWithOperators left, ClassWithOperators right)
					              """);
			}

			[Test]
			public async Task WhenMethodIsNotAnOperator_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod(
						nameof(ClassWithOperators.RegularMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnOperator());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
