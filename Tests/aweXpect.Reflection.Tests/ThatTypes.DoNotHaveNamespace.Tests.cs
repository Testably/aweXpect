using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DoNotHaveNamespace
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [];

				async Task Act()
				{
					await That(subject).DoNotHaveNamespace("aweXpect");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNamespaceIsOnlyParentNamespace_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject).DoNotHaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types");
				}

				await That(Act).DoesNotThrow()
					.Because("unlike AreNotWithinNamespace, the whole namespace is compared as a string");
			}

			[Test]
			public async Task WhenOneTypeIsNull_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ThatTypes), null,
				};

				async Task Act()
				{
					await That(subject).DoNotHaveNamespace("aweXpect");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have namespace equal to "aweXpect",
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes,
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<Type?>? subject = null;

				async Task Act()
				{
					await That(subject).DoNotHaveNamespace("aweXpect");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have namespace equal to "aweXpect",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypesHaveNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ThatTypes), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have namespace equal to "aweXpect.Reflection.Tests.Test…",
					             but 1 of 2 did

					             Matching items:
					             [
					               PublicClass
					             ]

					             Collection:
					             [
					               ThatTypes,
					               PublicClass
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesMatchIgnoringCase_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject)
						.DoNotHaveNamespace("AWEXPECT.REFLECTION.TESTS.TESTHELPERS.TYPES.TOVERIFYINGTHENAMESPACEOFIT")
						.IgnoringCase();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types with namespace ending with "ToVerifyingTheNamespaceOfIt" in assembly containing type ThatTypes.DoNotHaveNamespace.Tests
					             none have namespace equal to "AWEXPECT.REFLECTION.TESTS.TEST…" ignoring case,
					             but 1 of 1 did

					             Matching items:
					             [
					               SomeClassToVerifyTheNamespaceOfIt
					             ]

					             Collection:
					             [
					               SomeClassToVerifyTheNamespaceOfIt
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesMatchPrefix_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("ToVerifyingTheNamespaceOfIt").AsSuffix();

				async Task Act()
				{
					await That(subject).DoNotHaveNamespace("aweXpect.Reflection.Tests").AsPrefix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types with namespace ending with "ToVerifyingTheNamespaceOfIt" in assembly containing type ThatTypes.DoNotHaveNamespace.Tests
					             none have namespace starting with "aweXpect.Reflection.Tests",
					             but 1 of 1 did

					             Matching items:
					             [
					               SomeClassToVerifyTheNamespaceOfIt
					             ]

					             Collection:
					             [
					               SomeClassToVerifyTheNamespaceOfIt
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoTypeHasNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ThatTypes),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveNamespace("aweXpect"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has namespace equal to "aweXpect",
					             but none of 1 did

					             Collection:
					             [
					               ThatTypes
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneTypeHasNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ThatTypes), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.DoNotHaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types"));
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenNoTypeHasNamespace_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ThatTypes),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveNamespace("aweXpect");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneTypeHasNamespace_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ThatTypes), typeof(PublicClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have namespace equal to "aweXpect.Reflection.Tests.Test…",
					             but 1 of 2 did

					             Matching items:
					             [
					               PublicClass
					             ]

					             Collection:
					             [
					               ThatTypes,
					               PublicClass
					             ]
					             """);
			}
		}
#endif
	}
}
