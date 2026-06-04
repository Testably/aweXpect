using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichHaveADefaultConstructor
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForTypesWithADefaultConstructor()
			{
				Filtered.Types types = In.AssemblyContaining<ConcreteTestClass>()
					.Types().WhichHaveADefaultConstructor();

				await That(types).HaveADefaultConstructor().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("types which have a default constructor in assembly").AsPrefix();
			}
		}
	}
}
