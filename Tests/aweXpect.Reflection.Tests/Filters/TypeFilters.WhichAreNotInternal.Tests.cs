using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonInternalTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNotInternal();

				await That(types).AreNotInternal().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("non-internal types in assembly").AsPrefix();
			}
		}
	}
}
