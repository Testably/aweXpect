using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotPublic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPublicMethodsWithExplicitMethod()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNotPublic();

				await That(methods).AreNotPublic().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-public methods in assembly").AsPrefix();
			}
		}
	}
}
