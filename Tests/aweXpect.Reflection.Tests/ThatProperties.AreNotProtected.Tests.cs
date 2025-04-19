using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("InternalProperty")]
			[InlineData("PublicProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).AreNotProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsProtected_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("ProtectedProperty");

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
			[InlineData("InternalProperty")]
			[InlineData("PublicProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

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
			public async Task WhenPropertyInfoIsProtected_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("ProtectedProperty");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtected());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
