using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichOverride
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForOverridingMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<SealedMethodClass>()
					.Types().Methods().WhichOverride();

				await That(methods)
					.All().Satisfy(x => x.GetBaseDefinition().DeclaringType != x.DeclaringType)
					.And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("methods which override a base method in types in assembly").AsPrefix();
			}
		}
	}
}
