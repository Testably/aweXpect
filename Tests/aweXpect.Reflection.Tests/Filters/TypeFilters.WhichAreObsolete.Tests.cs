using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForObsoleteTypes()
			{
				Filtered.Types types = In.AssemblyContaining<ClassWithObsoleteMembers>()
					.Types().WhichAreObsolete();

				await That(types).All().Satisfy(x => Attribute.IsDefined(x, typeof(ObsoleteAttribute)))
					.And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("obsolete types in assembly").AsPrefix();
			}
		}
	}
}
