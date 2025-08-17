using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForStaticTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreStatic();

				await That(types).All().Satisfy(t => t.IsStatic()).And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("static types in assembly").AsPrefix();
			}
		}
	}
}
