using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNullableProperties()
			{
				Filtered.Properties properties = In.Type<ClassWithMixedNullableMembers>()
					.Properties().WhichAreNullable();

				await That(properties).AreNullable().And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("nullable properties in type").AsPrefix();
			}

			[Fact]
			public async Task ShouldOnlyKeepNullableProperties()
			{
				Filtered.Properties properties = In.Type<ClassWithMixedNullableMembers>()
					.Properties().WhichAreNullable();

				await That(properties).IsEqualTo([
					typeof(ClassWithMixedNullableMembers)
						.GetProperty(nameof(ClassWithMixedNullableMembers.NullableProperty))!,
					typeof(ClassWithMixedNullableMembers)
						.GetProperty(nameof(ClassWithMixedNullableMembers.NullableValueProperty))!,
				]).InAnyOrder();
			}
		}
	}
}
