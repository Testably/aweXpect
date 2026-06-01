using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WithoutName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutPropertiesWithName()
			{
				Filtered.Properties properties = In.Type<ConcretePropertyClass>()
					.Properties().WithoutName("ConcreteProperty");

				await That(properties).All().Satisfy(p => p!.Name != "ConcreteProperty").And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("properties without name equal to \"ConcreteProperty\" in").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Properties properties = In.Type<ConcretePropertyClass>()
					.Properties().WithoutName("ConcreteProperty").AsSuffix();

				await That(properties).All().Satisfy(p => !p!.Name.EndsWith("ConcreteProperty")).And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("properties without name ending with \"ConcreteProperty\" in").AsPrefix();
			}
		}
	}
}
