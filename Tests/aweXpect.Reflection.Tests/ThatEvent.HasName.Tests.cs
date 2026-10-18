using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventInfoHasExpectedPrefix_ShouldSucceed()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
				{
					await That(subject).HasName("Public").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoHasMatchingName_ShouldSucceed()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
				{
					await That(subject).HasName("PublicEvent");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).HasName("foo");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has name equal to "foo",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventInfoMatchesIgnoringCase_ShouldSucceed()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
				{
					await That(subject).HasName("pUBLICevent").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldFail()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
				{
					await That(subject).HasName("Event");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "Event",
					             but it had name "PublicEvent", which differs at index 0:
					                ↓ (actual)
					               "PublicEvent"
					               "Event"
					                ↑ (expected)
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventInfoDoesNotHaveName_ShouldSucceed()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasName("NonExistentEvent"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoHasName_ShouldFail()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasName("PublicEvent"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "PublicEvent",
					             but it had name "PublicEvent"
					             """);
			}
		}
	}
}
