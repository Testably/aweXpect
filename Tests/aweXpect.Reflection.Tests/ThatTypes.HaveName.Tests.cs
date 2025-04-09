using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class HaveName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypesContainTypeWithDifferentName_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName("Some").AsPrefix();

				async Task Act()
					=> await That(subject).HaveName("SomeOtherClassName");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types with name start with "Some" in assembly containing type Tests
					             all have name equal to "SomeOtherClassName",
					             but it contained not matching types [
					               *SomeClassToTestHaveNameForTypes*
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenTypesHaveName_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(SomeClassToTestHaveNameForTypes));

				async Task Act()
					=> await That(subject).HaveName("SomeClassToTestHaveNameForTypes");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(SomeClassToTestHaveNameForTypes));

				async Task Act()
					=> await That(subject).HaveName("sOMEcLASStOtESThAVEnAMEfORtYPES").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesMatchPrefix_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(SomeClassToTestHaveNameForTypes));

				async Task Act()
					=> await That(subject).HaveName("SomeClass").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			private class SomeClassToTestHaveNameForTypes;
		}
	}
}
