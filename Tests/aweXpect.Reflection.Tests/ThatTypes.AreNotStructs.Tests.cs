using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotStructs
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStructs>().Interfaces();

				async Task Act()
					=> await That(subject).AreNotStructs();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyStructs_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStructs>().Types()
					.WhichSatisfy(type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum);

				async Task Act()
					=> await That(subject).AreNotStructs();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum in assembly containing type ThatTypes.AreNotStructs
					             are all not structs,
					             but it contained structs [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
