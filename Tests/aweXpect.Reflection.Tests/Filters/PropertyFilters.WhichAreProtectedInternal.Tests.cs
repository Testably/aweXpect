using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForProtectedInternalPropertiesWithExplicitMethod()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreProtectedInternal();

				await That(properties).AreProtectedInternal().And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("protected internal properties in assembly").AsPrefix();
			}
		}
	}
}
