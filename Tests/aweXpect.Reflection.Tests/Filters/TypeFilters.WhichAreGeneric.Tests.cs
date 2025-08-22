using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed partial class WhichAreGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyGenericTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreGeneric>().Types()
					.WhichAreGeneric();

				await That(subject).AreGeneric().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreGeneric>().Types()
					.WhichAreGeneric();

				await That(subject.GetDescription()).Contains("generic types");
			}
		}
	}
}
