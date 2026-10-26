using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class HaveName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventInfosContainEventInfoWithDifferentName_ShouldFail()
			{
				Filtered.Events subject = GetTypes<ThatEvent.ClassWithEvents>().Events();

				async Task Act()
				{
					await That(subject).HaveName("PublicEvent");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that events in types matching t => t == typeof(T) in assembly containing type ThatEvent.ClassWithEvents
					             all have name equal to "PublicEvent",
					             but only 1 of 6 did

					             Not matching items:
					             [
					               event EventHandler ThatEvent.ClassWithEvents.InternalEvent,
					               event EventHandler ThatEvent.ClassWithEvents.ProtectedEvent,
					               event EventHandler ThatEvent.ClassWithEvents.ProtectedInternalEvent,
					               event EventHandler ThatEvent.ClassWithEvents.PrivateEvent,
					               event EventHandler ThatEvent.ClassWithEvents.PrivateProtectedEvent
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvent.ClassWithEvents.PublicEvent,
					               event EventHandler ThatEvent.ClassWithEvents.InternalEvent,
					               event EventHandler ThatEvent.ClassWithEvents.ProtectedEvent,
					               event EventHandler ThatEvent.ClassWithEvents.ProtectedInternalEvent,
					               event EventHandler ThatEvent.ClassWithEvents.PrivateEvent,
					               event EventHandler ThatEvent.ClassWithEvents.PrivateProtectedEvent
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEventInfosHaveName_ShouldSucceed()
			{
				Filtered.Events subject = GetTypes<ThatEvent.ClassWithSingleEvent>().Events();

				async Task Act()
				{
					await That(subject).HaveName("MyEvent");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfosMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Events subject = GetTypes<ThatEvent.ClassWithSingleEvent>().Events();

				async Task Act()
				{
					await That(subject).HaveName("mYevent").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfosMatchSuffix_ShouldSucceed()
			{
				Filtered.Events subject = GetTypes<ThatEvent.ClassWithEvents>().Events();

				async Task Act()
				{
					await That(subject).HaveName("Event").AsSuffix();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventsDoNotHaveName_ShouldSucceed()
			{
				Filtered.Events subject = GetTypes<ThatEvent.ClassWithEvents>().Events();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("NonExistentEvent"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventsHaveName_ShouldFail()
			{
				Filtered.Events subject = GetTypes<ThatEvent.ClassWithSingleEvent>().Events();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("MyEvent"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that events in types matching t => t == typeof(T) in assembly containing type ThatEvent.ClassWithSingleEvent
					             not all have name equal to "MyEvent",
					             but all 1 did

					             Collection:
					             [
					               event EventHandler ThatEvent.ClassWithSingleEvent.MyEvent
					             ]
					             """).AsWildcard();
			}
		}
	}
}
