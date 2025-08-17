using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonStaticProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreNotStatic();

				await That(properties).All().Satisfy(x => !x.IsReallyStatic()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("non-static properties in assembly").AsPrefix();
			}
		}
	}
}
