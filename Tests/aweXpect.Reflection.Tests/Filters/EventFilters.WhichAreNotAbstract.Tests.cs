using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonAbstractEvents()
			{
				Filtered.Events events = In.AssemblyContaining<ConcreteEventClass>()
					.Types().Events().WhichAreNotAbstract();

				await That(events).All().Satisfy(x => x.AddMethod?.IsAbstract == false).And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("non-abstract events in types in assembly").AsPrefix();
			}
		}
	}
}
