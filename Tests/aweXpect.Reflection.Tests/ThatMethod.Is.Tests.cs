using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class Is
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("PublicMethod", AccessModifiers.Public)]
			[Arguments("InternalMethod", AccessModifiers.Internal)]
			[Arguments("ProtectedMethod", AccessModifiers.Protected)]
			[Arguments("PrivateMethod", AccessModifiers.Private)]
			[Arguments("ProtectedInternalMethod", AccessModifiers.ProtectedInternal)]
			[Arguments("PrivateProtectedMethod", AccessModifiers.PrivateProtected)]
			public async Task WhenMethodInfoHasTheAccessModifier_ShouldSucceed(string methodName,
				AccessModifiers accessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).Is(accessModifier);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).Is(AccessModifiers.PrivateProtected);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is private protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldFailForPrivateProtected()
			{
				MethodInfo? subject = GetMethod("ProtectedInternalMethod");

				async Task Act()
				{
					await That(subject).Is(AccessModifiers.PrivateProtected);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is private protected,
					             but it was protected internal
					             """);
			}
		}

		public sealed class IsNotTests
		{
			[Test]
			public async Task WhenMethodInfoHasTheAccessModifier_ShouldFail()
			{
				MethodInfo? subject = GetMethod("ProtectedInternalMethod");

				async Task Act()
				{
					await That(subject).IsNot(AccessModifiers.ProtectedInternal);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was protected internal
					             """);
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNot(AccessModifiers.Public);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was <null>
					             """);
			}
		}
	}
}
