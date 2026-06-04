using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotInstantiable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonInstantiableTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AbstractTestClass>()
					.Types().WhichAreNotInstantiable();

				await That(types).AreNotInstantiable().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("non-instantiable types in assembly").AsPrefix();
			}
		}
	}
}
