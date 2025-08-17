using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonProtectedInternalTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNotProtectedInternal();

				await That(types).AreNotProtectedInternal().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("non-protected internal types in assembly").AsPrefix();
			}
		}
	}
}
