using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class ArePrivate
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
					await That(subject).ArePrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are all private,
					             but it contained not matching items

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFieldInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Fields subject = GetFields("PrivateField");

				async Task Act()
				{
					await That(subject).ArePrivate();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedField")]
			[Arguments("PublicField")]
			[Arguments("InternalField")]
			public async Task WhenFieldInfoIsNotPrivate_ShouldSucceed(string fieldName)
			{
				Filtered.Fields subject = GetFields(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.ArePrivate());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsPrivate_ShouldFail()
			{
				Filtered.Fields subject = GetFields("PrivateField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.ArePrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are not all private,
					             but it only contained private items

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
