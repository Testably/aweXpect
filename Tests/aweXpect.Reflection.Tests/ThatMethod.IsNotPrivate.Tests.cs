using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotPrivate
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("InternalMethod")]
			[InlineData("ProtectedInternalMethod")]
			[InlineData("PrivateProtectedMethod")]
			public async Task WhenMethodInfoIsNotPrivate_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).IsNotPrivate();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotPrivate();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivate_ShouldFail()
			{
				MethodInfo? subject = GetMethod("PrivateMethod");

				async Task Act()
					=> await That(subject).IsNotPrivate();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedMethod", "protected")]
			[InlineData("PublicMethod", "public")]
			[InlineData("InternalMethod", "internal")]
			[InlineData("ProtectedInternalMethod", "protected internal")]
			[InlineData("PrivateProtectedMethod", "private protected")]
			public async Task WhenMethodInfoIsNotPrivate_ShouldFail(string methodName, string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotPrivate());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is private,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivate_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("PrivateMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotPrivate());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
