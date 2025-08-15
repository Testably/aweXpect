using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichArePrivate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPrivatePropertiesWithExplicitMethod()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichArePrivate();

				await That(properties).ArePrivate();
				await That(properties.GetDescription())
					.IsEqualTo("private properties in assembly").AsPrefix();
			}
		}
	}
}
