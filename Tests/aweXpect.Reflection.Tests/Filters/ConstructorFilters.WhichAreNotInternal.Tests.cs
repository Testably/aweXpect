using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonInternalConstructorsWithExplicitMethod()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreNotInternal();

				await That(constructors).AreNotInternal();
				await That(constructors.GetDescription())
					.IsEqualTo("non-internal constructors in assembly").AsPrefix();
			}
		}
	}
}
