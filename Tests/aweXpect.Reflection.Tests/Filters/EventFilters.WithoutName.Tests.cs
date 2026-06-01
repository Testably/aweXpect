using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WithoutName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutEventsWithName()
			{
				Filtered.Events events = In.Type<ConcreteEventClass>()
					.Events().WithoutName("ConcreteEvent");

				await That(events).All().Satisfy(e => e!.Name != "ConcreteEvent").And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("events without name equal to \"ConcreteEvent\" in").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Events events = In.Type<ConcreteEventClass>()
					.Events().WithoutName("ConcreteEvent").AsSuffix();

				await That(events).All().Satisfy(e => !e!.Name.EndsWith("ConcreteEvent")).And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("events without name ending with \"ConcreteEvent\" in").AsPrefix();
			}
		}
	}
}
