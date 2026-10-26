using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsWithinNamespace
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Type subject = typeof(ClassInNamespaceScope);

				async Task Act()
				{
					await That(subject).IsWithinNamespace(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNamespaceDiffersOnlyByCase_ShouldFail()
			{
				Type subject = typeof(ClassInNestedNamespaceScope);

				async Task Act()
				{
					await That(subject)
						.IsWithinNamespace("AWEXPECT.REFLECTION.TESTS.TESTHELPERS.TYPES.NAMESPACESCOPE");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is within namespace "AWEXPECT.REFLECTION.TESTS.TESTHELPERS.TYPES.NAMESPACESCOPE",
					             but it was in namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested"
					             """);
			}

			[Test]
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

			[Test]
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

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsWithinNamespace("foo");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is within namespace "foo",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeOnlySharesPrefix_ShouldFail()
			{
				Type subject = typeof(ClassInSiblingNamespaceScope);

				async Task Act()
				{
					await That(subject)
						.IsWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope",
					             but it was in namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling"
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
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

			[Test]
			public async Task WhenTypeIsWithinNamespace_ShouldFail()
			{
				Type subject = typeof(ClassInNestedNamespaceScope);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.IsWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is not within namespace*NamespaceScope*").AsWildcard();
			}
		}
	}
}
