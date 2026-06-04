using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed partial class IsNotAnOperator
	{
		public sealed class WithOperatorTests
		{
			[Fact]
			public async Task WhenMethodIsADifferentOperator_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!;

				async Task Act()
				{
					await That(subject).IsNotAnOperator(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsNotAnOperator_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod(
						nameof(ClassWithOperators.RegularMethod))!;

				async Task Act()
				{
					await That(subject).IsNotAnOperator(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnOperator(Operator.Addition);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not the operator Addition,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodIsTheExpectedOperator_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Addition")!;

				async Task Act()
				{
					await That(subject).IsNotAnOperator(Operator.Addition);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not the operator Addition,
					              but it was the operator Addition {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class WithOperatorNegatedTests
		{
			[Fact]
			public async Task WhenMethodIsADifferentOperator_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Subtraction")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnOperator(Operator.Addition));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is the operator Addition,
					              but it was not the operator Addition {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMethodIsTheExpectedOperator_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Addition")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnOperator(Operator.Addition));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
