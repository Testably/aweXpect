using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class HaveNamespace
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsTypeWithDifferentNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(ThatTypes),
				};

				async Task Act()
				{
					await That(subject).HaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have namespace equal to "aweXpect.Reflection.Tests.Test…",
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes
					             ]

					             Collection:
					             [
					               PublicClass,
					               ThatTypes
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEnumerableTypesHaveNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicEnum),
				};

				async Task Act()
				{
					await That(subject).HaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types");
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenPatternIsInvalid_ShouldThrowForAnEmptyCollection()
			{
				IEnumerable<Type?> subject = Array.Empty<Type?>();

				async Task Act()
				{
					await That(subject).HaveNamespace("[").AsRegex();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' regex pattern is invalid: *").AsWildcard().And
					.WithParamName("expected")
					.Because("the pattern is validated before any item is read");
			}

			[Test]
			public async Task WhenTypesContainTypeWithDifferentNamespace_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject).HaveNamespace("aweXpect.Reflection");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types with namespace ending with "ToVerifyingTheNamespaceOfIt" in assembly containing type ThatTypes.HaveNamespace.Tests
					             all have namespace equal to "aweXpect.Reflection",
					             but none of 1 did

					             Not matching items:
					             [
					               SomeClassToVerifyTheNamespaceOfIt
					             ]

					             Collection:
					             [
					               SomeClassToVerifyTheNamespaceOfIt
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTypesHaveNamespace_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject)
						.HaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject)
						.HaveNamespace("AWExPECT.rEFLECTION.tESTS.tESThELPERS.tYPES.tOvERIFYINGtHEnAMESPACEoFiT")
						.IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesMatchPrefix_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject).HaveNamespace("aweXpect.Reflection.Tests").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypesContainTypeWithDifferentNamespace_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveNamespace("aweXpect.Reflection"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesHaveNamespace_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.HaveNamespace(
							"aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types with namespace ending with "ToVerifyingTheNamespaceOfIt" in assembly containing type ThatTypes.HaveNamespace.Tests
					             not all have namespace equal to "aweXpect.Reflection.Tests.Test…",
					             but all 1 did

					             Collection:
					             [
					               SomeClassToVerifyTheNamespaceOfIt
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class QuantifierTests
		{
			[Test]
			public async Task WhenEnoughTypesMatchPrefix_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicEnum), typeof(string),
				};

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveNamespace("aweXpect.Reflection.Tests").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTooFewTypesMatchPrefixIgnoringCase_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(ThatTypes), typeof(string),
				};

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveNamespace("AWEXPECT.REFLECTION.TESTS.TESTHELPERS").AsPrefix()
						.IgnoringCase();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has namespace starting with "AWEXPECT.REFLECTION.TESTS.TEST…" ignoring case for at least 2 items,
					             but only 1 of 3 did

					             Not matching items:
					             [
					               ThatTypes,
					               string
					             ]

					             Collection:
					             [
					               PublicClass,
					               ThatTypes,
					               string
					             ]
					             """);
			}
		}
	}
}
