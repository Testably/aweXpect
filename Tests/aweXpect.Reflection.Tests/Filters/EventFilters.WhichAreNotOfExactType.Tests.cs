using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotOfExactType
	{
		public delegate void CustomHandler();

		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForEventsNotOfExactType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().WhichAreNotOfExactType<EventHandler>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events not of exact type EventHandler in")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldKeepDerivedHandlerTypes()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().WhichAreNotOfExactType<Delegate>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
			}

			[Test]
			public async Task WithType_ShouldFilterForEventsNotOfExactType()
			{
				Type unexpectedType = typeof(EventHandler);
				Filtered.Events events = In.Type<TestClass>()
					.Events().WhichAreNotOfExactType(unexpectedType);

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events not of exact type EventHandler in")
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
