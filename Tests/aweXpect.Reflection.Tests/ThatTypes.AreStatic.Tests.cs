using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonStaticTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStatic>().Types();

				async Task Act()
					=> await That(subject).AreStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreStatic
					             are all static,
					             but it contained non-static types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStatic>().Types()
					.WhichSatisfy(type => type is { IsAbstract: true, IsSealed: true, IsInterface: false, });

				async Task Act()
					=> await That(subject).AreStatic();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
