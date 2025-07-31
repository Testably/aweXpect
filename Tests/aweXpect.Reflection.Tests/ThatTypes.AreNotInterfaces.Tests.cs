using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotInterfaces
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyEnumTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotInterfaces>().Enums();

				async Task Act()
					=> await That(subject).AreNotInterfaces();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyInterfaces_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotInterfaces>().Types()
					.WhichSatisfy(type => type.IsInterface);

				async Task Act()
					=> await That(subject).AreNotInterfaces();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsInterface in assembly containing type ThatTypes.AreNotInterfaces
					             are all not interfaces,
					             but it contained interfaces [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
