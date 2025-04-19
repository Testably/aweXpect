using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotPrivate
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("PublicProperty")]
			[InlineData("InternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivate_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).AreNotPrivate();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsPrivate_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("PrivateProperty");

				async Task Act()
					=> await That(subject).AreNotPrivate();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not private,
					             but it contained private items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("PublicProperty")]
			[InlineData("InternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivate_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is private,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertyInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("PrivateProperty");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
