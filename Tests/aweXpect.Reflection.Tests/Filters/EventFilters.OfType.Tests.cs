using System;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class OfType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFilterForEventsWhichInheritType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().OfType<MulticastDelegate>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events of type MulticastDelegate in type")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForEventsWithExactHandlerType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().OfType<EventHandler>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events of type EventHandler in type")
					.AsPrefix();
			}
		}

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload
		public sealed class TypeTests
		{
			[Fact]
			public async Task ShouldFilterForEventsWhichInheritType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().OfType(typeof(MulticastDelegate));

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
			[Fact]
			public async Task FirstCheckIsOfExactType_ShouldAllowInheritedTypes()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().OfExactType<EventHandler>().OrOfType<MulticastDelegate>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events of exact type EventHandler or of type MulticastDelegate in type")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForEventsWhichMatchAnyOfTheGivenTypes()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().OfType<EventHandler>().OrOfType<CustomHandler>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events of type EventHandler or of type EventFilters.OfType.CustomHandler in type")
					.AsPrefix();
			}
		}

		public sealed class OrOfTypeTests
		{
			[Fact]
			public async Task ShouldFilterForEventsWhichMatchAnyOfTheGivenTypes()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().OfType<EventHandler>().OrOfType(typeof(CustomHandler));

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events of type EventHandler or of type EventFilters.OfType.CustomHandler in type")
					.AsPrefix();
			}
		}
#pragma warning restore CA2263

		public delegate void CustomHandler();

#pragma warning disable CS0067 // The event is never used
		private class TestClass
		{
			public event EventHandler EventHandlerEvent = null!;
			public event CustomHandler CustomHandlerEvent = null!;
		}
#pragma warning restore CS0067
	}
}
