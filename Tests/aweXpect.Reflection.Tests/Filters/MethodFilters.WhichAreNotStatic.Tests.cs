using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonStaticMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNotStatic();

				await That(methods).All().Satisfy(m => !m.IsStatic).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-static methods in assembly").AsPrefix();
			}
		}
	}
}
