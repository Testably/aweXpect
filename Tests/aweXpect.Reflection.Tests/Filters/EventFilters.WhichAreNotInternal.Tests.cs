using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonInternalEventsWithExplicitMethod()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAreNotInternal();

				await That(events).AreNotInternal();
				await That(events.GetDescription())
					.IsEqualTo("non-internal events in assembly").AsPrefix();
			}
		}
	}
}
