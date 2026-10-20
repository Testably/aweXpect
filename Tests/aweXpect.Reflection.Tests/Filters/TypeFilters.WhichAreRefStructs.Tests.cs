using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreRefStructs
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOnlyRefStructTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreRefStructs>().Types()
					.WhichAreRefStructs();

				await That(subject).AreRefStructs().And.IsNotEmpty();
			}

			[Test]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreRefStructs>().Types()
					.WhichAreRefStructs();

				await That(subject.GetDescription()).Contains("types which are ref structs in");
			}
		}
	}
}
