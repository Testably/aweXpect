using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotProtected
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
					await That(subject).AreNotProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsProtected_ShouldFail()
			{
				Filtered.Fields subject = GetFields("ProtectedField");

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
			[Arguments("InternalField")]
			[Arguments("PublicField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotProtected_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

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
			public async Task WhenFieldInfoIsProtected_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("ProtectedField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
