using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichOverride
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForOverridingProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<SealedPropertyClass>()
					.Types().Properties().WhichOverride();

				await That(properties).All().Satisfy(x => x.IsReallyOverride()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("properties which override a base property in types in assembly").AsPrefix();
			}
		}
	}
}
