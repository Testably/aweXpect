using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class ArePrivate
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
					=> await That(subject).ArePrivate();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are private,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertyInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("PrivateProperty");

				async Task Act()
					=> await That(subject).ArePrivate();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("PublicProperty")]
			[InlineData("InternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivate_ShouldSucceed(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePrivate());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsPrivate_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("PrivateProperty");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePrivate());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are private,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
