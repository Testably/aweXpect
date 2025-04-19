using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodInfoIsInternal_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("InternalMethod");

				async Task Act()
					=> await That(subject).IsInternal();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedMethod", "protected")]
			[InlineData("PublicMethod", "public")]
			[InlineData("PrivateMethod", "private")]
			public async Task WhenMethodInfoIsNotInternal_ShouldFail(string methodName, string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).IsInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is internal,
					             but it was <null>
					             """);
			}
		}
	}
}
