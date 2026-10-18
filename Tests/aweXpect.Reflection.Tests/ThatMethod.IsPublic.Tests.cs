using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsPublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedMethod", "protected")]
			[Arguments("InternalMethod", "internal")]
			[Arguments("PrivateMethod", "private")]
			public async Task WhenMethodInfoIsNotPublic_ShouldFail(string methodName, string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is public,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is public,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodInfoIsPublic_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("PublicMethod");

				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("InternalMethod")]
			[Arguments("PrivateMethod")]
			public async Task WhenMethodInfoIsNotPublic_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPublic());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsPublic_ShouldFail()
			{
				MethodInfo? subject = GetMethod("PublicMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was
					             """);
			}
		}
	}
}
