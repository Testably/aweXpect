using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DependsOnlyOn
	{
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
		private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";

		private const string ConsumersNamespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers";

		private const string OwnSubNamespace =
			"aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers.OwnSub";

		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllDependenciesAreAllowed_ShouldSucceed()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
					=> await That(subject).DependsOnlyOn(Layer1Namespace);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependingOnDisallowedNamespace_ShouldFail()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
					=> await That(subject).DependsOnlyOn(Layer1Namespace);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on namespace "{Layer1Namespace}",
					              but it also depended on ["{Layer2Namespace}"]
					              """);
			}

			[Fact]
			public async Task WhenAllowingMultipleNamespaces_ShouldSucceed()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
					=> await That(subject).DependsOnlyOn(Layer1Namespace, Layer2Namespace);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldSucceed()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
					=> await That(subject).DependsOnlyOn(Layer1Namespace).OrOn(Layer2Namespace);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoNamespaceIsSpecified_ShouldThrowArgumentException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
					=> await That(subject).DependsOnlyOn();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("At least one namespace must be specified.");
			}

			[Fact]
			public async Task WhenDependingOnlyOnOwnNamespace_ShouldSucceed()
			{
				Type subject = typeof(ReferencesOwnNamespace);

				async Task Act()
					=> await That(subject).DependsOnlyOn(Layer1Namespace);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExcludingSubNamespaces_OwnSubNamespaceStaysAllowed()
			{
				// ReferencesOwnSubNamespace (in ...Consumers) only references ...Consumers.OwnSub. By default the
				// type's own sub-namespaces remain allowed even when sub-namespaces are excluded.
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
					=> await That(subject).DependsOnlyOn(Layer1Namespace).ExcludingSubNamespaces();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExcludingOwnSubNamespaces_OwnSubNamespaceBecomesViolation()
			{
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
					=> await That(subject).DependsOnlyOn(Layer1Namespace)
						.ExcludingSubNamespaces().ExcludingOwnSubNamespaces();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on namespace "{Layer1Namespace}",
					              but it also depended on ["{OwnSubNamespace}"]
					              """);
			}

			[Fact]
			public async Task WhenOnlyExcludingOwnSubNamespaces_AllowedSubNamespacesStayIncluded()
			{
				// The two toggles are independent: the own sub-namespace is no longer implicitly allowed, but
				// the allowed namespaces still include their sub-namespaces, which covers it explicitly.
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
					=> await That(subject).DependsOnlyOn(ConsumersNamespace).ExcludingOwnSubNamespaces();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependingOnlyOnFramework_ShouldSucceed()
			{
				Type subject = typeof(FrameworkConsumer);

				async Task Act()
					=> await That(subject).DependsOnlyOn(Layer1Namespace);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependingOnGlobalNamespace_ShouldReportGlobalNamespace()
			{
				Type subject = typeof(ReferencesGlobal);

				async Task Act()
					=> await That(subject).DependsOnlyOn(Layer1Namespace);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on namespace "{Layer1Namespace}",
					              but it also depended on ["<global namespace>"]
					              """);
			}

			[Fact]
			public async Task WhenGlobalNamespaceIsAllowedWithEmptyString_ShouldSucceed()
			{
				// An empty string targets exactly the global namespace.
				Type subject = typeof(ReferencesGlobal);

				async Task Act()
					=> await That(subject).DependsOnlyOn(Layer1Namespace, "");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsInGlobalNamespaceAndDependsOnlyOnFramework_ShouldSucceed()
			{
				Type subject = typeof(GlobalNamespaceTarget);

				async Task Act()
					=> await That(subject).DependsOnlyOn(Layer1Namespace);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenDependingOnDisallowedNamespace_ShouldSucceed()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DependsOnlyOn(Layer1Namespace));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
