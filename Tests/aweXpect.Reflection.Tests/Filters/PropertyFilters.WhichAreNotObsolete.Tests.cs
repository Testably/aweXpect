using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonObsoleteProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<ClassWithObsoleteMembers>()
					.Types().Properties().WhichAreNotObsolete();

				await That(properties).All().Satisfy(x => !Attribute.IsDefined(x, typeof(ObsoleteAttribute)))
					.And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("non-obsolete properties in types in assembly").AsPrefix();
			}
		}
	}
}
