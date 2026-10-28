using System.Collections.Generic;
using aweXpect.Customization;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DependOnlyOn
	{
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
		private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";

		public sealed class Tests
		{
			[Test]
			public async Task WhenAllDependenciesAreAllowed_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(FrameworkConsumer),
					typeof(ReferencesOwnNamespace),
				];

				async Task Act()
				{
					await That(subject).DependOnlyOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllowingAllNamespaces_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).DependOnlyOn(Layer1Namespace, Layer2Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAsManyTypesHaveDisallowedDependenciesAsTheMaximumNumberOfCollectionItems_ShouldListAll()
			{
				using IDisposable _ = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(2);
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					typeof(OnlyLayer2),
				];

				async Task Act()
				{
					await That(subject).DependOnlyOn(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all depend only on namespace "{Layer1Namespace}",
					              but it contained types with disallowed dependencies

					              Not matching items:
					              [
					                Layer1AndLayer2 depends on ["{Layer2Namespace}"],
					                OnlyLayer2 depends on ["{Layer2Namespace}"]
					              ]
					              """)
					.Because("the list is only limited when it has more items than the maximum");
			}

			[Test]
			public async Task WhenCollectionContainsNull_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					null,
				];

				async Task Act()
				{
					await That(subject).DependOnlyOn(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all depend only on namespace "{Layer1Namespace}",
					              but for the item at index 1, it was <null>

					              Collection:
					              [
					                OnlyLayer1,
					                <null>
					              ]
					              """);
			}

			[Test]
			public async Task WhenMoreTypesHaveDisallowedDependenciesThanTheMaximumNumberOfCollectionItems_ShouldLimitTheList()
			{
				using IDisposable _ = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(1);
				IEnumerable<Type?> subject =
				[
					typeof(Layer1AndLayer2),
					typeof(OnlyLayer2),
				];

				async Task Act()
				{
					await That(subject).DependOnlyOn(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all depend only on namespace "{Layer1Namespace}",
					              but it contained types with disallowed dependencies

					              Not matching items:
					              [
					                Layer1AndLayer2 depends on ["{Layer2Namespace}"],
					                (… and 1 more)
					              ]
					              """)
					.Because("the not matching types are limited like any other collection in a failure message");
			}

			[Test]
			public async Task WhenSomeTypeDependsOnDisallowedNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).DependOnlyOn(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all depend only on namespace "{Layer1Namespace}",
					              but it contained types with disallowed dependencies

					              Not matching items:
					              [
					                Layer1AndLayer2 depends on ["{Layer2Namespace}"]
					              ]
					              """);
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Test]
			public async Task WhenAllDependenciesAreInTargetCollection_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(FrameworkConsumer),
					typeof(ReferencesOwnNamespace),
				];

				async Task Act()
				{
					await That(subject).DependOnlyOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExcludingOwnSubNamespaces_OwnSubNamespaceBecomesViolation()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(ReferencesOwnSubNamespace),
				];

				async Task Act()
				{
					await That(subject).DependOnlyOn(Types.InNamespace(Layer1Namespace))
						.ExcludingOwnSubNamespaces();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all depend only on types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it contained types with disallowed dependencies

					              Not matching items:
					              [
					                ReferencesOwnSubNamespace depends on ["OwnSubTarget"]
					              ]
					              """);
			}

			[Test]
			public async Task WhenSomeTypeDependsOnTypeOutsideTargetCollections_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).DependOnlyOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all depend only on types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it contained types with disallowed dependencies

					              Not matching items:
					              [
					                Layer1AndLayer2 depends on ["TargetB"]
					              ]
					              """);
			}

			[Test]
			public async Task WhenTargetCollectionIsEmpty_ShouldStillAllowOwnNamespaceAndFramework()
			{
				IEnumerable<Type?> subject =
				[
					typeof(FrameworkConsumer),
					typeof(ReferencesOwnNamespace),
				];

				async Task Act()
				{
					await That(subject).DependOnlyOn(Types.InNamespace("Non.Existent.Namespace"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).DependOnlyOn(Types.InNamespace(Layer1Namespace))
						.OrOn(Types.InNamespace(Layer2Namespace));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
