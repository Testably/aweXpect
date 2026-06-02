using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichDoNotOverride
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonOverridingMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<ConcreteMethodClass>()
					.Types().Methods().WhichDoNotOverride();

				await That(methods)
					.All().Satisfy(x => x.GetBaseDefinition().DeclaringType == x.DeclaringType)
					.And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("methods which do not override a base method in types in assembly").AsPrefix();
			}
		}
	}
}
