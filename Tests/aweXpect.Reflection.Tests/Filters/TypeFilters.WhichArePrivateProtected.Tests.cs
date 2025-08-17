using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichArePrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPrivateProtectedTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichArePrivateProtected();

				await That(types).ArePrivateProtected().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("private protected types in assembly").AsPrefix();
			}
		}
	}
}
