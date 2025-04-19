using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsPrivate
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedMethod", "protected")]
			[InlineData("PublicMethod", "public")]
			[InlineData("InternalMethod", "internal")]
			public async Task WhenMethodInfoIsNotPrivate_ShouldFail(string methodName, string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).IsPrivate();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is private,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsPrivate();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is private,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivate_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("PrivateMethod");

				async Task Act()
					=> await That(subject).IsPrivate();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
