using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("InternalEvent")]
			[Arguments("PublicEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtected_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
				{
					await That(subject).AreNotProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsProtected_ShouldFail()
			{
				Filtered.Events subject = GetEvents("ProtectedEvent");

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
			[Arguments("InternalEvent")]
			[Arguments("PublicEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtected_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

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
			public async Task WhenEventInfoIsProtected_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("ProtectedEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
