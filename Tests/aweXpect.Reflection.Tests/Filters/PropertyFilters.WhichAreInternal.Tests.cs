using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalPropertiesWithExplicitMethod()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreInternal();

				await That(properties).AreInternal().And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("internal properties in assembly").AsPrefix();
			}
		}
	}
}
