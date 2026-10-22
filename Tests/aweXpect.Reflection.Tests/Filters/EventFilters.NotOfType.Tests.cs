using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class NotOfType
	{
		public delegate void CustomHandler();

		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForEventsNotOfType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().NotOfType<EventHandler>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events not of type EventHandler in")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldIncludeDerivedHandlerTypes()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().NotOfType<Delegate>();

				await That(events).IsEmpty();
			}

			[Test]
			public async Task WithType_ShouldFilterForEventsNotOfType()
			{
				Type unexpectedType = typeof(EventHandler);
				Filtered.Events events = In.Type<TestClass>()
					.Events().NotOfType(unexpectedType);

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events not of type EventHandler in")
					.AsPrefix();
			}
		}

		private class TestClass
		{
			public event EventHandler? EventHandlerEvent
			{
				add { }
				remove { }
			}

			public event CustomHandler? CustomHandlerEvent
			{
				add { }
				remove { }
			}
		}
	}
}
