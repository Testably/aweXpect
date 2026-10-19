using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotPrivate
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
					await That(subject).AreNotPrivate();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsPrivate_ShouldFail()
			{
				Filtered.Events subject = GetEvents("PrivateEvent");

				async Task Act()
				{
					await That(subject).AreNotPrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are all not private,
					             but it contained private items

					             Not matching items:
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
			public async Task WhenEventInfoIsNotPrivate_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             also contain a private item,
					             but it contained no private items

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEventInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("PrivateEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
