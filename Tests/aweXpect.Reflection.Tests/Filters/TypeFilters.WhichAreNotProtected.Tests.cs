using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonProtectedTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNotProtected();

				await That(types).AreNotProtected().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("non-protected types in assembly").AsPrefix();
			}
		}
	}
}
