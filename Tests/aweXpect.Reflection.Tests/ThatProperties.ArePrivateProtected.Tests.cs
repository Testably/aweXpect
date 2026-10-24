using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class ArePrivateProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("InternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivateProtected_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).ArePrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             all are private protected,
					             but none of * were

					             Not matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivateProtected_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("PrivateProtectedProperty");

				async Task Act()
				{
					await That(subject).ArePrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("InternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivateProtected_ShouldSucceed(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.ArePrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivateProtected_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("PrivateProtectedProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.ArePrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             not all are private protected,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
