using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotPublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedEvent")]
			[Arguments("InternalEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotPublic_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
				{
					await That(subject).AreNotPublic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsPublic_ShouldFail()
			{
				Filtered.Events subject = GetEvents("PublicEvent");

				async Task Act()
				{
					await That(subject).AreNotPublic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are all not public,
					             but it contained public items

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
			[Arguments("InternalEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotPublic_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             also contain a public item,
					             but it contained no public items

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEventInfoIsPublic_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("PublicEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotPublic());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
