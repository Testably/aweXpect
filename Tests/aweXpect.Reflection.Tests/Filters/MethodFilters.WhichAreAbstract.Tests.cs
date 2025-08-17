using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForAbstractMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AbstractMethodClass>()
					.Types().Methods().WhichAreAbstract();

				await That(methods).All().Satisfy(x => x.IsAbstract).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("abstract methods in types in assembly").AsPrefix();
			}
		}
	}
}
