using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class ArePublic
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("InternalProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).ArePublic();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are public,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertyInfoIsPublic_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("PublicProperty");

				async Task Act()
					=> await That(subject).ArePublic();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("InternalProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldSucceed(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePublic());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsPublic_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("PublicProperty");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePublic());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are public,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
