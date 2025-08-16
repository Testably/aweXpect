using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithoutParameters
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWithoutParameters()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithoutParameters();

				await That(methods).Contains(ExpectedParameterlessMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo("methods without parameters in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeMethodsWithParameters()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithoutParameters();

				await That(methods).DoesNotContain(ExpectedSingleParameterMethodInfo());
				await That(methods).DoesNotContain(ExpectedMultipleParameterMethodInfo());
			}
		}
	}
}
