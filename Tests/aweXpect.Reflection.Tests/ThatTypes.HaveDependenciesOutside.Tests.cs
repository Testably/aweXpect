using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using Xunit.Sdk;
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
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside namespace "{Layer1Namespace}" or "{Layer2Namespace}",
					              but it contained types depending only on the allowed namespaces [
					                Layer1AndLayer2
					              ]
					              """);
			}

			[Fact]
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

			[Fact]
			public async Task WhenCollectionContainsNull_ShouldListNull()
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside namespace "{Layer1Namespace}",
					              but it contained types depending only on the allowed namespaces [
					                <null>
					              ]
					              """);
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside namespace "{Layer1Namespace}",
					              but it contained types depending only on the allowed namespaces [
					                OnlyLayer1
					              ]
					              """);
			}

#if NET8_0_OR_GREATER
			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside namespace "{Layer1Namespace}",
					              but it contained types depending only on the allowed namespaces [
					                OnlyLayer1
					              ]
					              """);
			}
#endif
		}

		public sealed class FilteredTypesTargetTests
		{
			[Fact]
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

			[Fact]
			public async Task WhenCollectionContainsNull_ShouldListNull()
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it contained types depending only on the allowed types [
					                <null>
					              ]
					              """);
			}

			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              not all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it only contained types with dependencies outside the allowed types [
					                Layer1AndLayer2 depends on ["TargetB"],
					                OnlyLayer2 depends on ["TargetB"]
					              ]
					              """);
			}

			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it contained types depending only on the allowed types [
					                ReferencesOwnSubNamespace
					              ]
					              """);
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it contained types depending only on the allowed types [
					                OnlyLayer1
					              ]
					              """);
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies or types within namespace "{Layer2Namespace}" in all loaded assemblies,
					              but it contained types depending only on the allowed types [
					                Layer1AndLayer2
					              ]
					              """);
			}

#if NET8_0_OR_GREATER
			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              all have dependencies outside types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it contained types depending only on the allowed types [
					                OnlyLayer1
					              ]
					              """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              not all have dependencies outside namespace "{Layer1Namespace}",
					              but it only contained types with dependencies outside the allowed namespaces [
					                Layer1AndLayer2 depends on ["{Layer2Namespace}"],
					                OnlyLayer2 depends on ["{Layer2Namespace}"]
					              ]
					              """);
			}

			[Fact]
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
