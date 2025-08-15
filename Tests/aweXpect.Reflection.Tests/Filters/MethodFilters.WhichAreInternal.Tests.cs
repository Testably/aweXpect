using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalMethodsWithExplicitMethod()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreInternal();

				await That(methods).AreInternal();
				await That(methods.GetDescription())
					.IsEqualTo("internal methods in assembly").AsPrefix();
			}
		}
	}
}
