using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForProtectedMethodsWithExplicitMethod()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreProtected();

				await That(methods).AreProtected().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("protected methods in assembly").AsPrefix();
			}
		}
	}
}
