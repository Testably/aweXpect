using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreStructs
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonStructTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStructs>().Types();

				async Task Act()
					=> await That(subject).AreStructs();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreStructs
					             are all structs,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyStructs_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStructs>().Types()
					.WhichSatisfy(type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum);

				async Task Act()
					=> await That(subject).AreStructs();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
