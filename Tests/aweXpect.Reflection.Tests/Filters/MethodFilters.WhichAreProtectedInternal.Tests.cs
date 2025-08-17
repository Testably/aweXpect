using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForProtectedInternalMethodsWithExplicitMethod()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreProtectedInternal();

				await That(methods).AreProtectedInternal().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("protected internal methods in assembly").AsPrefix();
			}
		}
	}
}
