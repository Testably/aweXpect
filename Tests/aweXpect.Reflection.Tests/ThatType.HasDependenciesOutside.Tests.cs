using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Synthetic;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class HasDependenciesOutside
	{
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
		private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";

		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllDependenciesAreAllowed_ShouldFail()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}",
					              but it only depended on the allowed namespaces
					              """);
			}

			[Fact]
			public async Task WhenAllowingMultipleNamespaces_ShouldFail()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace, Layer2Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}" or "{Layer2Namespace}",
					              but it only depended on the allowed namespaces
					              """);
			}

			[Fact]
			public async Task WhenDependingOnGlobalNamespace_ShouldSucceed()
			{
				Type subject = typeof(ReferencesGlobal);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependingOnlyOnFramework_ShouldFail()
			{
				// Framework dependencies never count as outside.
				Type subject = typeof(FrameworkConsumer);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}",
					              but it only depended on the allowed namespaces
					              """);
			}

			[Fact]
			public async Task WhenDependingOnlyOnOwnNamespace_ShouldFail()
			{
				// The type's own namespace never counts as outside.
				Type subject = typeof(ReferencesOwnNamespace);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}",
					              but it only depended on the allowed namespaces
					              """);
			}

			[Fact]
			public async Task WhenDependingOnNamespaceOutsideAllowedSet_ShouldSucceed()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependingOnSubNamespaceOfAllowedNamespace_ShouldFail()
			{
				// ViaSubNamespace references only Layer1.Sub, which is covered by Layer1 by default.
				Type subject = typeof(ViaSubNamespace);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}",
					              but it only depended on the allowed namespaces
					              """);
			}

			[Fact]
			public async Task WhenExcludingOwnSubNamespaces_OwnSubNamespaceCountsAsOutside()
			{
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace).ExcludingOwnSubNamespaces();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExcludingSubNamespaces_OwnSubNamespaceStaysAllowed()
			{
				// ReferencesOwnSubNamespace (in ...Consumers) only references ...Consumers.OwnSub. By default the
				// type's own sub-namespaces never count as outside, even when sub-namespaces are excluded.
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace).ExcludingSubNamespaces();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}",
					              but it only depended on the allowed namespaces
					              """);
			}

			[Fact]
			public async Task WhenExcludingSubNamespaces_SubNamespaceDependencyCountsAsOutside()
			{
				Type subject = typeof(ViaSubNamespace);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace).ExcludingSubNamespaces();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenGlobalNamespaceIsAllowedWithEmptyString_ShouldFail()
			{
				// An empty string allows exactly the global namespace.
				Type subject = typeof(ReferencesGlobal);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace, "");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}" or "",
					              but it only depended on the allowed namespaces
					              """);
			}

			[Fact]
			public async Task WhenNoNamespaceIsSpecified_ShouldThrowArgumentException()
			{
				Type subject = typeof(OnlyLayer2);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside();
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("namespaces").And
					.WithMessage("The 'namespaces' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsInGlobalNamespaceAndDependsOnlyOnFramework_ShouldFail()
			{
				Type subject = typeof(GlobalNamespaceTarget);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}",
					              but it only depended on the allowed namespaces
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}",
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldFail()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Layer1Namespace).OrOn(Layer2Namespace);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}" or "{Layer2Namespace}",
					              but it only depended on the allowed namespaces
					              """);
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Fact]
			public async Task WhenAllDependenciesAreInTargetCollection_ShouldFail()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it only depended on the allowed types
					              """);
			}

			[Fact]
			public async Task WhenDependingOnTypeOutsideTargetCollections_ShouldSucceed()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExcludingOwnSubNamespaces_OwnSubNamespaceCountsAsOutside()
			{
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Types.InNamespace(Layer1Namespace))
						.ExcludingOwnSubNamespaces();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegated_ShouldFailForTypeWithDependencyOutside()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.HasDependenciesOutside(Types.InNamespace(Layer1Namespace)));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it had dependencies outside the allowed types

					              Disallowed dependencies:
					              [
					                "TargetB"
					              ]
					              """);
			}

			[Fact]
			public async Task WhenNegated_ShouldSucceedForTypeDependingOnlyOnAllowedTypes()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.HasDependenciesOutside(Types.InNamespace(Layer1Namespace)));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegatedAndDistinctViolatorsShareTheSimpleName_ShouldQualifyThemByNamespace()
			{
				// Both AmbiguousTarget dependencies are outside the allowed set; they must stay apart in the
				// message instead of collapsing into one indistinguishable "AmbiguousTarget" entry.
				Type subject = typeof(WithSameNamedDependencies);

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.HasDependenciesOutside(Types.InNamespace(Layer1Namespace)));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("*AmbiguousA.AmbiguousTarget*AmbiguousB.AmbiguousTarget*").AsWildcard();
			}

			[Fact]
			public async Task WhenReferencingOwnSubNamespace_ShouldFailByDefault()
			{
				Type subject = typeof(ReferencesOwnSubNamespace);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it only depended on the allowed types
					              """);
			}

			[Fact]
			public async Task WhenTargetCollectionIsEmpty_DisallowedDependencyShouldSucceed()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Types.InNamespace("Non.Existent.Namespace"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTargetCollectionIsEmpty_FrameworkDependenciesStayInside()
			{
				Type subject = typeof(FrameworkConsumer);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Types.InNamespace("Non.Existent.Namespace"));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenTargetCollectionIsEmpty_OwnNamespaceStaysInside()
			{
				Type subject = typeof(ReferencesOwnNamespace);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Types.InNamespace("Non.Existent.Namespace"));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldFail()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).HasDependenciesOutside(Types.InNamespace(Layer1Namespace))
						.OrOn(Types.InNamespace(Layer2Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies or types within namespace "{Layer2Namespace}" in all loaded assemblies,
					              but it only depended on the allowed types
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllDependenciesAreAllowed_ShouldSucceed()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasDependenciesOutside(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependingOnNamespaceOutsideAllowedSet_ShouldFail()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasDependenciesOutside(Layer1Namespace));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have dependencies outside namespace "{Layer1Namespace}",
					              but it had dependencies outside the allowed namespaces

					              Disallowed dependencies:
					              [
					                "{Layer2Namespace}"
					              ]
					              """);
			}

			[Fact]
			public async Task WhenUsedForItems_ShouldLabelTheDisallowedDependenciesWithTheItem()
			{
				Type[] subject = [typeof(OnlyLayer1), typeof(Layer1AndLayer2), typeof(ReferencesGlobal),];

				async Task Act()
				{
					await That(subject).All()
						.ComplyWith(type => type.DoesNotComplyWith(it => it.HasDependenciesOutside(Layer1Namespace)));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have dependencies outside namespace "{Layer1Namespace}" for all items,
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
						That(typeof(Layer1AndLayer2)).DoesNotComplyWith(it => it.HasDependenciesOutside(Layer1Namespace)),
						That(typeof(OnlyLayer1)).DoesNotComplyWith(it => it.HasDependenciesOutside(Layer1Namespace)),
						That(typeof(ReferencesGlobal)).DoesNotComplyWith(it => it.HasDependenciesOutside(Layer1Namespace)));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected all of the following to succeed:
					               [01] Expected that typeof(Layer1AndLayer2) does not have dependencies outside namespace "{Layer1Namespace}"
					               [02] Expected that typeof(OnlyLayer1) does not have dependencies outside namespace "{Layer1Namespace}"
					               [03] Expected that typeof(ReferencesGlobal) does not have dependencies outside namespace "{Layer1Namespace}"
					              but
					               [01] it had dependencies outside the allowed namespaces
					               [03] it had dependencies outside the allowed namespaces

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
					await That(subject).Whose(holder => holder.Type,
						type => type.DoesNotComplyWith(it => it.HasDependenciesOutside(Layer1Namespace)));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              whose Type does not have dependencies outside namespace "{Layer1Namespace}",
					              but it had dependencies outside the allowed namespaces

					              Disallowed dependencies (Type):
					              [
					                "{Layer2Namespace}"
					              ]
					              """);
			}

			private sealed class TypeHolder(Type type)
			{
				public Type Type { get; } = type;
			}
		}
	}
}
