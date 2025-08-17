using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotPublic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPublicEventsWithExplicitMethod()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAreNotPublic();

				await That(events).AreNotPublic().And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("non-public events in assembly").AsPrefix();
			}
		}
	}
}
