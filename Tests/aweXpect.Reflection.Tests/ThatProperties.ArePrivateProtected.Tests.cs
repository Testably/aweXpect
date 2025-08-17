using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class ArePrivateProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("PublicProperty")]
			[InlineData("InternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivateProtected_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).ArePrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are private protected,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertyInfoIsPrivateProtected_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("PrivateProtectedProperty");

				async Task Act()
					=> await That(subject).ArePrivateProtected();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("PublicProperty")]
			[InlineData("InternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivateProtected_ShouldSucceed(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePrivateProtected());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsPrivateProtected_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("PrivateProtectedProperty");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePrivateProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are private protected,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
