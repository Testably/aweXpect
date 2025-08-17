using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonSealedProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<ConcretePropertyClass>()
					.Types().Properties().WhichAreNotSealed();

				await That(properties).All().Satisfy(x => !x.IsReallySealed()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("non-sealed properties in types in assembly").AsPrefix();
			}
		}
	}
}
