using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyInfoIsProtectedInternal_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("ProtectedInternalProperty");

				async Task Act()
					=> await That(subject).AreProtectedInternal();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("PublicProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtectedInternal_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).AreProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are protected internal,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("ProtectedInternalProperty");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtectedInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are protected internal,
					             but all were
					             """).AsWildcard();
			}

			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("PublicProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtectedInternal_ShouldSucceed(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtectedInternal());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
