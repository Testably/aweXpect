using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class ContainsEvents
	{
		public sealed class Tests
		{
			[Fact]
			public async Task Never_WhenTypeContainsNoMatchingEvent_ShouldSucceed()
			{
				Type subject = typeof(ClassWithoutMarkedEvent);

				async Task Act()
				{
					await That(subject).ContainsEvents(events => events.With<MarkerAttribute>()).Never();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeContainsMatchingEvent_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMarkedEvent);

				async Task Act()
				{
					await That(subject).ContainsEvents(events => events.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeContainsNoMatchingEvent_ShouldFail()
			{
				Type subject = typeof(ClassWithoutMarkedEvent);

				async Task Act()
				{
					await That(subject).ContainsEvents(events => events.With<MarkerAttribute>());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains events with ThatType.ContainsEvents.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsEvents.ClassWithoutMarkedEvent
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeContainsMatchingEvent_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedEvent);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.ContainsEvents(events => events.With<MarkerAttribute>()));
				}

				await That(Act).Throws<XunitException>()
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
	}
}
