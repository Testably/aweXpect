using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreNotProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonProtectedInternalConstructorsWithExplicitMethod()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreNotProtectedInternal();

				await That(constructors).AreNotProtectedInternal().And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("non-protected internal constructors in assembly").AsPrefix();
			}
		}
	}
}
