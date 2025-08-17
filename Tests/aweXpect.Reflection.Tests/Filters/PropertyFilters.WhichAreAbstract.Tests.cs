using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForAbstractProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<AbstractPropertyClass>()
					.Types().Properties().WhichAreAbstract();

				await That(properties).All().Satisfy(x => x.IsReallyAbstract()).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("abstract properties in types in assembly").AsPrefix();
			}
		}
	}
}
