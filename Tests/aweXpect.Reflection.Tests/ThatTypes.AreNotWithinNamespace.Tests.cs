using System.Collections.Generic;
using aweXpect.Reflection.Collections;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotWithinNamespace
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypesAreNotWithinNamespace_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().NotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				async Task Act()
				{
					await That(subject)
						.AreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage(
						"*are all not within namespace*but it contained not matching types*ClassInNestedNamespaceScope*")
					.AsWildcard();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenAsyncEnumerableTypesAreNotWithinNamespace_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new Type?[]
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

			[Fact]
			public async Task WhenAsyncEnumerableContainsTypeWithinNamespace_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new Type?[]
				{
					typeof(ClassInSiblingNamespaceScope),
					typeof(ClassInNestedNamespaceScope),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject)
						.AreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage(
						"*are all not within namespace*but it contained not matching types*ClassInNestedNamespaceScope*")
					.AsWildcard();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypesAreNotWithinNamespace_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().NotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AreNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("*also contain a type within namespace*NamespaceScope*").AsWildcard();
			}

			[Fact]
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
		}
	}
}
