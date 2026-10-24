using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotWithinNamespace
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsNotWithinNamespace_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(ClassInSiblingNamespaceScope);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.IsNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope",
					             but it was in namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling"
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotWithinNamespace_ShouldSucceed()
			{
				Type subject = typeof(ClassInSiblingNamespaceScope);

				async Task Act()
				{
					await That(subject)
						.IsNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotWithinNamespace("foo");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not within namespace "foo",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsWithinNamespace_ShouldFail()
			{
				Type subject = typeof(ClassInNestedNamespaceScope);

				async Task Act()
				{
					await That(subject)
						.IsNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope",
					             but it was in namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested"
					             """);
			}

			[Test]
			public async Task WhenTypeIsWithinNamespace_ShouldSucceedWithNegatedAssertion()
			{
				Type subject = typeof(ClassInNestedNamespaceScope);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.IsNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				Type subject = typeof(ClassInSiblingNamespaceScope);

				async Task Act()
				{
					await That(subject).IsNotWithinNamespace(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' cannot be null.").AsPrefix();
			}
		}
	}
}
