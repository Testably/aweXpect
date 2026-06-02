using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichHaveAGetter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPropertiesWithAGetter()
			{
				Filtered.Properties properties = In.Type<TestClassWithPropertyAccessors>()
					.Properties().WhichHaveAGetter();

				await That(properties).HaveAGetter().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithPropertyAccessors>()
					.Properties().WhichHaveAGetter();

				await That(properties.GetDescription())
					.IsEqualTo("properties with a getter in assembly").AsPrefix();
			}
		}
	}
}
