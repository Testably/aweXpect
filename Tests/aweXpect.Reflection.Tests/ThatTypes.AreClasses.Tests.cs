using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreClasses
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonClassTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreClasses>().Types();

				async Task Act()
					=> await That(subject).AreClasses();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreClasses
					             are all classes,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyClasses_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreClasses>().Types()
					.WhichSatisfy(type => type.IsClass && !type.IsRecordClass());

				async Task Act()
					=> await That(subject).AreClasses();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
