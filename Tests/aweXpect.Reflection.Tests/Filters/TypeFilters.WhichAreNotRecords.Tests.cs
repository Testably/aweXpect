using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotRecords
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyNonRecordTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotRecords>().Types()
					.WhichAreNotRecords();

				await That(subject).AreNotRecords().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotRecords>().Types()
					.WhichAreNotRecords();

				await That(subject.GetDescription()).Contains("types which are not records in");
			}
		}
	}
}
