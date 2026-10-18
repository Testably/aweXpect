using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreStructs
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonStructTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStructs>().Types();

				async Task Act()
				{
					await That(subject).AreStructs();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreStructs
					             are all structs,
					             but it contained other types

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyStructs_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStructs>().Types()
					.Which(type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum);

				async Task Act()
				{
					await That(subject).AreStructs();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonStructTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStructs>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStructs());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyStructs_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStructs>().Types()
					.Which(type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStructs());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum in assembly containing type ThatTypes.AreStructs
					             are not all structs,
					             but it only contained structs

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
