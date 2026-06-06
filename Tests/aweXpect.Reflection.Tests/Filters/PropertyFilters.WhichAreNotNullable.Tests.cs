using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonNullableProperties()
			{
				Filtered.Properties properties = In.Type<ClassWithMixedNullableMembers>()
					.Properties().WhichAreNotNullable();

				await That(properties).AreNotNullable().And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("non-nullable properties in type").AsPrefix();
			}

			[Fact]
			public async Task ShouldOnlyKeepNonNullableProperties()
			{
				Filtered.Properties properties = In.Type<ClassWithMixedNullableMembers>()
					.Properties().WhichAreNotNullable();

				await That(properties).IsEqualTo([
					typeof(ClassWithMixedNullableMembers)
						.GetProperty(nameof(ClassWithMixedNullableMembers.NonNullableProperty))!,
					typeof(ClassWithMixedNullableMembers)
						.GetProperty(nameof(ClassWithMixedNullableMembers.NonNullableValueProperty))!,
				]).InAnyOrder();
			}
		}
	}
}
