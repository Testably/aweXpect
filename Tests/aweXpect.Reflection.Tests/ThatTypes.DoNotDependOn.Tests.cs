using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DoNotDependOn
	{
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
		private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";

		public sealed class Tests
		{
			[Test]
			public async Task WhenCollectionContainsNull_ShouldFail()
			{
				// A null item's dependencies cannot be verified, so it fails the negative assertion just
				// like it fails DependOn and DependOnlyOn, instead of slipping through.
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					null,
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Layer2Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all do not depend on namespace "{Layer2Namespace}",
					              but it contained types with the dependency

					              Not matching items:
					              [
					                <null>
					              ]
					              """);
			}

			[Test]
			public async Task WhenNoTypeDependsOnNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Layer2Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypeDependsOnNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Layer2Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all do not depend on namespace "{Layer2Namespace}",
					              but it contained types with the dependency

					              Not matching items:
					              [
					                Layer1AndLayer2
					              ]
					              """);
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Test]
			public async Task WhenNoTypeDependsOnTargetCollection_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Types.InNamespace(Layer2Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypeDependsOnTargetCollection_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Types.InNamespace(Layer2Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all do not depend on types within namespace "{Layer2Namespace}" in all loaded assemblies,
					              but it contained types with the dependency

					              Not matching items:
					              [
					                Layer1AndLayer2
					              ]
					              """);
			}

			[Test]
			public async Task WhenTargetCollectionIsEmpty_ShouldSucceedTrivially()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(Layer1AndLayer2),
					typeof(FrameworkConsumer),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Types.InNamespace("Non.Existent.Namespace"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldFailIfAnyMatches()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn(Types.InNamespace(Layer2Namespace))
						.OrOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not depend on types within namespace "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2" in all loaded assemblies or types within namespace "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1" in all loaded assemblies,
					             but it contained types with the dependency

					             Not matching items:
					             [
					               OnlyLayer1
					             ]
					             """);
			}
		}

		public sealed class SpecificTypeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableContainsTypeWithTheDependency_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(OnlyLayer1), typeof(Layer1AndLayer2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotDependOn<TargetB>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not depend on type TargetB,
					             but it contained types with the dependency

					             Not matching items:
					             [
					               Layer1AndLayer2
					             ]
					             """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableTypesDoNotDependOnType_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ViaField), typeof(OnlyLayer1),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotDependOn<TargetB>();
				}

				await That(Act).DoesNotThrow();
			}
#endif

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
					await That(subject).DoNotDependOn<TargetB>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not depend on type TargetB,
					             but it contained types with the dependency

					             Not matching items:
					             [
					               <null>
					             ]
					             """)
					.Because("a null item's dependencies cannot be verified, so it must not slip through as \"does not depend\"");
			}

			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [];

				async Task Act()
				{
					await That(subject).DoNotDependOn<TargetB>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoTypeDependsOnType_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(OnlyLayer1),
					typeof(FrameworkConsumer),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn<TargetB>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOnlySomeTypesDependOnType_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(OnlyLayer2),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn<TargetB>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not depend on type TargetB,
					             but it contained types with the dependency

					             Not matching items:
					             [
					               OnlyLayer2,
					               Layer1AndLayer2
					             ]
					             """)
					.Because("a plural negation means that none of the types depends on the type, not that not all do");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<Type?>? subject = null;

				async Task Act()
				{
					await That(subject).DoNotDependOn<TargetB>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not depend on type TargetB,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldFailIfAnyMatches()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject).DoNotDependOn<TargetB>().OrOn<TargetA>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not depend on type TargetB or TargetA,
					             but it contained types with the dependency

					             Not matching items:
					             [
					               OnlyLayer1
					             ]
					             """);
			}
		}
	}
}
