using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithNoParameters
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForConstructorsWithoutParameters()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithNoParameters();

				await That(constructors).Contains(ExpectedParameterlessConstructorInfo());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without parameters in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldNotIncludeConstructorsWithParameters()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithNoParameters();

				await That(constructors).DoesNotContain(ExpectedSingleParameterConstructorInfo());
				await That(constructors).DoesNotContain(ExpectedMultipleParameterConstructorInfo());
			}
		}
	}
}
