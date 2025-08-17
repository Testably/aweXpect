using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonAbstractProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<ConcretePropertyClass>()
					.Types().Properties().WhichAreNotAbstract();

				await That(properties).All().Satisfy(x => !x.IsReallyAbstract()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("non-abstract properties in types in assembly").AsPrefix();
			}
		}
	}
}
