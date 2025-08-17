using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotPrivateProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("InternalMethod")]
			[InlineData("PrivateMethod")]
			[InlineData("ProtectedInternalMethod")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).IsNotPrivateProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotPrivateProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldFail()
			{
				MethodInfo? subject = GetMethod("PrivateProtectedMethod");

				async Task Act()
					=> await That(subject).IsNotPrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
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
			[InlineData("PrivateMethod", "private")]
			[InlineData("ProtectedInternalMethod", "protected internal")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldFail(string methodName,
				string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("PrivateProtectedMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
