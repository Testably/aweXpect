using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonSealedMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<ConcreteMethodClass>()
					.Types().Methods().WhichAreNotSealed();

				await That(methods).All().Satisfy(x => !x.IsFinal).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-sealed methods in types in assembly").AsPrefix();
			}
		}
	}
}
