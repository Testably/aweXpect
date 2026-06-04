using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotRequired
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonRequiredProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<SomeClassToVerifyThePropertyNameOfIt>()
					.Types().Properties().WhichAreNotRequired();

				await That(properties).All().Satisfy(x => !x.IsRequired()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("non-required properties in types in assembly").AsPrefix();
			}
		}
	}
}
