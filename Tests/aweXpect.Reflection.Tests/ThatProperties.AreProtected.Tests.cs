using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("InternalProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).AreProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             all are protected,
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
			public async Task WhenPropertyInfoIsProtected_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("ProtectedProperty");

				async Task Act()
				{
					await That(subject).AreProtected();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("InternalProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldSucceed(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsProtected_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("ProtectedProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             not all are protected,
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
