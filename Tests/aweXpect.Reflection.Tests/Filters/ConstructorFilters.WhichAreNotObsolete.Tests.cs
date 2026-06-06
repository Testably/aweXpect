using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreNotObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonObsoleteConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<ClassWithObsoleteMembers>()
					.Types().Constructors().WhichAreNotObsolete();

				await That(constructors).All().Satisfy(x => !Attribute.IsDefined(x, typeof(ObsoleteAttribute)))
					.And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("non-obsolete constructors in types in assembly").AsPrefix();
			}
		}
	}
}
