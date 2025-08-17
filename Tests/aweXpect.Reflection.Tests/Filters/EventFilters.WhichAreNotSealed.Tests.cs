using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonSealedEvents()
			{
				Filtered.Events events = In.AssemblyContaining<ConcreteEventClass>()
					.Types().Events().WhichAreNotSealed();

				await That(events).All().Satisfy(x => x.AddMethod?.IsFinal == false).And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("non-sealed events in types in assembly").AsPrefix();
			}
		}
	}
}
