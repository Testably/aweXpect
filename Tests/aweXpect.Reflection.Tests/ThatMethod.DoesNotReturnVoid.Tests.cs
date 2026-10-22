using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class DoesNotReturnVoid
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotReturnVoid();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not return void,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodReturnsValue_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotReturnVoid();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodReturnsVoid_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.VoidMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotReturnVoid();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not return void,
					             but it returned void
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodReturnsValue_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotReturnVoid());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             returns void,
					             but it returned int
					             """);
			}

			[Test]
			public async Task WhenMethodReturnsVoid_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.VoidMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotReturnVoid());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
