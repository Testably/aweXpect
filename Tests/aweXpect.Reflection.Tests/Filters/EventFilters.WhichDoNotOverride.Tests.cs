using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichDoNotOverride
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonOverridingEvents()
			{
				Filtered.Events events = In.AssemblyContaining<ConcreteEventClass>()
					.Types().Events().WhichDoNotOverride();

				await That(events)
					.All().Satisfy(x => x.AddMethod?.GetBaseDefinition().DeclaringType == x.AddMethod?.DeclaringType)
					.And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("events which do not override a base event in types in assembly").AsPrefix();
			}
		}
	}
}
