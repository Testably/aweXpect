using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreWriteOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForWriteOnlyProperties()
			{
				Filtered.Properties properties = In.Type<TestClassWithReadWriteProperties>()
					.Properties().WhichAreWriteOnly();

				await That(properties).AreWriteOnly().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithReadWriteProperties>()
					.Properties().WhichAreWriteOnly();

				await That(properties.GetDescription())
					.IsEqualTo("write-only properties in assembly").AsPrefix();
			}
		}
	}
}
