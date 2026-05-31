using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotRefStructs
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyNonRefStructTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotRefStructs>().Types()
					.WhichAreNotRefStructs();

				await That(subject).AreNotRefStructs().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotRefStructs>().Types()
					.WhichAreNotRefStructs();

				await That(subject.GetDescription()).Contains("types which are not ref structs in");
			}
		}
	}
}
