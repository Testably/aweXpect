using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotClasses
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyNonClassTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotClasses>().Types()
					.WhichAreNotClasses();

				await That(subject).AreNotClasses().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotClasses>().Types()
					.WhichAreNotClasses();

				await That(subject.GetDescription()).Contains("types which are not classes in");
			}
		}
	}
}
