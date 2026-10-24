using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotPrivate
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("InternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivate_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).AreNotPrivate();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivate_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("PrivateProperty");

				async Task Act()
				{
					await That(subject).AreNotPrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             none are private,
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
			[Arguments("PublicProperty")]
			[Arguments("InternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivate_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             at least one is private,
					             but none of * were

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("PrivateProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
