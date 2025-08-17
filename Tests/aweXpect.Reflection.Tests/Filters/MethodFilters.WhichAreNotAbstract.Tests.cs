using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonAbstractMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<ConcreteMethodClass>()
					.Types().Methods().WhichAreNotAbstract();

				await That(methods).All().Satisfy(x => !x.IsAbstract).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-abstract methods in types in assembly").AsPrefix();
			}
		}
	}
}
