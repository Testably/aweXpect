using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotPrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPrivateProtectedEventsWithExplicitMethod()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAreNotPrivateProtected();

				await That(events).AreNotPrivateProtected().And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("non-private protected events in assembly").AsPrefix();
			}
		}
	}
}
