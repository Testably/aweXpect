using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalConstructorsWithExplicitMethod()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreInternal();

				await That(constructors).AreInternal();
				await That(constructors.GetDescription())
					.IsEqualTo("internal constructors in assembly").AsPrefix();
			}
		}
	}
}
