using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreReadOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForReadOnlyProperties()
			{
				Filtered.Properties properties = In.Type<TestClassWithReadWriteProperties>()
					.Properties().WhichAreReadOnly();

				await That(properties).AreReadOnly().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithReadWriteProperties>()
					.Properties().WhichAreReadOnly();

				await That(properties.GetDescription())
					.IsEqualTo("read-only properties in assembly").AsPrefix();
			}
		}
	}
}
