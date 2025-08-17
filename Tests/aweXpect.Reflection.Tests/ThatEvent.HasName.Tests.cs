using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventInfoHasExpectedPrefix_ShouldSucceed()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
					=> await That(subject).HasName("Public").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoHasMatchingName_ShouldSucceed()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
					=> await That(subject).HasName("PublicEvent");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).HasName("foo");

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has name equal to "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenEventInfoMatchesIgnoringCase_ShouldSucceed()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
					=> await That(subject).HasName("pUBLICevent").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldFail()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
					=> await That(subject).HasName("Event");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "Event",
					             but it was "PublicEvent" which differs at index 0:
					                ↓ (actual)
					               "PublicEvent"
					               "Event"
					                ↑ (expected)
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventInfoDoesNotHaveName_ShouldSucceed()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasName("NonExistentEvent"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoHasName_ShouldFail()
			{
				EventInfo? subject =
					typeof(ClassWithEvents).GetEvent(nameof(ClassWithEvents.PublicEvent));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasName("PublicEvent"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "PublicEvent",
					             but it was "PublicEvent"
					             """);
			}
		}
	}
}
