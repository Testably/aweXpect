using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonAbstractTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAbstract>().SealedTypes();

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyAbstractTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAbstract>().AbstractTypes();

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that abstract types in assembly containing type AreAbstract
					             are all not abstract,
					             but it contained abstract types [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
