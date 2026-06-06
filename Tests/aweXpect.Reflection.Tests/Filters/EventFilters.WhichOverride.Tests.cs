using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichOverride
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForOverridingEvents()
			{
				Filtered.Events events = In.AssemblyContaining<SealedEventClass>()
					.Types().Events().WhichOverride();

				await That(events)
					.All().Satisfy(x => x.AddMethod?.GetBaseDefinition().DeclaringType != x.AddMethod?.DeclaringType)
					.And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("events which override a base event in types in assembly").AsPrefix();
			}
		}
	}
}
