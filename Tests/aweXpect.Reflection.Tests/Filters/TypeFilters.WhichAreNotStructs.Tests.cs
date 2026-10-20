using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotStructs
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOnlyNonStructTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotStructs>().Types()
					.WhichAreNotStructs();

				await That(subject).AreNotStructs().And.IsNotEmpty();
			}

			[Test]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotStructs>().Types()
					.WhichAreNotStructs();

				await That(subject.GetDescription()).Contains("types which are not structs in");
			}
		}
	}
}
