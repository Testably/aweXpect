using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreWithinNamespace
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEnumerableContainsTypeInOtherNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassInNamespaceScope),
					typeof(ClassInSiblingNamespaceScope),
				];

				async Task Act()
				{
					await That(subject)
						.AreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*all are within namespace*but only * of * were*ClassInSiblingNamespaceScope*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenEnumerableTypesAreWithinNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassInNamespaceScope),
					typeof(ClassInNestedNamespaceScope),
				];

				async Task Act()
				{
					await That(subject)
						.AreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<Type?> subject = [typeof(ClassInNamespaceScope),];

				async Task Act()
				{
					await That(subject).AreWithinNamespace(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenTypesAreWithinNamespace_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WhichAreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				async Task Act()
				{
					await That(subject)
						.AreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesContainTypeInOtherNamespace_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope")
					.AsPrefix();

				async Task Act()
				{
					await That(subject)
						.AreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types with namespace starting with "aweXpect.Reflection.Tests.Test…" in assembly containing type ThatTypes.AreWithinNamespace.Tests
					             all are within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope",
					             but only 2 of 3 were

					             Not matching items:
					             [
					               *ClassInSiblingNamespaceScope*
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableTypesAreWithinNamespace_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassInNamespaceScope), typeof(ClassInNestedNamespaceScope),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject)
						.AreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAsyncEnumerableContainsTypeInOtherNamespace_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassInNamespaceScope), typeof(ClassInSiblingNamespaceScope),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject)
						.AreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*all are within namespace*but only * of * were*ClassInSiblingNamespaceScope*")
					.AsWildcard();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypesAreWithinNamespace_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WhichAreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*not all are within namespace*NamespaceScope*").AsWildcard();
			}
		}
	}
}
