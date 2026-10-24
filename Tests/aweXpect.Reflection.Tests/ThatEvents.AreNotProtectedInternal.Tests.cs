using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotProtectedInternal
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedEvent")]
			[Arguments("PublicEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtectedInternal_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
				{
					await That(subject).AreNotProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Events subject = GetEvents("ProtectedInternalEvent");

				async Task Act()
				{
					await That(subject).AreNotProtectedInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             none are protected internal,
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
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtectedInternal_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that *
					             at least one is protected internal,
					             but none of * were

					             Collection:
					             *
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEventInfoIsProtectedInternal_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("ProtectedInternalEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
