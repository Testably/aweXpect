using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonStaticTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNotStatic();

				await That(types).All().Satisfy(t => !t.IsStatic()).And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("non-static types in assembly").AsPrefix();
			}
		}
	}
}
