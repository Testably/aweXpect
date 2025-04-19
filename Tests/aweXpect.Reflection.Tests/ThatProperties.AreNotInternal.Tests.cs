using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyInfoIsInternal_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("InternalProperty");

				async Task Act()
					=> await That(subject).AreNotInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not internal,
					             but it contained internal items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("PublicProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotInternal_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).AreNotInternal();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyInfoIsInternal_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("InternalProperty");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotInternal());

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
					=> await That(subject).DoesNotComplyWith(they => they.AreNotInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is internal,
					             but none were
					             """).AsWildcard();
			}
		}
	}
}
