using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreProtected
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
					await That(subject).AreProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are all protected,
					             but it contained not matching items

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEventInfoIsProtected_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("ProtectedEvent");

				async Task Act()
				{
					await That(subject).AreProtected();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("InternalEvent")]
			[Arguments("PublicEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtected_ShouldSucceed(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsProtected_ShouldFail()
			{
				Filtered.Events subject = GetEvents("ProtectedEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             are not all protected,
					             but it only contained protected items

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
