using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichArePrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPrivateProtectedEventsWithExplicitMethod()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichArePrivateProtected();

				await That(events).ArePrivateProtected().And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("private protected events in assembly").AsPrefix();
			}
		}
	}
}
