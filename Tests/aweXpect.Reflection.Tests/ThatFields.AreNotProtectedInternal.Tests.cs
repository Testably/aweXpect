using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotProtectedInternal
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedField")]
			[Arguments("PublicField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotProtectedInternal_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
				{
					await That(subject).AreNotProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Fields subject = GetFields("ProtectedInternalField");

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
			[Arguments("ProtectedField")]
			[Arguments("PublicField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotProtectedInternal_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

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
			public async Task WhenFieldInfoIsProtectedInternal_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("ProtectedInternalField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
