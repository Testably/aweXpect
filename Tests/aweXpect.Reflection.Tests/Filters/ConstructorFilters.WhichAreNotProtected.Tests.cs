using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreNotProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonProtectedConstructorsWithExplicitMethod()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreNotProtected();

				await That(constructors).AreNotProtected().And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("non-protected constructors in assembly").AsPrefix();
			}
		}
	}
}
