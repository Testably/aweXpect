using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonGenericTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotGeneric>().Types()
					.WhichSatisfy(type => !type.IsGenericType);

				async Task Act()
					=> await That(subject).AreNotGeneric();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyGenericTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotGeneric>().Types()
					.WhichSatisfy(type => type.IsGenericType);

				async Task Act()
					=> await That(subject).AreNotGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsGenericType in assembly containing type ThatTypes.AreNotGeneric
					             are all not generic,
					             but it contained generic types [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
