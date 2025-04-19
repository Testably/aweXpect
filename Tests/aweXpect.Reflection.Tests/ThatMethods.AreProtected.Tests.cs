using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreProtected
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
					=> await That(subject).AreProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are protected,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodInfoIsProtected_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("ProtectedMethod");

				async Task Act()
					=> await That(subject).AreProtected();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("InternalMethod")]
			[InlineData("PublicMethod")]
			[InlineData("PrivateMethod")]
			public async Task WhenMethodInfoIsNotProtected_ShouldSucceed(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtected());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsProtected_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("ProtectedMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are protected,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
