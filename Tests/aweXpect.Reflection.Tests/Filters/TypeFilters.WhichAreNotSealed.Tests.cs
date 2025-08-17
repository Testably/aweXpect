using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonSealedTypes()
			{
				Filtered.Types types = In.AssemblyContaining<ConcreteTestClass>()
					.Types().WhichAreNotSealed();

				await That(types).AreNotSealed().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("non-sealed types in assembly").AsPrefix();
			}
		}
	}
}
