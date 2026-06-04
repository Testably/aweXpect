using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichDoNotHaveADefaultConstructor
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForTypesWithoutADefaultConstructor()
			{
				Filtered.Types types = In.AssemblyContaining<AbstractTestClass>()
					.Types().WhichDoNotHaveADefaultConstructor();

				await That(types).DoNotHaveADefaultConstructor().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("types which do not have a default constructor in assembly").AsPrefix();
			}
		}
	}
}
