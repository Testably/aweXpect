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
					             all have namespace not equal to "aweXpect",
					             but it contained not matching types

					             Not matching items:
					             [
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
					             all have namespace not equal to "aweXpect",
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
					             all have namespace not equal to "aweXpect.Reflection.Tests.Test…",
					             but it contained not matching types

					             Not matching items:
					             [
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
					             all have namespace not equal to "AWEXPECT.REFLECTION.TESTS.TEST…" ignoring case,
					             but it contained not matching types

					             Not matching items:
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
					             all have namespace not starting with "aweXpect.Reflection.Tests",
					             but it contained not matching types

					             Not matching items:
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
					             not all have namespace not equal to "aweXpect",
					             but it only contained matching types

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
					             all have namespace not equal to "aweXpect.Reflection.Tests.Test…",
					             but it contained not matching types

					             Not matching items:
					             [
					               PublicClass
					             ]
					             """);
			}
		}
#endif
	}
}
