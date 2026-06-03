using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreAttributes
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyAttributeTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreAttributes>().Types()
					.WhichAreAttributes();

				await That(subject).AreAttributes().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreAttributes>().Types()
					.WhichAreAttributes();

				await That(subject.GetDescription()).Contains("types which are attributes in");
			}
		}
	}
}
