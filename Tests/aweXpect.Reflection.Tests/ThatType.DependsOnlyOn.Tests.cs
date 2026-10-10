using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Synthetic;
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
				{
					await That(subject).DependsOnlyOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllowingMultipleNamespaces_ShouldSucceed()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Layer1Namespace, Layer2Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependingOnDisallowedNamespace_ShouldFail()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Layer1Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on namespace "{Layer1Namespace}",
					              but it had disallowed dependencies

					              Disallowed dependencies:
					              [
					                "{Layer2Namespace}"
					              ]
					              """);
			}

			[Fact]
			public async Task WhenDependingOnGlobalNamespace_ShouldReportGlobalNamespace()
			{
				Type subject = typeof(ReferencesGlobal);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Layer1Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on namespace "{Layer1Namespace}",
					              but it had disallowed dependencies

					              Disallowed dependencies:
					              [
					                "<global namespace>"
					              ]
					              """);
			}

			[Fact]
			public async Task WhenDependingOnlyOnFramework_ShouldSucceed()
			{
				Type subject = typeof(FrameworkConsumer);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependingOnlyOnOwnNamespace_ShouldSucceed()
			{
				Type subject = typeof(ReferencesOwnNamespace);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExcludingOwnSubNamespaces_OwnSubNamespaceBecomesViolation()
			{
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Layer1Namespace)
						.ExcludingSubNamespaces().ExcludingOwnSubNamespaces();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on namespace "{Layer1Namespace}",
					              but it had disallowed dependencies

					              Disallowed dependencies:
					              [
					                "{OwnSubNamespace}"
					              ]
					              """);
			}

			[Fact]
			public async Task WhenExcludingSubNamespaces_OwnSubNamespaceStaysAllowed()
			{
				// ReferencesOwnSubNamespace (in ...Consumers) only references ...Consumers.OwnSub. By default the
				// type's own sub-namespaces remain allowed even when sub-namespaces are excluded.
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Layer1Namespace).ExcludingSubNamespaces();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenGlobalNamespaceIsAllowedWithEmptyString_ShouldSucceed()
			{
				// An empty string targets exactly the global namespace.
				Type subject = typeof(ReferencesGlobal);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Layer1Namespace, "");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoNamespaceIsSpecified_ShouldThrowArgumentException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOnlyOn();
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("namespaces").And
					.WithMessage("The 'namespaces' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenOnlyExcludingOwnSubNamespaces_AllowedSubNamespacesStayIncluded()
			{
				// The two toggles are independent: the own sub-namespace is no longer implicitly allowed, but
				// the allowed namespaces still include their sub-namespaces, which covers it explicitly.
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(ConsumersNamespace).ExcludingOwnSubNamespaces();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsInGlobalNamespaceAndDependsOnlyOnFramework_ShouldSucceed()
			{
				Type subject = typeof(GlobalNamespaceTarget);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedForItems_ShouldLabelTheDisallowedDependenciesWithTheItem()
			{
				Type[] subject = [typeof(OnlyLayer1), typeof(Layer1AndLayer2), typeof(ReferencesGlobal),];

				async Task Act()
				{
					await That(subject).All().ComplyWith(type => type.DependsOnlyOn(Layer1Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on namespace "{Layer1Namespace}" for all items,
					              but only 1 of 3 did

					              Not matching items:
					              [
					                Layer1AndLayer2,
					                ReferencesGlobal
					              ]

					              Collection:
					              [
					                OnlyLayer1,
					                Layer1AndLayer2,
					                ReferencesGlobal
					              ]

					              Disallowed dependencies (item [1]):
					              [
					                "{Layer2Namespace}"
					              ]
					              """);
			}

			[Fact]
			public async Task WhenUsedInThatAll_ShouldListTheDisallowedDependenciesOfEachFailure()
			{
				async Task Act()
				{
					await ThatAll(
						That(typeof(Layer1AndLayer2)).DependsOnlyOn(Layer1Namespace),
						That(typeof(OnlyLayer1)).DependsOnlyOn(Layer1Namespace),
						That(typeof(ReferencesGlobal)).DependsOnlyOn(Layer1Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected all of the following to succeed:
					               [01] Expected that typeof(Layer1AndLayer2) depends only on namespace "{Layer1Namespace}"
					               [02] Expected that typeof(OnlyLayer1) depends only on namespace "{Layer1Namespace}"
					               [03] Expected that typeof(ReferencesGlobal) depends only on namespace "{Layer1Namespace}"
					              but
					               [01] it had disallowed dependencies
					               [03] it had disallowed dependencies

					              [01] Disallowed dependencies:
					              [
					                "{Layer2Namespace}"
					              ]

					              [03] Disallowed dependencies:
					              [
					                "<global namespace>"
					              ]
					              """);
			}

			[Fact]
			public async Task WhenUsedInWhose_ShouldLabelTheDisallowedDependenciesWithTheMember()
			{
				TypeHolder subject = new(typeof(Layer1AndLayer2));

				async Task Act()
				{
					await That(subject).Whose(holder => holder.Type, type => type.DependsOnlyOn(Layer1Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              whose Type depends only on namespace "{Layer1Namespace}",
					              but Type had disallowed dependencies

					              Disallowed dependencies (Type):
					              [
					                "{Layer2Namespace}"
					              ]
					              """);
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldSucceed()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Layer1Namespace).OrOn(Layer2Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			private sealed class TypeHolder(Type type)
			{
				public Type Type { get; } = type;
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Fact]
			public async Task WhenAllDependenciesAreInTargetCollection_ShouldSucceed()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependingOnTypeOutsideTargetCollections_ShouldFail()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it had disallowed dependencies

					              Disallowed dependencies:
					              [
					                "TargetB"
					              ]
					              """);
			}

			[Fact]
			public async Task WhenDistinctViolatorsShareTheSimpleName_ShouldQualifyThemByNamespace()
			{
				// Both AmbiguousTarget dependencies are disallowed; they must stay apart in the message
				// instead of collapsing into one indistinguishable "AmbiguousTarget" entry.
				Type subject = typeof(WithSameNamedDependencies);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("*AmbiguousA.AmbiguousTarget*AmbiguousB.AmbiguousTarget*").AsWildcard();
			}

			[Fact]
			public async Task WhenExcludingOwnSubNamespaces_OwnSubNamespaceBecomesViolation()
			{
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Types.InNamespace(Layer1Namespace))
						.ExcludingOwnSubNamespaces();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it had disallowed dependencies

					              Disallowed dependencies:
					              [
					                "OwnSubTarget"
					              ]
					              """);
			}

			[Fact]
			public async Task WhenExcludingOwnSubNamespaces_TargetCollectionCoveringTheSubNamespaceStillAllowsIt()
			{
				// The own sub-namespace is no longer implicitly allowed, but the target collection contains the
				// referenced OwnSub type, which covers it explicitly.
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Types.InNamespace(ConsumersNamespace))
						.ExcludingOwnSubNamespaces();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegated_ShouldSucceedForDisallowedDependency()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.DependsOnlyOn(Types.InNamespace(Layer1Namespace)));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenReferencingOwnSubNamespace_ShouldSucceedByDefault()
			{
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTargetCollectionIsEmpty_DisallowedDependencyShouldFail()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Types.InNamespace("Non.Existent.Namespace"));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenTargetCollectionIsEmpty_ShouldStillAllowFrameworkDependencies()
			{
				Type subject = typeof(FrameworkConsumer);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Types.InNamespace("Non.Existent.Namespace"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTargetCollectionIsEmpty_ShouldStillAllowOwnNamespace()
			{
				Type subject = typeof(ReferencesOwnNamespace);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Types.InNamespace("Non.Existent.Namespace"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldSucceed()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).DependsOnlyOn(Types.InNamespace(Layer1Namespace))
						.OrOn(Types.InNamespace(Layer2Namespace));
				}

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
				{
					await That(subject).DoesNotComplyWith(it => it.DependsOnlyOn(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependingOnlyOnAllowedNamespaces_ShouldFail()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DependsOnlyOn(Layer1Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not depend only on namespace "{Layer1Namespace}",
					              but it only depended on the allowed namespaces
					              """);
			}
		}
	}
}
