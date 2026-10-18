using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotEnums
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOnlyNonEnumTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotEnums>().Types()
					.WhichAreNotEnums();

				await That(subject).AreNotEnums().And.IsNotEmpty();
			}

			[Test]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotEnums>().Types()
					.WhichAreNotEnums();

				await That(subject.GetDescription()).Contains("types which are not enums in");
			}
		}
	}
}
