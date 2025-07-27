using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonGenericTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreGeneric>().Types();

				async Task Act()
					=> await That(subject).AreGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreGeneric
					             are all generic,
					             but it contained non-generic types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyGenericTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreGeneric>().Types()
					.WhichSatisfy(type => type.IsGenericType);

				async Task Act()
					=> await That(subject).AreGeneric();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
