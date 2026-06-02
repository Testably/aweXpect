using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichHaveAnInitSetter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPropertiesWithAnInitSetter()
			{
				Filtered.Properties properties = In.Type<TestClassWithPropertyAccessors>()
					.Properties().WhichHaveAnInitSetter();

				await That(properties).HaveAnInitSetter().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithPropertyAccessors>()
					.Properties().WhichHaveAnInitSetter();

				await That(properties.GetDescription())
					.IsEqualTo("properties with an init setter in assembly").AsPrefix();
			}
		}
	}
}
