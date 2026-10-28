using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class HaveDependenciesOutside
	{
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
		private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";

		public sealed class Tests
		{
			[Test]
			public async Task WhenAllowingAllNamespaces_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Layer1Namespace, Layer2Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside namespace "{Layer1Namespace}" or "{Layer2Namespace}",
					              but it contained types depending only on the allowed namespaces

					              Not matching items:
					              [
					                Layer1AndLayer2
					              ]
					              """);
			}

			[Test]
			public async Task WhenAllTypesHaveDependenciesOutside_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					typeof(OnlyLayer2),
				];

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCollectionContainsNull_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					null,
				];

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside namespace "{Layer1Namespace}",
					              but for the item at index 1, it was <null>

					              Collection:
					              [
					                Layer1AndLayer2,
					                <null>
					              ]
					              """);
			}

			[Test]
			public async Task WhenSomeTypeDependsOnlyOnAllowedNamespaces_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside namespace "{Layer1Namespace}",
					              but it contained types depending only on the allowed namespaces

					              Not matching items:
					              [
					                OnlyLayer1
					              ]
					              """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableTypesHaveDependenciesOutside_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Layer1AndLayer2), typeof(OnlyLayer2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAsyncEnumerableContainsTypeDependingOnlyOnAllowedNamespaces_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Layer1AndLayer2), typeof(OnlyLayer1),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside namespace "{Layer1Namespace}",
					              but it contained types depending only on the allowed namespaces

					              Not matching items:
					              [
					                OnlyLayer1
					              ]
					              """);
			}
#endif
		}

		public sealed class FilteredTypesTargetTests
		{
			[Test]
			public async Task WhenAllTypesHaveDependenciesOutsideTargetCollection_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					typeof(OnlyLayer2),
				];

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCollectionContainsNull_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					null,
				];

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but for the item at index 1, it was <null>

					              Collection:
					              [
					                Layer1AndLayer2,
					                <null>
					              ]
					              """);
			}

			[Test]
			public async Task WhenExcludingOwnSubNamespaces_OwnSubNamespaceCountsAsOutside()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ReferencesOwnSubNamespace),
				];

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Types.InNamespace(Layer1Namespace))
						.ExcludingOwnSubNamespaces();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegated_ShouldFailWhenAllTypesHaveDependenciesOutside()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					typeof(OnlyLayer2),
				];

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(they => they.HaveDependenciesOutside(Types.InNamespace(Layer1Namespace)));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              not all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it only contained types with dependencies outside the allowed types

					              Collection:
					              [
					                Layer1AndLayer2 depends on ["TargetB"],
					                OnlyLayer2 depends on ["TargetB"]
					              ]
					              """);
			}

			[Test]
			public async Task WhenNegated_ShouldSucceedWhenSomeTypeDependsOnlyOnTargetCollections()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(they => they.HaveDependenciesOutside(Types.InNamespace(Layer1Namespace)));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenReferencingOwnSubNamespace_ShouldFailByDefault()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ReferencesOwnSubNamespace),
				];

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it contained types depending only on the allowed types

					              Not matching items:
					              [
					                ReferencesOwnSubNamespace
					              ]
					              """);
			}

			[Test]
			public async Task WhenSomeTypeDependsOnlyOnTargetCollections_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it contained types depending only on the allowed types

					              Not matching items:
					              [
					                OnlyLayer1
					              ]
					              """);
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Types.InNamespace(Layer1Namespace))
						.OrOn(Types.InNamespace(Layer2Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies or types within namespace "{Layer2Namespace}" in all loaded assemblies,
					              but it contained types depending only on the allowed types

					              Not matching items:
					              [
					                Layer1AndLayer2
					              ]
					              """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableTypesHaveDependenciesOutsideTargetCollection_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Layer1AndLayer2), typeof(OnlyLayer2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAsyncEnumerableContainsTypeDependingOnlyOnTargetCollections_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Layer1AndLayer2), typeof(OnlyLayer1),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveDependenciesOutside(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it contained types depending only on the allowed types

					              Not matching items:
					              [
					                OnlyLayer1
					              ]
					              """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesHaveDependenciesOutside_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					typeof(OnlyLayer2),
				];

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(they => they.HaveDependenciesOutside(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              not all have dependencies outside namespace "{Layer1Namespace}",
					              but it only contained types with dependencies outside the allowed namespaces

					              Collection:
					              [
					                Layer1AndLayer2 depends on ["{Layer2Namespace}"],
					                OnlyLayer2 depends on ["{Layer2Namespace}"]
					              ]
					              """);
			}

			[Test]
			public async Task WhenSomeTypeDependsOnlyOnAllowedNamespaces_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(they => they.HaveDependenciesOutside(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
