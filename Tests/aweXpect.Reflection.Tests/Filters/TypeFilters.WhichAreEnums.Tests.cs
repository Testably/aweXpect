using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreEnums
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyEnumTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreEnums>().Types()
					.WhichAreEnums();

				await That(subject).AreEnums().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreEnums>().Types()
					.WhichAreEnums();

				await That(subject.GetDescription()).Contains("types which are enums in");
			}
		}
	}
}
