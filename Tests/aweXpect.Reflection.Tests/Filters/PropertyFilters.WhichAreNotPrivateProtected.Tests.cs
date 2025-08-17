using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotPrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPrivateProtectedPropertiesWithExplicitMethod()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreNotPrivateProtected();

				await That(properties).AreNotPrivateProtected().And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("non-private protected properties in assembly").AsPrefix();
			}
		}
	}
}
