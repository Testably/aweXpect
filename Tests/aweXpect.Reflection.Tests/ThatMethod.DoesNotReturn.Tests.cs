using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class DoesNotReturn
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotReturn<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not return int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodReturnsDifferentType_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotReturn<string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodReturnsType_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotReturn<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not return int,
					             but it returned int
					             """);
			}

			[Test]
			public async Task WhenReturnTypeInheritsFromType_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

				async Task Act()
				{
					await That(subject).DoesNotReturn<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not return ThatMethod.BaseClass,
					             but it returned ThatMethod.DerivedClass
					             """);
			}

			[Test]
			public async Task WithType_WhenMethodReturnsDifferentType_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;
				Type unexpectedType = typeof(string);

				async Task Act()
				{
					await That(subject).DoesNotReturn(unexpectedType);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenMethodReturnsType_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).DoesNotReturn(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not return int,
					             but it returned int
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodReturnsDifferentType_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotReturn<string>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             returns string,
					             but it returned int
					             """);
			}

			[Test]
			public async Task WhenMethodReturnsType_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotReturn<int>());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
