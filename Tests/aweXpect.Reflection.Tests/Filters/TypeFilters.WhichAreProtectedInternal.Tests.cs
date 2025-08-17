using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForProtectedInternalTypesWithExplicitMethod()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreProtectedInternal();

				await That(types).AreProtectedInternal().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("protected internal types in assembly").AsPrefix();
			}
		}
	}
}
