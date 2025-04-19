using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotPublic
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("InternalMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotPublic_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).IsNotPublic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotPublic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsPublic_ShouldFail()
			{
				MethodInfo? subject = GetMethod("PublicMethod");

				async Task Act()
					=> await That(subject).IsNotPublic();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was
					             """);
			}
		}
	}
}
