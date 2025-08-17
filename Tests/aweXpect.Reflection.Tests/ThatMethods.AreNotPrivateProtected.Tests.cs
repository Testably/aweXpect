using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotPrivateProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedMethod")]
			[InlineData("PublicMethod")]
			[InlineData("InternalMethod")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).AreNotPrivateProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("PrivateProtectedMethod");

				async Task Act()
					=> await That(subject).AreNotPrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not private protected,
					             but it contained private protected items [
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
			[InlineData("InternalMethod")]
			public async Task WhenMethodInfoIsNotPrivateProtected_ShouldFail(string methodName)
			{
				Filtered.Methods subject = GetMethods(methodName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is private protected,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodInfoIsPrivateProtected_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("PrivateProtectedMethod");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
