using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotProtected
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
					await That(subject).AreNotProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsProtected_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("ProtectedProperty");

				async Task Act()
				{
					await That(subject).AreNotProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             none are protected,
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
			[Arguments("InternalProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             at least one is protected,
					             but none of * were

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfoIsProtected_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("ProtectedProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
