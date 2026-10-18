using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class ArePublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("InternalProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).ArePublic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are all public,
					             but it contained not matching items

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfoIsPublic_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("PublicProperty");

				async Task Act()
				{
					await That(subject).ArePublic();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("InternalProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldSucceed(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.ArePublic());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsPublic_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("PublicProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.ArePublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are not all public,
					             but it only contained public items

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
