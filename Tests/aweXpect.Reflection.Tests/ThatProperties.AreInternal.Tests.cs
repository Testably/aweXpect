using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyInfoIsInternal_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("InternalProperty");

				async Task Act()
				{
					await That(subject).AreInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotInternal_ShouldFail(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).AreInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are all internal,
					             but it contained not matching items

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
			public async Task WhenPropertyInfoIsInternal_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("InternalProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are not all internal,
					             but it only contained internal items

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotInternal_ShouldSucceed(string propertyName)
			{
				Filtered.Properties subject = GetProperties(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
