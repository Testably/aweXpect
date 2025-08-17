using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotInterfaces
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyNonInterfaceTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotInterfaces>().Types()
					.WhichAreNotInterfaces();

				await That(subject).AreNotInterfaces().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotInterfaces>().Types()
					.WhichAreNotInterfaces();

				await That(subject.GetDescription()).Contains("types which are not interfaces in");
			}
		}
	}
}
