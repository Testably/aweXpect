using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForProtectedInternalEventsWithExplicitMethod()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAreProtectedInternal();

				await That(events).AreProtectedInternal().And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("protected internal events in assembly").AsPrefix();
			}
		}
	}
}
