using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotNested
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonNestedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotNested>().Types()
					.WhichSatisfy(type => !type.IsNested);

				async Task Act()
					=> await That(subject).AreNotNested();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyNestedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotNested>().Types()
					.WhichSatisfy(type => type.IsNested);

				async Task Act()
					=> await That(subject).AreNotNested();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsNested in assembly containing type ThatTypes.AreNotNested
					             are all not nested,
					             but it contained nested types [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
