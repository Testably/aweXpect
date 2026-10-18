using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotProtectedInternal
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("PrivateMethod")]
			[Arguments("InternalMethod")]
			[Arguments("PrivateProtectedMethod")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldFail()
			{
				MethodInfo? subject = GetMethod("ProtectedInternalMethod");

				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedMethod", "protected")]
			[Arguments("PublicMethod", "public")]
			[Arguments("PrivateMethod", "private")]
			[Arguments("InternalMethod", "internal")]
			[Arguments("PrivateProtectedMethod", "private protected")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldFail(string methodName,
				string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is protected internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("ProtectedInternalMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
