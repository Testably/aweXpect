using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotPublic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPublicTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNotPublic();

				await That(types).AreNotPublic();
				await That(types.GetDescription())
					.IsEqualTo("non-public types in assembly").AsPrefix();
			}
		}
	}
}
