using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class HaveNamespace
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypesContainTypeWithDifferentNamespace_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
					=> await That(subject).HaveNamespace("aweXpect.Reflection");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types with namespace end with "ToVerifyingTheNamespaceOfIt" in assembly containing type Tests
					             all have namespace equal to "aweXpect.Reflection",
					             but it contained not matching types [
					               *SomeClassToVerifyTheNamespaceOfIt*
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenTypesHaveNamespace_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
					=> await That(subject)
						.HaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
					=> await That(subject)
						.HaveNamespace("AWExPECT.rEFLECTION.tESTS.tESThELPERS.tYPES.tOvERIFYINGtHEnAMESPACEoFiT")
						.IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesMatchPrefix_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
					=> await That(subject).HaveNamespace("aweXpect.Reflection.Tests").AsPrefix();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
