using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotPrivate
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedMethod")]
			[Arguments("PublicMethod")]
			[Arguments("InternalMethod")]
			[Arguments("ProtectedInternalMethod")]
			[Arguments("PrivateProtectedMethod")]
			public async Task WhenMethodInfoIsNotPrivate_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).IsNotPrivate();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotPrivate();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodInfoIsPrivate_ShouldFail()
			{
				MethodInfo? subject = GetMethod("PrivateMethod");

				async Task Act()
				{
					await That(subject).IsNotPrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not private,
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
			[Arguments("ProtectedInternalMethod", "protected internal")]
			[Arguments("PrivateProtectedMethod", "private protected")]
			public async Task WhenMethodInfoIsNotPrivate_ShouldFail(string methodName, string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is private,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenMethodInfoIsPrivate_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("PrivateMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivate());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
