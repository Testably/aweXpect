using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyInfoIsInternal_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("InternalProperty");

				async Task Act()
					=> await That(subject).AreInternal();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("PublicProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotInternal_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).AreInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are internal,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyInfoIsInternal_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("InternalProperty");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are internal,
					             but all were
					             """).AsWildcard();
			}

			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("PublicProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotInternal_ShouldSucceed(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreInternal());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
