using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonSealedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreSealed>().Types();

				async Task Act()
					=> await That(subject).AreSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreSealed
					             are all sealed,
					             but it contained non-sealed types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlySealedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreSealed>().Types()
					.WhichSatisfy(type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, });

				async Task Act()
					=> await That(subject).AreSealed();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
