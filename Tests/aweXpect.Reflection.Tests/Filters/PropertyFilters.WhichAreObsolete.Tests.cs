using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForObsoleteProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<ClassWithObsoleteMembers>()
					.Types().Properties().WhichAreObsolete();

				await That(properties).All().Satisfy(x => Attribute.IsDefined(x, typeof(ObsoleteAttribute)))
					.And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("obsolete properties in types in assembly").AsPrefix();
			}
		}
	}
}
