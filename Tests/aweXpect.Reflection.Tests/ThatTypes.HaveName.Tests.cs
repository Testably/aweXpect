using System.Collections.Generic;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class HaveName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(SomeClassToTestHaveNameForTypes), null,];

				async Task Act()
				{
					await That(subject).HaveName("SomeClassToTestHaveNameForTypes");
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("SomeClassToTestHaveNameForTypes"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have name equal to "SomeClassToTestHaveNameForType…",
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.HaveName.Tests.SomeClassToTestHaveNameForTypes,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have name equal to "SomeClassToTestHaveNameForType…",
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.HaveName.Tests.SomeClassToTestHaveNameForTypes,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenTypesContainTypeWithDifferentName_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName("Some").AsPrefix();

				async Task Act()
				{
					await That(subject).HaveName("SomeOtherClassName");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types with name starting with "Some" in assembly containing type ThatTypes.HaveName.Tests
					             all have name equal to "SomeOtherClassName",
					             but none of 9 did

					             Not matching items:
					             [
					               SomeClassToVerifyTheNamespaceOfIt,
					               ConstructorFilters.SomeClassToVerifyTheConstructorNameOfIt,
					               EventFilters.SomeClassToVerifyTheEventNameOfIt,
					               FieldFilters.SomeClassToVerifyTheFieldNameOfIt,
					               MethodFilters.SomeClassToVerifyTheMethodNameOfIt,
					               PropertyFilters.SomeClassToVerifyThePropertyNameOfIt,
					               ThatTypes.HaveName.Tests.SomeClassToTestHaveNameForTypes,
					               TypeFilters.Which.Tests.SomeClassToVerifyViaAPredicate,
					               TypeFilters.WithName.Tests.SomeClassToVerifyTheNameOfIt
					             ]

					             Collection:
					             [
					               SomeClassToVerifyTheNamespaceOfIt,
					               ConstructorFilters.SomeClassToVerifyTheConstructorNameOfIt,
					               EventFilters.SomeClassToVerifyTheEventNameOfIt,
					               FieldFilters.SomeClassToVerifyTheFieldNameOfIt,
					               MethodFilters.SomeClassToVerifyTheMethodNameOfIt,
					               PropertyFilters.SomeClassToVerifyThePropertyNameOfIt,
					               ThatTypes.HaveName.Tests.SomeClassToTestHaveNameForTypes,
					               TypeFilters.Which.Tests.SomeClassToVerifyViaAPredicate,
					               TypeFilters.WithName.Tests.SomeClassToVerifyTheNameOfIt
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTypesHaveName_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(SomeClassToTestHaveNameForTypes));

				async Task Act()
				{
					await That(subject).HaveName("SomeClassToTestHaveNameForTypes");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(SomeClassToTestHaveNameForTypes));

				async Task Act()
				{
					await That(subject).HaveName("sOMEcLASStOtESThAVEnAMEfORtYPES").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesMatchPrefix_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName(nameof(SomeClassToTestHaveNameForTypes));

				async Task Act()
				{
					await That(subject).HaveName("SomeClass").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			private class SomeClassToTestHaveNameForTypes;
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypesContainTypeWithDifferentName_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName("Some").AsPrefix();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("SomeOtherClassName"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesHaveName_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithName("SomeClassToTestHaveNameForTypes");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("SomeClassToTestHaveNameForTypes"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types with name equal to "SomeClassToTestHaveNameForType…" in assembly containing type ThatTypes.HaveName.Tests
					             not all have name equal to "SomeClassToTestHaveNameForType…",
					             but all 1 did

					             Collection:
					             [
					               ThatTypes.HaveName.Tests.SomeClassToTestHaveNameForTypes
					             ]
					             """).AsWildcard();
			}
		}
	}
}
