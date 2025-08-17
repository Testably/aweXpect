using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForSealedEvents()
			{
				Filtered.Events events = In.AssemblyContaining<SealedEventClass>()
					.Types().Events().WhichAreSealed();

				await That(events).All().Satisfy(x => x.AddMethod?.IsFinal == true).And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("sealed events in types in assembly").AsPrefix();
			}
		}
	}
}
