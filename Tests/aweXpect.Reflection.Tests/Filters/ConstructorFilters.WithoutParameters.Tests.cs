using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithoutParameters
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForConstructorsWithoutParameters()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithoutParameters();

				await That(constructors).Contains(ExpectedParameterlessConstructorInfo());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without parameters in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeConstructorsWithParameters()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithoutParameters();

				await That(constructors).DoesNotContain(ExpectedSingleParameterConstructorInfo());
				await That(constructors).DoesNotContain(ExpectedMultipleParameterConstructorInfo());
			}
		}
	}
}
