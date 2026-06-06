using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreReadable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForReadableProperties()
			{
				Filtered.Properties properties = In.Type<TestClassWithReadWriteProperties>()
					.Properties().WhichAreReadable();

				await That(properties).AreReadable().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithReadWriteProperties>()
					.Properties().WhichAreReadable();

				await That(properties.GetDescription())
					.IsEqualTo("readable properties in assembly").AsPrefix();
			}
		}
	}
}
