using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotProtectedInternal
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtectedInternal_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).AreNotProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("ProtectedInternalProperty");

				async Task Act()
				{
					await That(subject).AreNotProtectedInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             none are protected internal,
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
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtectedInternal_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             at least one is protected internal,
					             but none of * were

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfoIsProtectedInternal_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("ProtectedInternalProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
