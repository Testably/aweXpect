using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForSealedTypes()
			{
				Filtered.Types types = In.AssemblyContaining<SealedTestClass>()
					.Types().WhichAreSealed();

				await That(types).AreSealed().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("sealed types in assembly").AsPrefix();
			}
		}
	}
}
