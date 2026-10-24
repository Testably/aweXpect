using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotPublic
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
					await That(subject).AreNotPublic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsPublic_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("PublicProperty");

				async Task Act()
				{
					await That(subject).AreNotPublic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             none are public,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
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
					await That(subject).DoesNotComplyWith(they => they.AreNotPublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             at least one is public,
					             but none of * were

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfoIsPublic_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("PublicProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPublic());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
