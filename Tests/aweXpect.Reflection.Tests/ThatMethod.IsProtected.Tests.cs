using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("InternalMethod", "internal")]
			[InlineData("PublicMethod", "public")]
			[InlineData("PrivateMethod", "private")]
			public async Task WhenMethodInfoIsNotProtected_ShouldFail(string methodName, string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsProtected_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("ProtectedMethod");

				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
