using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreRecords
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonRecordTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecords>().Types();

				async Task Act()
				{
					await That(subject).AreRecords();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreRecords
					             all are records,
					             but only * of * were

					             Not matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyRecords_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecords>().Types()
					.Which(type => type.IsRecordClass());

				async Task Act()
				{
					await That(subject).AreRecords();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonRecordTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecords>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreRecords());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyRecords_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecords>().Types()
					.Which(type => type.IsRecordClass());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreRecords());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type.IsRecordClass() in assembly containing type ThatTypes.AreRecords
					             not all are records,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
