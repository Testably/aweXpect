using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreRecordStructs
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonRecordStructTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecordStructs>().Types();

				async Task Act()
					=> await That(subject).AreRecordStructs();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreRecordStructs
					             are all record structs,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyRecordStructs_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecordStructs>().Types()
					.WhichSatisfy(type => type.IsRecordStruct());

				async Task Act()
					=> await That(subject).AreRecordStructs();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
