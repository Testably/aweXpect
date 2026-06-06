using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForVirtualEvents()
			{
				Filtered.Events events = In.AssemblyContaining<SealedEventClass>()
					.Types().Events().WhichAreVirtual();

				await That(events).All().Satisfy(x => x.AddMethod?.IsVirtual == true).And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("virtual events in types in assembly").AsPrefix();
			}
		}
	}
}
