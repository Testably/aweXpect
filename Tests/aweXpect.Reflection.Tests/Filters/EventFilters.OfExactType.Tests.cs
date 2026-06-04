using System;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class OfExactType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFilterForEventsWithExactHandlerType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().OfExactType<EventHandler>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events of exact type EventHandler in type")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotFilterForEventsWhichOnlyInheritTheType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().OfExactType<MulticastDelegate>();

				await That(events).IsEmpty();
				await That(events.GetDescription())
					.IsEqualTo("events of exact type MulticastDelegate in type")
					.AsPrefix();
			}
		}

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload
		public sealed class TypeTests
		{
			[Fact]
			public async Task ShouldFilterForEventsWithExactHandlerType()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().OfExactType(typeof(EventHandler));

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events of exact type EventHandler in type")
					.AsPrefix();
			}
		}

		public sealed class OrOfExactTypeGenericTests
		{
			[Fact]
			public async Task ShouldFilterForEventsWhichMatchAnyOfTheGivenExactTypes()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().OfExactType<EventHandler>().OrOfExactType<CustomHandler>();

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events of exact type EventHandler or of exact type EventFilters.OfExactType.CustomHandler in type")
					.AsPrefix();
			}
		}

		public sealed class OrOfExactTypeTests
		{
			[Fact]
			public async Task ShouldFilterForEventsWhichMatchAnyOfTheGivenExactTypes()
			{
				Filtered.Events events = In.Type<TestClass>()
					.Events().OfExactType<EventHandler>().OrOfExactType(typeof(CustomHandler));

				await That(events).IsEqualTo([
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events of exact type EventHandler or of exact type EventFilters.OfExactType.CustomHandler in type")
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
