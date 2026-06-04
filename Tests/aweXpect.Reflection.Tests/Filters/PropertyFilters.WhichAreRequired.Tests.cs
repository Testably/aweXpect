using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreRequired
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForRequiredProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<SomeClassToVerifyThePropertyNameOfIt>()
					.Types().Properties().WhichAreRequired();

				await That(properties).All().Satisfy(x => x.IsRequired()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("required properties in types in assembly").AsPrefix();
			}
		}
	}
}
