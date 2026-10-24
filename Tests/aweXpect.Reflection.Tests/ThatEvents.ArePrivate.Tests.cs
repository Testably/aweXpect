using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class ArePrivate
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedEvent")]
			[Arguments("PublicEvent")]
			[Arguments("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivate_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
				{
					await That(subject).ArePrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             all are private,
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

			[Test]
			public async Task WhenEventInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("PrivateEvent");

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
			[Arguments("ProtectedEvent")]
			[Arguments("PublicEvent")]
			[Arguments("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivate_ShouldSucceed(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.ArePrivate());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsPrivate_ShouldFail()
			{
				Filtered.Events subject = GetEvents("PrivateEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.ArePrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             not all are private,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
