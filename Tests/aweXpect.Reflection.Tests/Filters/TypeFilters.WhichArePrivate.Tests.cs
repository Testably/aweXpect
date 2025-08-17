using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichArePrivate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPrivateTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichArePrivate();

				await That(types).ArePrivate().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("private types in assembly").AsPrefix();
			}
		}
	}
}
