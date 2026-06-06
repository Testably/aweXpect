using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreIndexers
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForIndexers()
			{
				Filtered.Properties properties = In.Type<TestClassWithIndexers>()
					.Properties().WhichAreIndexers();

				await That(properties).AreIndexers().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateTheDescription()
			{
				Filtered.Properties properties = In.AssemblyContaining<TestClassWithIndexers>()
					.Properties().WhichAreIndexers();

				await That(properties.GetDescription())
					.IsEqualTo("indexer properties in assembly").AsPrefix();
			}
		}
	}
}
