using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichArePublic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPublicMethodsWithExplicitMethod()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichArePublic();

				await That(methods).ArePublic().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("public methods in assembly").AsPrefix();
			}
		}
	}
}
