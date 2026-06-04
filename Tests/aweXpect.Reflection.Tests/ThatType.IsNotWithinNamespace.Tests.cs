using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotWithinNamespace
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsNotWithinNamespace_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(ClassInSiblingNamespaceScope);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.IsNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope",
					             but it was in namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling"
					             """);
			}

			[Fact]
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

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotWithinNamespace("foo");
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not within namespace "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsWithinNamespace_ShouldFail()
			{
				Type subject = typeof(ClassInNestedNamespaceScope);

				async Task Act()
				{
					await That(subject)
						.IsNotWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope",
					             but it was in namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested"
					             """);
			}

			[Fact]
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
		}
	}
}
