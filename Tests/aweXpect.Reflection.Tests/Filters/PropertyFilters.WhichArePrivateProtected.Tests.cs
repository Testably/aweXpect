using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichArePrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPrivateProtectedPropertiesWithExplicitMethod()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichArePrivateProtected();

				await That(properties).ArePrivateProtected().And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("private protected properties in assembly").AsPrefix();
			}
		}
	}
}
