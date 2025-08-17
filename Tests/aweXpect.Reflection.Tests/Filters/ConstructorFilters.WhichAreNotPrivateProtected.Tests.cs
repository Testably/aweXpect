using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreNotPrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPrivateProtectedConstructorsWithExplicitMethod()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreNotPrivateProtected();

				await That(constructors).AreNotPrivateProtected().And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("non-private protected constructors in assembly").AsPrefix();
			}
		}
	}
}
