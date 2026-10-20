using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("InternalMethod")]
			[Arguments("PublicMethod")]
			[Arguments("PrivateMethod")]
			[Arguments("ProtectedInternalMethod")]
			[Arguments("PrivateProtectedMethod")]
			public async Task WhenMethodInfoIsNotProtected_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodInfoIsProtected_ShouldFail()
			{
				MethodInfo? subject = GetMethod("ProtectedMethod");

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("InternalMethod", "internal")]
			[Arguments("PublicMethod", "public")]
			[Arguments("PrivateMethod", "private")]
			[Arguments("ProtectedInternalMethod", "protected internal")]
			[Arguments("PrivateProtectedMethod", "private protected")]
			public async Task WhenMethodInfoIsNotProtected_ShouldFail(string methodName, string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenMethodInfoIsProtected_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("ProtectedMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
