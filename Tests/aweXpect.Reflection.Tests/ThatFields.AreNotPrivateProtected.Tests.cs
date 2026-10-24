using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotPrivateProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedField")]
			[Arguments("PublicField")]
			[Arguments("InternalField")]
			public async Task WhenFieldInfoIsNotPrivateProtected_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
				{
					await That(subject).AreNotPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsPrivateProtected_ShouldFail()
			{
				Filtered.Fields subject = GetFields("PrivateProtectedField");

				async Task Act()
				{
					await That(subject).AreNotPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             none are private protected,
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
			[Arguments("PublicField")]
			[Arguments("InternalField")]
			public async Task WhenFieldInfoIsNotPrivateProtected_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             at least one is private protected,
					             but none of * were

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFieldInfoIsPrivateProtected_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("PrivateProtectedField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
