using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotPrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPrivateProtectedTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNotPrivateProtected();

				await That(types).AreNotPrivateProtected().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("non-private protected types in assembly").AsPrefix();
			}
		}
	}
}
