using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("InternalField")]
			[Arguments("PublicField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotProtected_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

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
			public async Task WhenFieldInfoIsProtected_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("ProtectedField");

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
			[Arguments("InternalField")]
			[Arguments("PublicField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotProtected_ShouldSucceed(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsProtected_ShouldFail()
			{
				Filtered.Fields subject = GetFields("ProtectedField");

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
