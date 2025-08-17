using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyNonGenericTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotGeneric>().Types()
					.WhichAreNotGeneric();

				await That(subject).AreNotGeneric().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotGeneric>().Types()
					.WhichAreNotGeneric();

				await That(subject.GetDescription()).Contains("non-generic types");
			}
		}
	}
}
