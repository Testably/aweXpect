using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreWritable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForWritableProperties()
			{
				Filtered.Properties properties = In.Type<TestClassWithReadWriteProperties>()
					.Properties().WhichAreWritable();

				await That(properties).AreWritable().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithReadWriteProperties>()
					.Properties().WhichAreWritable();

				await That(properties.GetDescription())
					.IsEqualTo("writable properties in assembly").AsPrefix();
			}
		}
	}
}
