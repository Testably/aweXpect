using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichArePrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPrivateProtectedConstructorsWithExplicitMethod()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichArePrivateProtected();

				await That(constructors).ArePrivateProtected().And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("private protected constructors in assembly").AsPrefix();
			}

			internal class WithPrivateProtectedConstructor
			{
				private protected WithPrivateProtectedConstructor()
				{
				}
			}
		}
	}
}
