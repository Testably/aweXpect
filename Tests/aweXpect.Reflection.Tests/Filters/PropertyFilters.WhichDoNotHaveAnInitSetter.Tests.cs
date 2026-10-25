using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichDoNotHaveAnInitSetter
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldAllowFilteringForPropertiesWithoutAnInitSetter()
			{
				Filtered.Properties properties = In.Type<TestClassWithPropertyAccessors>()
					.Properties().WhichDoNotHaveAnInitSetter();

				await That(properties).DoNotHaveAnInitSetter().And.HasCount(3);
			}

			[Test]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithPropertyAccessors>()
					.Properties().WhichDoNotHaveAnInitSetter();

				await That(properties.GetDescription())
					.IsEqualTo("properties without an init setter in assembly").AsPrefix();
			}
		}
	}
}
