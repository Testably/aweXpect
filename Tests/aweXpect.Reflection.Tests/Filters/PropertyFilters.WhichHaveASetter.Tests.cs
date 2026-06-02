using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichHaveASetter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPropertiesWithASetter()
			{
				Filtered.Properties properties = In.Type<TestClassWithPropertyAccessors>()
					.Properties().WhichHaveASetter();

				await That(properties).HaveASetter().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithPropertyAccessors>()
					.Properties().WhichHaveASetter();

				await That(properties.GetDescription())
					.IsEqualTo("properties with a setter in assembly").AsPrefix();
			}
		}
	}
}
