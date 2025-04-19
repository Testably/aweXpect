using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreProtected
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
			public async Task WhenPropertyInfoIsProtected_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("ProtectedProperty");

				async Task Act()
					=> await That(subject).AreProtected();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("InternalProperty")]
			[InlineData("PublicProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldSucceed(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtected());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsProtected_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("ProtectedProperty");

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
