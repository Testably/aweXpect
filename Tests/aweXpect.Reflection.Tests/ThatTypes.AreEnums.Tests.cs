using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreEnums
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonEnumTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreEnums>().Types();

				async Task Act()
					=> await That(subject).AreEnums();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreEnums
					             are all enums,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyEnums_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreEnums>().Types()
					.WhichSatisfy(type => type.IsEnum);

				async Task Act()
					=> await That(subject).AreEnums();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
