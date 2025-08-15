using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreNotPrivate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPrivateConstructorsWithExplicitMethod()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreNotPrivate();

				await That(constructors).AreNotPrivate();
				await That(constructors.GetDescription())
					.IsEqualTo("non-private constructors in assembly").AsPrefix();
			}
		}
	}
}
