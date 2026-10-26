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
	public sealed class AreNotWithinNamespace
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEnumerableContainsTypeWithinNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassInSiblingNamespaceScope),
					typeof(ClassInNestedNamespaceScope),
				];

				async Task Act()
				{
					await That(subject)
						.AreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).Throws<FailException>()
					.WithMessage(
						"*none are within namespace*but * of * were*ClassInNestedNamespaceScope*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenEnumerableTypesAreNotWithinNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassInSiblingNamespaceScope),
				];

				async Task Act()
				{
					await That(subject)
						.AreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesAreNotWithinNamespace_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WhichAreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				async Task Act()
				{
					await That(subject)
						.AreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<Type?> subject = [typeof(ClassInSiblingNamespaceScope),];

				async Task Act()
				{
					await That(subject).AreNotWithinNamespace(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableTypesAreNotWithinNamespace_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassInSiblingNamespaceScope),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject)
						.AreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAsyncEnumerableContainsTypeWithinNamespace_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassInSiblingNamespaceScope), typeof(ClassInNestedNamespaceScope),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject)
						.AreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).Throws<FailException>()
					.WithMessage(
						"*none are within namespace*but * of * were*ClassInNestedNamespaceScope*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenUnexpectedIsNullForAsyncEnumerable_ShouldThrowArgumentNullException()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassInSiblingNamespaceScope),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AreNotWithinNamespace(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEnumerableContainsTypeWithinNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassInNamespaceScope),
					typeof(ClassInNestedNamespaceScope),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesAreNotWithinNamespace_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WhichAreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*at least one is within namespace*NamespaceScope*").AsWildcard();
			}
		}
	}
}
