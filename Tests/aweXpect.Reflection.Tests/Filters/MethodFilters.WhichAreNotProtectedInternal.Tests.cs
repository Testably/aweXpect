using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonProtectedInternalMethodsWithExplicitMethod()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNotProtectedInternal();

				await That(methods).AreNotProtectedInternal().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-protected internal methods in assembly").AsPrefix();
			}
		}
	}
}
