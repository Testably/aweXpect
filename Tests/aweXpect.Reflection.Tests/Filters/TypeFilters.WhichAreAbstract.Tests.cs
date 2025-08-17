using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForAbstractTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AbstractTestClass>()
					.Types().WhichAreAbstract();

				await That(types).AreAbstract().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("abstract types in assembly").AsPrefix();
			}
		}
	}
}
