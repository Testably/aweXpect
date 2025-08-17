using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreInternal();

				await That(types).AreInternal().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("internal types in assembly").AsPrefix();
			}
		}
	}
}
