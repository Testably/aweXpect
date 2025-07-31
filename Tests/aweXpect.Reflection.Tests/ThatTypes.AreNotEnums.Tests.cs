using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotEnums
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyClassTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotEnums>().Classes();

				async Task Act()
					=> await That(subject).AreNotEnums();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyEnums_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotEnums>().Types()
					.WhichSatisfy(type => type.IsEnum);

				async Task Act()
					=> await That(subject).AreNotEnums();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsEnum in assembly containing type ThatTypes.AreNotEnums
					             are all not enums,
					             but it contained enums [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
