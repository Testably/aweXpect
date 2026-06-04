using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForObsoleteConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<ClassWithObsoleteMembers>()
					.Types().Constructors().WhichAreObsolete();

				await That(constructors).All().Satisfy(x => Attribute.IsDefined(x, typeof(ObsoleteAttribute)))
					.And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("obsolete constructors in types in assembly").AsPrefix();
			}
		}
	}
}
