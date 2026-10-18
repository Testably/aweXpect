using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreRecords
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOnlyRecordTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreRecords>().Types()
					.WhichAreRecords();

				await That(subject).AreRecords().And.IsNotEmpty();
			}

			[Test]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreRecords>().Types()
					.WhichAreRecords();

				await That(subject.GetDescription()).Contains("types which are records in");
			}
		}
	}
}
