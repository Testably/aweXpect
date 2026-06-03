using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForVirtualProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<SealedPropertyClass>()
					.Types().Properties().WhichAreVirtual();

				await That(properties).All().Satisfy(x => x.IsReallyVirtual()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("virtual properties in types in assembly").AsPrefix();
			}
		}
	}
}
