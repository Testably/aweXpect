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
				async Task Act()
					=> await That(In.AssemblyContaining<AreAbstract>().SealedTypes()).AreNotAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyAbstractTypes_ShouldFail()
			{
				async Task Act()
					=> await That(In.AssemblyContaining<AreAbstract>().AbstractTypes())
						.AreNotAbstract();

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
