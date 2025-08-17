using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotNested
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonNestedTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNotNested();

				await That(types).AreNotNested().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("non-nested types in assembly").AsPrefix();
			}
		}
	}
}
