using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsPrivateProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedMethod", "protected")]
			[Arguments("PublicMethod", "public")]
			[Arguments("InternalMethod", "internal")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldFail(string methodName,
				string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is private protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("PrivateProtectedMethod");

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("InternalMethod")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldFail()
			{
				MethodInfo? subject = GetMethod("PrivateProtectedMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was
					             """);
			}
		}
	}
}
