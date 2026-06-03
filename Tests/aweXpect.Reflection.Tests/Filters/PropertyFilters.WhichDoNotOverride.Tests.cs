using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichDoNotOverride
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonOverridingProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<ConcretePropertyClass>()
					.Types().Properties().WhichDoNotOverride();

				await That(properties).All().Satisfy(x => !x.IsReallyOverride()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("properties which do not override a base property in types in assembly").AsPrefix();
			}
		}
	}
}
