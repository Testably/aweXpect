using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreInterfaces
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyInterfaceTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreInterfaces>().Types()
					.WhichAreInterfaces();

				await That(subject).AreInterfaces().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreInterfaces>().Types()
					.WhichAreInterfaces();

				await That(subject.GetDescription()).Contains("types which are interfaces in");
			}
		}
	}
}
