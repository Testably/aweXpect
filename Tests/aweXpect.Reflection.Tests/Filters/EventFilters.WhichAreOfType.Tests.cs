using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreOfType
	{
		public delegate void CustomHandler();

		public sealed class GenericTests
		{
			[Test]
			public async Task ShouldFilterForEventsWhichInheritType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().WhichAreOfType<MulticastDelegate>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events of type MulticastDelegate in type")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldFilterForEventsWithExactHandlerType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().WhichAreOfType<EventHandler>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events of type EventHandler in type")
					.AsPrefix();
			}
		}

#pragma warning disable CS0067 // The event is never used
		private class TestClass
		{
			public event EventHandler EventHandlerEvent = null!;
			public event CustomHandler CustomHandlerEvent = null!;
		}
#pragma warning restore CS0067

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload
		public sealed class TypeTests
		{
			[Test]
			public async Task ShouldFilterForEventsWhichInheritType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().WhichAreOfType(typeof(MulticastDelegate));

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events of type MulticastDelegate in type")
					.AsPrefix();
			}
		}

		public sealed class OrOfTypeGenericTests
		{
			[Test]
			public async Task FirstCheckIsOfExactType_ShouldAllowInheritedTypes()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().WhichAreOfExactType<EventHandler>().OrOfType<MulticastDelegate>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events of exact type EventHandler or of type MulticastDelegate in type")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldFilterForEventsWhichMatchAnyOfTheGivenTypes()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().WhichAreOfType<EventHandler>().OrOfType<CustomHandler>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events of type EventHandler or of type EventFilters.WhichAreOfType.CustomHandler in type")
					.AsPrefix();
			}
		}

		public sealed class OrOfTypeTests
		{
			[Test]
			public async Task ShouldFilterForEventsWhichMatchAnyOfTheGivenTypes()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().WhichAreOfType<EventHandler>().OrOfType(typeof(CustomHandler));

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events of type EventHandler or of type EventFilters.WhichAreOfType.CustomHandler in type")
					.AsPrefix();
			}
		}
#pragma warning restore CA2263
	}
}
