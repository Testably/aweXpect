using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreClasses
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyClassTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreClasses>().Types()
					.WhichAreClasses();

				await That(subject).AreClasses().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreClasses>().Types()
					.WhichAreClasses();

				await That(subject.GetDescription()).Contains("types which are classes in");
			}
		}
	}
}
