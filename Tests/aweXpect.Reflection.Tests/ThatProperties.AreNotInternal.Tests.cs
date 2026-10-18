using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyInfoIsInternal_ShouldFail()
			{
				Filtered.Properties subject = GetProperties("InternalProperty");

				async Task Act()
				{
					await That(subject).AreNotInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             all are not internal,
					             but it contained internal items

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
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
					await That(subject).AreNotInternal();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyInfoIsInternal_ShouldSucceed()
			{
				Filtered.Properties subject = GetProperties("InternalProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotInternal());
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
					await That(subject).DoesNotComplyWith(they => they.AreNotInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             at least one is internal,
					             but it contained no internal items

					             Collection:
					             *
					             """).AsWildcard();
			}
		}
	}
}
