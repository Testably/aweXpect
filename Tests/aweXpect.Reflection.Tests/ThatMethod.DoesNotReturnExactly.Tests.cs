using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class DoesNotReturnExactly
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotReturnExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not return exactly int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodReturnsExactlyType_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotReturnExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not return exactly int,
					             but it returned int
					             """);
			}

			[Test]
			public async Task WhenReturnTypeInheritsFromType_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

				async Task Act()
				{
					await That(subject).DoesNotReturnExactly<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenMethodReturnsExactlyType_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).DoesNotReturnExactly(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not return exactly int,
					             but it returned int
					             """);
			}

			[Test]
			public async Task WithType_WhenReturnTypeInheritsFromType_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;
				Type unexpectedType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoesNotReturnExactly(unexpectedType);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodReturnsExactlyType_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotReturnExactly<int>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenReturnTypeInheritsFromType_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotReturnExactly<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             returns exactly ThatMethod.BaseClass,
					             but it returned ThatMethod.DerivedClass
					             """);
			}
		}
	}
}
