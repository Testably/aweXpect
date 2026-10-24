using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldInfoIsInternal_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("InternalField");

				async Task Act()
				{
					await That(subject).AreInternal();
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
					await That(subject).AreInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             all are internal,
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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldInfoIsInternal_ShouldFail()
			{
				Filtered.Fields subject = GetFields("InternalField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             not all are internal,
					             but all * were

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
			public async Task WhenFieldInfoIsNotInternal_ShouldSucceed(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
