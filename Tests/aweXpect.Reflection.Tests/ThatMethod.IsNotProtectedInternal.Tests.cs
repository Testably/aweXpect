using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotProtectedInternal
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			[InlineData("InternalMethod")]
			[InlineData("PrivateProtectedMethod")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldSucceed(string methodName)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).IsNotProtectedInternal();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotProtectedInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldFail()
			{
				MethodInfo? subject = GetMethod("ProtectedInternalMethod");

				async Task Act()
					=> await That(subject).IsNotProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedMethod", "protected")]
			[InlineData("PublicMethod", "public")]
			[InlineData("PrivateMethod", "private")]
			[InlineData("InternalMethod", "internal")]
			[InlineData("PrivateProtectedMethod", "private protected")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldFail(string methodName,
				string expectedAccessModifier)
			{
				MethodInfo? subject = GetMethod(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is protected internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldSucceed()
			{
				MethodInfo? subject = GetMethod("ProtectedInternalMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
