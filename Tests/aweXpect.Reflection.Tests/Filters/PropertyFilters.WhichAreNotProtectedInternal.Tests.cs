using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonProtectedInternalPropertiesWithExplicitMethod()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreNotProtectedInternal();

				await That(properties).AreNotProtectedInternal().And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("non-protected internal properties in assembly").AsPrefix();
			}
		}
	}
}
