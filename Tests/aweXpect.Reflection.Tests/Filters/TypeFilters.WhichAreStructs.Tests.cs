using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreStructs
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyStructTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreStructs>().Types()
					.WhichAreStructs();

				await That(subject).AreStructs().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreStructs>().Types()
					.WhichAreStructs();

				await That(subject.GetDescription()).Contains("types which are structs in");
			}
		}
	}
}
