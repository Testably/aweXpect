using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNested
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNestedTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNested();

				await That(types).AreNested().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("nested types in assembly").AsPrefix();
			}
		}
	}
}
