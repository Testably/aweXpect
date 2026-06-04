using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotExtensionProperties
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonExtensionProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<PropertyFilters>()
					.Types().Properties().WhichAreNotExtensionProperties();

				await That(properties).All().Satisfy(x => !x.IsReallyExtensionProperty()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("non-extension properties in types in assembly").AsPrefix();
			}
		}
	}
}
