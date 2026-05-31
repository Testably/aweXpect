using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreReadWrite
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForReadWriteProperties()
			{
				Filtered.Properties properties = In.Type<TestClassWithReadWriteProperties>()
					.Properties().WhichAreReadWrite();

				await That(properties).AreReadWrite().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithReadWriteProperties>()
					.Properties().WhichAreReadWrite();

				await That(properties.GetDescription())
					.IsEqualTo("read-write properties in assembly").AsPrefix();
			}
		}
	}
}
