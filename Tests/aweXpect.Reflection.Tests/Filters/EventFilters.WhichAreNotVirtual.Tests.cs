using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonVirtualEvents()
			{
				Filtered.Events events = In.AssemblyContaining<ConcreteEventClass>()
					.Types().Events().WhichAreNotVirtual();

				await That(events).All().Satisfy(x => x.AddMethod?.IsVirtual != true).And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("non-virtual events in types in assembly").AsPrefix();
			}
		}
	}
}
