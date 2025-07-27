using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNested
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonNestedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNested>().Types();

				async Task Act()
					=> await That(subject).AreNested();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreNested
					             are all nested,
					             but it contained non-nested types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyNestedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNested>().Types()
					.WhichSatisfy(type => type.IsNested);

				async Task Act()
					=> await That(subject).AreNested();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
