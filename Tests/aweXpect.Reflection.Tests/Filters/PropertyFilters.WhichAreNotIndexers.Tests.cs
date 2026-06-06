using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotIndexers
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonIndexers()
			{
				Filtered.Properties properties = In.Type<TestClassWithIndexers>()
					.Properties().WhichAreNotIndexers();

				await That(properties).AreNotIndexers().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithIndexers>()
					.Properties().WhichAreNotIndexers();

				await That(properties.GetDescription())
					.IsEqualTo("non-indexer properties in assembly").AsPrefix();
			}
		}
	}
}
