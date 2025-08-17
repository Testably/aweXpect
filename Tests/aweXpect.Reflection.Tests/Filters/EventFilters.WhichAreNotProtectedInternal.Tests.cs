using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonProtectedInternalEventsWithExplicitMethod()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAreNotProtectedInternal();

				await That(events).AreNotProtectedInternal().And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("non-protected internal events in assembly").AsPrefix();
			}
		}
	}
}
