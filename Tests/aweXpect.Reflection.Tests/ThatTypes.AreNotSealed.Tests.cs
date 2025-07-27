using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonSealedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotSealed>().Abstract.Types();

				async Task Act()
					=> await That(subject).AreNotSealed();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlySealedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotSealed>().Types()
					.WhichSatisfy(type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, });

				async Task Act()
					=> await That(subject).AreNotSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, } in assembly containing type ThatTypes.AreNotSealed
					             are all not sealed,
					             but it contained sealed types [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
