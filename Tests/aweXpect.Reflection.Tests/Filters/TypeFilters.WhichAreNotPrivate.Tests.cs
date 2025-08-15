using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotPrivate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPrivateTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNotPrivate();

				await That(types).AreNotPrivate();
				await That(types.GetDescription())
					.IsEqualTo("non-private types in assembly").AsPrefix();
			}
		}
	}
}
