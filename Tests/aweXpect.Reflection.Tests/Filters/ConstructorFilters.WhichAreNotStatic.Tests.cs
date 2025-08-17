using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreNotStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonStaticConstructors()
			{
				// Most constructors are non-static
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreNotStatic();

				await That(constructors).All().Satisfy(c => !c.IsStatic).And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("non-static constructors in assembly").AsPrefix();
			}
		}
	}
}
