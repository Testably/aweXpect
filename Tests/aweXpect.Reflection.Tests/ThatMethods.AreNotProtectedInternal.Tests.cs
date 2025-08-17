using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotProtectedInternal
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).AreNotProtectedInternal();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("ProtectedInternalMethod");

				async Task Act()
					=> await That(subject).AreNotProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not protected internal,
					             but it contained protected internal items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtectedInternal_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is protected internal,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodInfoIsProtectedInternal_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("ProtectedInternalMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
