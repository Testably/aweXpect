using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreRecordStructs
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonRecordStructTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecordStructs>().Types();

				async Task Act()
				{
					await That(subject).AreRecordStructs();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreRecordStructs
					             all are record structs,
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
			public async Task WhenFilteringOnlyRecordStructs_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecordStructs>().Types()
					.Which(type => type.IsRecordStruct());

				async Task Act()
				{
					await That(subject).AreRecordStructs();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonRecordStructTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecordStructs>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreRecordStructs());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyRecordStructs_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecordStructs>().Types()
					.Which(type => type.IsRecordStruct());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreRecordStructs());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type.IsRecordStruct() in assembly containing type ThatTypes.AreRecordStructs
					             not all are record structs,
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
