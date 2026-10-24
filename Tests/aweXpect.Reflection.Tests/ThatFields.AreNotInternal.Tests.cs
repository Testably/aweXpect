using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldInfoIsInternal_ShouldFail()
			{
				Filtered.Fields subject = GetFields("InternalField");

				async Task Act()
				{
					await That(subject).AreNotInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             none are internal,
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

			[Test]
			[Arguments("ProtectedField")]
			[Arguments("PublicField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotInternal_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

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
			public async Task WhenFieldInfoIsInternal_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("InternalField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotInternal());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("ProtectedField")]
			[Arguments("PublicField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotInternal_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             at least one is internal,
					             but none of * were

					             Collection:
					             *
					             """).AsWildcard();
			}
		}
	}
}
