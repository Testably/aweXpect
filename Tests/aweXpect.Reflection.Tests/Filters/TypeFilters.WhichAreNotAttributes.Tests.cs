using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotAttributes
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyNonAttributeTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotAttributes>().Types()
					.WhichAreNotAttributes();

				await That(subject).AreNotAttributes().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotAttributes>().Types()
					.WhichAreNotAttributes();

				await That(subject.GetDescription()).Contains("types which are not attributes in");
			}
		}
	}
}
