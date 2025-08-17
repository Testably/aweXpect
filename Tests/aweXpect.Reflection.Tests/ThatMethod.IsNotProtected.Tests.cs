using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("InternalMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			[InlineData("ProtectedInternalMethod")]
			[InlineData("PrivateProtectedMethod")]
			public async Task WhenMethodInfoIsNotProtected_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).IsNotProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsProtected_ShouldFail()
			{
				MethodInfo? subject = GetMethod("ProtectedMethod");

				async Task Act()
					=> await That(subject).IsNotProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("InternalMethod", "internal")]
			[InlineData("PublicMethod", "public")]
			[InlineData("PrivateMethod", "private")]
			[InlineData("ProtectedInternalMethod", "protected internal")]
			[InlineData("PrivateProtectedMethod", "private protected")]
			public async Task WhenMethodInfoIsNotProtected_ShouldFail(string methodName, string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenMethodInfoIsProtected_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("ProtectedMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotProtected());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
