using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreRecordStructs
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOnlyRecordStructTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreRecordStructs>().Types()
					.WhichAreRecordStructs();

				await That(subject).AreRecordStructs().And.IsNotEmpty();
			}

			[Test]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreRecordStructs>().Types()
					.WhichAreRecordStructs();

				await That(subject.GetDescription()).Contains("types which are record structs in");
			}
		}
	}
}
