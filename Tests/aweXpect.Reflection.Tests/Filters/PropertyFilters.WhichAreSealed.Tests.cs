using System.Linq;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForSealedProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<SealedPropertyClass>()
					.Types().Properties().WhichAreSealed();

				await That(properties).All().Satisfy(x => x.IsReallySealed()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("sealed properties in types in assembly").AsPrefix();
			}
		}
	}
}
