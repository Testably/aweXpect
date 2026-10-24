using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotPrivate
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedField")]
			[Arguments("PublicField")]
			[Arguments("InternalField")]
			public async Task WhenFieldInfoIsNotPrivate_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
				{
					await That(subject).AreNotPrivate();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsPrivate_ShouldFail()
			{
				Filtered.Fields subject = GetFields("PrivateField");

				async Task Act()
				{
					await That(subject).AreNotPrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             none are private,
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
			public async Task WhenFieldInfoIsNotPrivate_ShouldFail(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             at least one is private,
					             but none of * were

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFieldInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("PrivateField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
