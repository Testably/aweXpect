using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class ContainsEvents
	{
		public sealed class Tests
		{
			[Test]
			public async Task Never_WhenTypeContainsNoMatchingEvent_ShouldSucceed()
			{
				Type subject = typeof(ClassWithoutMarkedEvent);

				async Task Act()
				{
					await That(subject).ContainsEvents(events => events.With<MarkerAttribute>()).Never();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInheritedEventMatches_Directly_ShouldFail()
			{
				Type subject = typeof(DerivedClassWithInheritedMarkedEvent);

				async Task Act()
				{
					await That(subject).ContainsEvents(events => events.With<MarkerAttribute>()).Directly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             declares events with ThatType.ContainsEvents.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsEvents.DerivedClassWithInheritedMarkedEvent
					             """);
			}

			[Test]
			public async Task WhenInheritedEventMatches_ShouldSucceed()
			{
				Type subject = typeof(DerivedClassWithInheritedMarkedEvent);

				async Task Act()
				{
					await That(subject).ContainsEvents(events => events.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeContainsMatchingEvent_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMarkedEvent);

				async Task Act()
				{
					await That(subject).ContainsEvents(events => events.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeContainsNoMatchingEvent_ShouldFail()
			{
				Type subject = typeof(ClassWithoutMarkedEvent);

				async Task Act()
				{
					await That(subject).ContainsEvents(events => events.With<MarkerAttribute>());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains events with ThatType.ContainsEvents.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsEvents.ClassWithoutMarkedEvent
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeContainsMatchingEvent_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedEvent);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.ContainsEvents(events => events.With<MarkerAttribute>()));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain events with ThatType.ContainsEvents.MarkerAttribute at least once,
					             but it contained 1 matching member in ThatType.ContainsEvents.ClassWithMarkedEvent
					             """);
			}
		}

		[AttributeUsage(AttributeTargets.Event)]
		private class MarkerAttribute : Attribute
		{
		}

		private class ClassWithMarkedEvent
		{
			[Marker] public event EventHandler? Something;

			protected virtual void OnSomething() => Something?.Invoke(this, EventArgs.Empty);
		}

		private class ClassWithoutMarkedEvent
		{
			public event EventHandler? Something;

			protected virtual void OnSomething() => Something?.Invoke(this, EventArgs.Empty);
		}

		private class BaseClassWithMarkedEvent
		{
			[Marker] public event EventHandler? Inherited;

			protected virtual void OnInherited() => Inherited?.Invoke(this, EventArgs.Empty);
		}

		private class DerivedClassWithInheritedMarkedEvent : BaseClassWithMarkedEvent
		{
		}
	}
}
