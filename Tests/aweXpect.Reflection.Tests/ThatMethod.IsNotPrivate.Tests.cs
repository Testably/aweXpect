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
	}
}
