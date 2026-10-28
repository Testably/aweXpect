using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithNoParameters
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForMethodsWithoutParameters()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithNoParameters();

				await That(methods).Contains(ExpectedParameterlessMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo("methods without parameters in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldNotIncludeMethodsWithParameters()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithNoParameters();

				await That(methods).DoesNotContain(ExpectedSingleParameterMethodInfo());
				await That(methods).DoesNotContain(ExpectedMultipleParameterMethodInfo());
			}
		}
	}
}
