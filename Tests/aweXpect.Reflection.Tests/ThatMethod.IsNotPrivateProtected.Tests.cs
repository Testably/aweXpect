using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotPrivateProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("InternalMethod")]
			[Arguments("PrivateMethod")]
			[Arguments("ProtectedInternalMethod")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldFail()
			{
				MethodInfo? subject = GetMethod("PrivateProtectedMethod");

				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedMethod", "protected")]
			[Arguments("PublicMethod", "public")]
			[Arguments("InternalMethod", "internal")]
			[Arguments("PrivateMethod", "private")]
			[Arguments("ProtectedInternalMethod", "protected internal")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldFail(string methodName,
				string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("PrivateProtectedMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
