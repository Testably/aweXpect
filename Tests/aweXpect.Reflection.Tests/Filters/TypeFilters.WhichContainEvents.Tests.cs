using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichContainEvents
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterByMatchingEvent()
			{
				Filtered.Types types = In.Types<ClassWithMarkedEvent, UntaggedClass>()
					.WhichContainEvents(events => events.With<MarkerAttribute>());

				await That(types).IsEqualTo([typeof(ClassWithMarkedEvent),]).InAnyOrder();
			}

			[Fact]
			public async Task ShouldUseTheInnerFilterDescription()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainEvents(events => events.With<MarkerAttribute>());

				await That(types.GetDescription())
					.IsEqualTo(
						"types which contain events with TypeFilters.WhichContainEvents.MarkerAttribute at least once ")
					.AsPrefix();
			}
		}

		[AttributeUsage(AttributeTargets.Event)]
		private class MarkerAttribute : Attribute
		{
		}

		private class ClassWithMarkedEvent
		{
#pragma warning disable CS0067 // The event is never used
			[Marker] public event EventHandler? Changed;
#pragma warning restore CS0067
		}

		private class UntaggedClass
		{
#pragma warning disable CS0067 // The event is never used
			public event EventHandler? Changed;
#pragma warning restore CS0067
		}
	}
}
