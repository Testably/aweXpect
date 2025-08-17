using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotPrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPrivateProtectedMethodsWithExplicitMethod()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNotPrivateProtected();

				await That(methods).AreNotPrivateProtected().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-private protected methods in assembly").AsPrefix();
			}
		}
	}
}
