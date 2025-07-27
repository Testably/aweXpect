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
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Sealed.Types();

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyAbstractTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Types()
					.WhichSatisfy(type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, });

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, } in assembly containing type ThatTypes.AreNotAbstract
					             are all not abstract,
					             but it contained abstract types [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
