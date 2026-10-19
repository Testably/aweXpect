using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotRecordStructs
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOnlyNonRecordStructTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotRecordStructs>().Types()
					.WhichAreNotRecordStructs();

				await That(subject).AreNotRecordStructs().And.IsNotEmpty();
			}

			[Test]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotRecordStructs>().Types()
					.WhichAreNotRecordStructs();

				await That(subject.GetDescription()).Contains("types which are not record structs in");
			}
		}
	}
}
