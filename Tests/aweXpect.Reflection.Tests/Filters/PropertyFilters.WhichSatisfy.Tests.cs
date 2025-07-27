using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichSatisfy
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForPropertiesWhichSatisfyThePredicate()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichSatisfy(it => it.Name.Equals("SomePropertyToVerifyTheNameOfIt"));

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties matching it => it.Name.Equals(\"SomePropertyToVerifyTheNameOfIt\") in assembly")
					.AsPrefix();
			}
		}
	}
}
