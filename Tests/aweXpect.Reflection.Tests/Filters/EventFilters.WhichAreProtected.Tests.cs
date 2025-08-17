using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForProtectedEventsWithExplicitMethod()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAreProtected();

				await That(events).AreProtected().And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("protected events in assembly").AsPrefix();
			}
		}
	}
}
