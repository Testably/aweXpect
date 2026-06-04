using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling;
using Xunit.Sdk;
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
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("*are all within namespace*but it contained not matching types*ClassInSiblingNamespaceScope*")
					.AsWildcard();
			}

			[Fact]
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

			[Fact]
			public async Task WhenTypesAreWithinNamespace_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				async Task Act()
				{
					await That(subject)
						.AreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types with namespace starting with "aweXpect.Reflection.Tests.Test…" in assembly containing type ThatTypes.AreWithinNamespace.Tests
					             are all within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope",
					             but it contained not matching types [
					               *ClassInSiblingNamespaceScope*
					             ]
					             """).AsWildcard();
			}

#if NET8_0_OR_GREATER
			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("*are all within namespace*but it contained not matching types*ClassInSiblingNamespaceScope*")
					.AsWildcard();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypesAreWithinNamespace_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<Tests>()
					.Types().WithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("*are not all within namespace*NamespaceScope*").AsWildcard();
			}
		}
	}
}
