using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNotObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonObsoleteFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<ClassWithObsoleteMembers>()
					.Types().Fields().WhichAreNotObsolete();

				await That(fields).All().Satisfy(x => !Attribute.IsDefined(x, typeof(ObsoleteAttribute)))
					.And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("non-obsolete fields in types in assembly").AsPrefix();
			}
		}
	}
}
