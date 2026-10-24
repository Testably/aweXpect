using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotPublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedField")]
			[Arguments("InternalField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotPublic_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
				{
					await That(subject).AreNotPublic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsPublic_ShouldFail()
			{
				Filtered.Fields subject = GetFields("PublicField");

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
			[Arguments("ProtectedField")]
			[Arguments("InternalField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotPublic_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

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
			public async Task WhenFieldInfoIsPublic_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("PublicField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPublic());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
