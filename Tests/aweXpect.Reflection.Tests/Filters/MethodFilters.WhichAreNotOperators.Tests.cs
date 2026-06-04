using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotOperators
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonOperators()
			{
				Filtered.Methods methods = In.AssemblyContaining<MethodFilters>()
					.Types().Methods().WhichAreNotOperators();

				await That(methods).All().Satisfy(x => !x.IsReallyOperator()).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-operator methods in types in assembly").AsPrefix();
			}
		}
	}
}
