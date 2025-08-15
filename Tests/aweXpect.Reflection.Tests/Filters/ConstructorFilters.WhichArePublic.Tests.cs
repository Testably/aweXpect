using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichArePublic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPublicConstructorsWithExplicitMethod()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichArePublic();

				await That(constructors).ArePublic();
				await That(constructors.GetDescription())
					.IsEqualTo("public constructors in assembly").AsPrefix();
			}
		}
	}
}
