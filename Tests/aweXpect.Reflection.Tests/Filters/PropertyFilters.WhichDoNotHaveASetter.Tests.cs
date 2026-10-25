using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichDoNotHaveASetter
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldAllowFilteringForPropertiesWithoutASetter()
			{
				Filtered.Properties properties = In.Type<TestClassWithPropertyAccessors>()
					.Properties().WhichDoNotHaveASetter();

				await That(properties).DoNotHaveASetter().And.HasCount(2);
				await That(properties).Contains(typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!)
					.Because("an init-only setter is not a regular setter");
			}

			[Test]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithPropertyAccessors>()
					.Properties().WhichDoNotHaveASetter();

				await That(properties.GetDescription())
					.IsEqualTo("properties without a setter in assembly").AsPrefix();
			}
		}
	}
}
