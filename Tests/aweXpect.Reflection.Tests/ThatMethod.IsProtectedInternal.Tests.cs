using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsProtectedInternal
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedMethod", "protected")]
			[Arguments("PublicMethod", "public")]
			[Arguments("PrivateMethod", "private")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldFail(string methodName,
				string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).IsProtectedInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is protected internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsProtectedInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is protected internal,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("ProtectedInternalMethod");

				async Task Act()
				{
					await That(subject).IsProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtectedInternal());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldFail()
			{
				MethodInfo? subject = GetMethod("ProtectedInternalMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtectedInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was
					             """);
			}
		}
	}
}
