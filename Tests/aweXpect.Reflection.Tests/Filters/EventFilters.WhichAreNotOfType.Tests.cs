using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotOfType
	{
		public delegate void CustomHandler();

		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForEventsNotOfType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().WhichAreNotOfType<EventHandler>();

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
					.Events().WhichAreNotOfType<Delegate>();

				await That(events).IsEmpty();
			}

			[Test]
			public async Task WithType_ShouldFilterForEventsNotOfType()
			{
				Type unexpectedType = typeof(EventHandler);
				Filtered.Events events = In.Type<TestClass>()
					.Events().WhichAreNotOfType(unexpectedType);

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
			public static event EventHandler? EventHandlerEvent
			{
				add { }
				remove { }
			}

			public static event CustomHandler? CustomHandlerEvent
			{
				add { }
				remove { }
			}
		}
	}
}
