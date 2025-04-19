using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotPublic
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
					=> await That(subject).AreNotPublic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsPublic_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("PublicProperty");

				async Task Act()
					=> await That(subject).AreNotPublic();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not public,
					             but it contained public items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("InternalProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPublic());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is public,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertyInfoIsPublic_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("PublicProperty");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPublic());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
