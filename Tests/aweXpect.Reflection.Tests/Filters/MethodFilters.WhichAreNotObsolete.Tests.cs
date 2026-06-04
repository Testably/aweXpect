using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonObsoleteMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<ClassWithObsoleteMembers>()
					.Types().Methods().WhichAreNotObsolete();

				await That(methods).All().Satisfy(x => !Attribute.IsDefined(x, typeof(ObsoleteAttribute)))
					.And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-obsolete methods in types in assembly").AsPrefix();
			}
		}
	}
}
