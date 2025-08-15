using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotPublic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPublicPropertiesWithExplicitMethod()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreNotPublic();

				await That(properties).AreNotPublic();
				await That(properties.GetDescription())
					.IsEqualTo("non-public properties in assembly").AsPrefix();
			}
		}
	}
}
