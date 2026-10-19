using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotPrivateProtected
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
					await That(subject).AreNotPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivateProtected_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("PrivateProtectedProperty");

				async Task Act()
				{
					await That(subject).AreNotPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are all not private protected,
					             but it contained private protected items

					             Not matching items:
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
			[Arguments("PublicProperty")]
			[Arguments("InternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivateProtected_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             also contain a private protected item,
					             but it contained no private protected items

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivateProtected_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("PrivateProtectedProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
