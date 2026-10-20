using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("InternalMethod", "internal")]
			[Arguments("PublicMethod", "public")]
			[Arguments("PrivateMethod", "private")]
			public async Task WhenMethodInfoIsNotProtected_ShouldFail(string methodName, string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodInfoIsProtected_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("ProtectedMethod");

				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("InternalMethod")]
			[Arguments("PublicMethod")]
			[Arguments("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtected_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsProtected_ShouldFail()
			{
				MethodInfo? subject = GetMethod("ProtectedMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was
					             """);
			}
		}
	}
}
