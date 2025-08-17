using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotPrivate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPrivatePropertiesWithExplicitMethod()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreNotPrivate();

				await That(properties).AreNotPrivate().And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("non-private properties in assembly").AsPrefix();
			}
		}
	}
}
