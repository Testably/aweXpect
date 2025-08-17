using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonAbstractTypes()
			{
				Filtered.Types types = In.AssemblyContaining<ConcreteTestClass>()
					.Types().WhichAreNotAbstract();

				await That(types).AreNotAbstract().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("non-abstract types in assembly").AsPrefix();
			}
		}
	}
}
