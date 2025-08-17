using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotRecords
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecords>().Interfaces();

				async Task Act()
					=> await That(subject).AreNotRecords();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyRecords_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecords>().Types()
					.WhichSatisfy(type => type.IsRecordClass());

				async Task Act()
					=> await That(subject).AreNotRecords();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsRecordClass() in assembly containing type ThatTypes.AreNotRecords
					             are all not records,
					             but it contained records [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
