using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotPrivateProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedEvent")]
			[Arguments("PublicEvent")]
			[Arguments("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivateProtected_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
				{
					await That(subject).AreNotPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsPrivateProtected_ShouldFail()
			{
				Filtered.Events subject = GetEvents("PrivateProtectedEvent");

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
			[Arguments("ProtectedEvent")]
			[Arguments("PublicEvent")]
			[Arguments("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivateProtected_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

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
			public async Task WhenEventInfoIsPrivateProtected_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("PrivateProtectedEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
