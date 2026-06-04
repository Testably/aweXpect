using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotAnOperator
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodIsAnOperator_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Addition")!;

				async Task Act()
				{
					await That(subject).IsNotAnOperator();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not an operator,
					              but it was an operator {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMethodIsNotAnOperator_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod(
						nameof(ClassWithOperators.RegularMethod))!;

				async Task Act()
				{
					await That(subject).IsNotAnOperator();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnOperator();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an operator,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodIsAnOperator_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod("op_Addition")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnOperator());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsNotAnOperator_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithOperators).GetMethod(
						nameof(ClassWithOperators.RegularMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnOperator());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an operator,
					              but it was not an operator {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
