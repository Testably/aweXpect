using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutEventsThatSatisfyThePredicate()
			{
				Filtered.Events events = In.Type<ConcreteEventClass>()
					.Events().Except(e => e.Name == "ConcreteEvent");

				await That(events).All().Satisfy(e => e!.Name != "ConcreteEvent").And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("events except e => e.Name == \"ConcreteEvent\" in").AsPrefix();
			}
		}
	}
}
