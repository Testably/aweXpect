using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsPrivateProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedMethod", "protected")]
			[InlineData("PublicMethod", "public")]
			[InlineData("InternalMethod", "internal")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldFail(string methodName, string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is private protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("PrivateProtectedMethod");

				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("InternalMethod")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldFail()
			{
				MethodInfo? subject = GetMethod("PrivateProtectedMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was
					             """);
			}
		}
	}
}
