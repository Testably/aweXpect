using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotExtensionMethods
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonExtensionMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<MethodFilters>()
					.Types().Methods().WhichAreNotExtensionMethods();

				await That(methods).All().Satisfy(x => !x.IsReallyExtensionMethod()).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-extension methods in types in assembly").AsPrefix();
			}
		}
	}
}
