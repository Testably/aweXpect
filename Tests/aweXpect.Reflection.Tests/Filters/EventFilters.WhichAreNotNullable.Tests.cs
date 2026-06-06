using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonNullableEvents()
			{
				Filtered.Events events = In.Type<ClassWithMixedNullableEvents>()
					.Events().WhichAreNotNullable();

				await That(events).AreNotNullable().And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("non-nullable events in type").AsPrefix();
			}

			[Fact]
			public async Task ShouldOnlyKeepNonNullableEvents()
			{
				Filtered.Events events = In.Type<ClassWithMixedNullableEvents>()
					.Events().WhichAreNotNullable();

				await That(events).IsEqualTo([
					typeof(ClassWithMixedNullableEvents)
						.GetEvent(nameof(ClassWithMixedNullableEvents.NonNullableEvent))!,
				]).InAnyOrder();
			}
		}
	}
}
