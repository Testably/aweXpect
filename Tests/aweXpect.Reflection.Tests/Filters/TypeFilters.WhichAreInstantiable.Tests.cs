using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreInstantiable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInstantiableTypes()
			{
				Filtered.Types types = In.AssemblyContaining<ConcreteTestClass>()
					.Types().WhichAreInstantiable();

				await That(types).AreInstantiable().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("instantiable types in assembly").AsPrefix();
			}
		}
	}
}
