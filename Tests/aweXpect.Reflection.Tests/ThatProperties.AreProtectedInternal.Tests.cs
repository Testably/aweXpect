using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreProtectedInternal
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
					await That(subject).AreProtectedInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             all are protected internal,
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
			public async Task WhenPropertyInfoIsProtectedInternal_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("ProtectedInternalProperty");

				async Task Act()
				{
					await That(subject).AreProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtectedInternal_ShouldSucceed(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreProtectedInternal());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("ProtectedInternalProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreProtectedInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             not all are protected internal,
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
