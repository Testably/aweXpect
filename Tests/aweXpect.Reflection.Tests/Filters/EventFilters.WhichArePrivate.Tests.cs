using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichArePrivate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPrivateEventsWithExplicitMethod()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichArePrivate();

				await That(events).ArePrivate().And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("private events in assembly").AsPrefix();
			}
		}
	}
}
