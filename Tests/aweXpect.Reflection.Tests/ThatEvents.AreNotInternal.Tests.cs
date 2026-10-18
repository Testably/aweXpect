using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventInfoIsInternal_ShouldFail()
			{
				Filtered.Events subject = GetEvents("InternalEvent");

				async Task Act()
				{
					await That(subject).AreNotInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are all not internal,
					             but it contained internal items

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			[Arguments("ProtectedEvent")]
			[Arguments("PublicEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotInternal_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

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
			public async Task WhenEventInfoIsInternal_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("InternalEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotInternal());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("ProtectedEvent")]
			[Arguments("PublicEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotInternal_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             also contain an internal item,
					             but it contained no internal items

					             Collection:
					             *
					             """).AsWildcard();
			}
		}
	}
}
