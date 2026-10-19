using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodInfoIsInternal_ShouldFail()
			{
				MethodInfo? subject = GetMethod("InternalMethod");

				async Task Act()
				{
					await That(subject).IsNotInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was
					             """);
			}

			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("PrivateMethod")]
			[Arguments("ProtectedInternalMethod")]
			[Arguments("PrivateProtectedMethod")]
			public async Task WhenMethodInfoIsNotInternal_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).IsNotInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodInfoIsInternal_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("InternalMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotInternal());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("ProtectedMethod", "protected")]
			[Arguments("PublicMethod", "public")]
			[Arguments("PrivateMethod", "private")]
			[Arguments("ProtectedInternalMethod", "protected internal")]
			[Arguments("PrivateProtectedMethod", "private protected")]
			public async Task WhenMethodInfoIsNotInternal_ShouldFail(string methodName, string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is internal,
					              but it was {expectedAccessModifier}
					              """);
			}
		}
	}
}
