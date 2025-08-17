using System.Linq;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForAbstractEvents()
			{
				Filtered.Events events = In.AssemblyContaining<AbstractEventClass>()
					.Types().Events().WhichAreAbstract();

				await That(events).All().Satisfy(x => x.AddMethod?.IsAbstract == true).And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("abstract events in types in assembly").AsPrefix();
			}
		}
	}
}
