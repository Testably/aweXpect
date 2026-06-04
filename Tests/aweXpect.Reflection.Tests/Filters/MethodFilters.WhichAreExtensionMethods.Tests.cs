using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreExtensionMethods
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForExtensionMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<MethodFilters>()
					.Types().Methods().WhichAreExtensionMethods();

				await That(methods).All().Satisfy(x => x.IsReallyExtensionMethod()).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("extension methods in types in assembly").AsPrefix();
			}
		}
	}
}
