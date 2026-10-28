using System.Collections.Generic;
using aweXpect.Reflection.Collections;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class ContainEvents
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesContainMatchingEvent_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedEvent, ClassWithTwoMarkedEvents>();

				async Task Act()
				{
					await That(subject).ContainEvents(events => events.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableTypesOnlyContainInheritedEvent_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClassWithInheritedMarkedEvent),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).ContainEvents(events => events.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			public async Task WhenSomeTypeContainsNoMatchingEvent_ShouldFail()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedEvent, ClassWithoutMarkedEvent>();

				async Task Act()
				{
					await That(subject).ContainEvents(events => events.With<MarkerAttribute>());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainEvents.ClassWithMarkedEvent and ThatTypes.ContainEvents.ClassWithoutMarkedEvent
					             all contain events with ThatTypes.ContainEvents.MarkerAttribute at least once,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.ContainEvents.ClassWithoutMarkedEvent
					             ]

					             Collection:
					             [
					               ThatTypes.ContainEvents.ClassWithMarkedEvent,
					               ThatTypes.ContainEvents.ClassWithoutMarkedEvent
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypeOnlyContainsInheritedEvent_Directly_ShouldFail()
			{
				Filtered.Types subject =
					In.Types<DerivedClassWithInheritedMarkedEvent, BaseClassWithMarkedEvent>();

				async Task Act()
				{
					await That(subject).ContainEvents(events => events.With<MarkerAttribute>()).Directly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainEvents.DerivedClassWithInheritedMarkedEvent and ThatTypes.ContainEvents.BaseClassWithMarkedEvent
					             all declare events with ThatTypes.ContainEvents.MarkerAttribute at least once,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.ContainEvents.DerivedClassWithInheritedMarkedEvent
					             ]

					             Collection:
					             [
					               ThatTypes.ContainEvents.DerivedClassWithInheritedMarkedEvent,
					               ThatTypes.ContainEvents.BaseClassWithMarkedEvent
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesOnlyContainInheritedEvent_ShouldSucceed()
			{
				Filtered.Types subject =
					In.Types<DerivedClassWithInheritedMarkedEvent, BaseClassWithMarkedEvent>();

				async Task Act()
				{
					await That(subject).ContainEvents(events => events.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSomeTypeContainsNoMatchingEvent_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedEvent, ClassWithoutMarkedEvent>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.ContainEvents(events => events.With<MarkerAttribute>()));
				}

				await That(Act).DoesNotThrow();
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

		private class ClassWithTwoMarkedEvents
		{
			[Marker] public event EventHandler? First;

			[Marker] public event EventHandler? Second;

			protected virtual void OnFirst() => First?.Invoke(this, EventArgs.Empty);
			protected virtual void OnSecond() => Second?.Invoke(this, EventArgs.Empty);
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
