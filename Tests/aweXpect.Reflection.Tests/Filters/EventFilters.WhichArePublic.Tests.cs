using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichArePublic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPublicEventsWithExplicitMethod()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichArePublic();

				await That(events).ArePublic();
				await That(events.GetDescription())
					.IsEqualTo("public events in assembly").AsPrefix();
			}
		}
	}
}
