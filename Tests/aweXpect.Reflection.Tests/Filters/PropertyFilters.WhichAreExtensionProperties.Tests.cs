using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreExtensionProperties
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForExtensionProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<PropertyFilters>()
					.Types().Properties().WhichAreExtensionProperties();

				await That(properties).All().Satisfy(x => x.IsReallyExtensionProperty());
				await That(properties.GetDescription())
					.IsEqualTo("extension properties in types in assembly").AsPrefix();
			}
		}

#if NET10_0_OR_GREATER
		public sealed class NewSyntaxTests
		{
			[Fact]
			public async Task ShouldIncludeInstanceAndStaticExtensionProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<PropertyFilters>()
					.Types().Properties().WhichAreExtensionProperties();

				await That(properties).Contains(x =>
						x.Name == "IsBlankText" && x.GetMethod?.IsStatic == false)
					.And.Contains(x => x.Name == "DefaultValue" && x.GetMethod?.IsStatic == true);
				await That(properties).All().Satisfy(x => x.IsReallyExtensionProperty()).And.IsNotEmpty();
			}
		}
#endif
	}
}
