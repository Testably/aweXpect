using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotPrivate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPrivateEventsWithExplicitMethod()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAreNotPrivate();

				await That(events).AreNotPrivate();
				await That(events.GetDescription())
					.IsEqualTo("non-private events in assembly").AsPrefix();
			}
		}
	}
}
