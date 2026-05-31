using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsWithinNamespace
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeHasExactNamespace_ShouldSucceed()
			{
				Type subject = typeof(ClassInNamespaceScope);

				async Task Act()
				{
					await That(subject)
						.IsWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsInSubNamespace_ShouldSucceed()
			{
				Type subject = typeof(ClassInNestedNamespaceScope);

				async Task Act()
				{
					await That(subject)
						.IsWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsWithinNamespace("foo");
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is within namespace "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenNamespaceDiffersOnlyByCase_ShouldFail()
			{
				Type subject = typeof(ClassInNestedNamespaceScope);

				async Task Act()
				{
					await That(subject)
						.IsWithinNamespace("AWEXPECT.REFLECTION.TESTS.TESTHELPERS.TYPES.NAMESPACESCOPE");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is within namespace "AWEXPECT.REFLECTION.TESTS.TESTHELPERS.TYPES.NAMESPACESCOPE",
					             but it was in namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested"
					             """);
			}

			[Fact]
			public async Task WhenTypeOnlySharesPrefix_ShouldFail()
			{
				Type subject = typeof(ClassInSiblingNamespaceScope);

				async Task Act()
				{
					await That(subject)
						.IsWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope",
					             but it was in namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling"
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsNotWithinNamespace_ShouldSucceed()
			{
				Type subject = typeof(ClassInSiblingNamespaceScope);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.IsWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsWithinNamespace_ShouldFail()
			{
				Type subject = typeof(ClassInNestedNamespaceScope);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.IsWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("*is not within namespace*NamespaceScope*").AsWildcard();
			}
		}
	}
}
