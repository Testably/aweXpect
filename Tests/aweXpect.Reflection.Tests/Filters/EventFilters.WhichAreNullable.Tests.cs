using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNullableEvents()
			{
				Filtered.Events events = In.Type<ClassWithMixedNullableEvents>()
					.Events().WhichAreNullable();

				await That(events).AreNullable().And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("nullable events in type").AsPrefix();
			}

			[Fact]
			public async Task ShouldOnlyKeepNullableEvents()
			{
				Filtered.Events events = In.Type<ClassWithMixedNullableEvents>()
					.Events().WhichAreNullable();

				await That(events).IsEqualTo([
					typeof(ClassWithMixedNullableEvents)
						.GetEvent(nameof(ClassWithMixedNullableEvents.NullableEvent))!,
				]).InAnyOrder();
			}
		}
	}
}
