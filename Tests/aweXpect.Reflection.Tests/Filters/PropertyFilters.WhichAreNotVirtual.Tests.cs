using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonVirtualProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<ConcretePropertyClass>()
					.Types().Properties().WhichAreNotVirtual();

				await That(properties).All().Satisfy(x => !x.IsReallyVirtual()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("non-virtual properties in types in assembly").AsPrefix();
			}
		}
	}
}
