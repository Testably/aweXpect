using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichArePrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPrivateProtectedMethodsWithExplicitMethod()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichArePrivateProtected();

				await That(methods).ArePrivateProtected().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("private protected methods in assembly").AsPrefix();
			}
		}
	}
}
