using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutPropertiesThatSatisfyThePredicate()
			{
				Filtered.Properties properties = In.Type<ConcretePropertyClass>()
					.Properties().Except(property => property.Name == "ConcreteProperty");

				await That(properties).All().Satisfy(p => p!.Name != "ConcreteProperty").And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("properties except property => property.Name == \"ConcreteProperty\" in").AsPrefix();
			}
		}
	}
}
