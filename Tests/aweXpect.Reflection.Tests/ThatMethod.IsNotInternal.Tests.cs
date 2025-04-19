using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodInfoIsInternal_ShouldFail()
			{
				MethodInfo? subject = GetMethod("InternalMethod");

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was
					             """);
			}

			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotInternal_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was <null>
					             """);
			}
		}
	}
}
