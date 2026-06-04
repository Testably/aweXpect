using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonObsoleteTypes()
			{
				Filtered.Types types = In.AssemblyContaining<ClassWithObsoleteMembers>()
					.Types().WhichAreNotObsolete();

				await That(types).All().Satisfy(x => !Attribute.IsDefined(x, typeof(ObsoleteAttribute)))
					.And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("non-obsolete types in assembly").AsPrefix();
			}
		}
	}
}
