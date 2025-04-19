using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("InternalMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtected_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).AreNotProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsProtected_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("ProtectedMethod");

				async Task Act()
					=> await That(subject).AreNotProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not protected,
					             but it contained protected items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("InternalMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtected_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is protected,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodInfoIsProtected_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("ProtectedMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtected());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
