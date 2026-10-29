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
					              none depend on namespace "{Layer2Namespace}",
					              but for the item at index 1, it was <null>

					              Collection:
					              [
					                OnlyLayer1,
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
					              none depend on namespace "{Layer2Namespace}",
					              but 1 of 2 did

					              Matching items:
					              [
					                Layer1AndLayer2
					              ]

					              Collection:
					              [
					                OnlyLayer1,
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
					              none depend on types within namespace "{Layer2Namespace}" in all loaded assemblies,
					              but 1 of 2 did

					              Matching items:
					              [
					                Layer1AndLayer2
					              ]

					              Collection:
					              [
					                OnlyLayer1,
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
					             none depend on types within namespace "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2" in all loaded assemblies or types within namespace "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1" in all loaded assemblies,
					             but 1 of 1 did

					             Matching items:
					             [
					               OnlyLayer1
					             ]

					             Collection:
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
					             none depend on type TargetB,
					             but 1 of 2 did

					             Matching items:
					             [
					               Layer1AndLayer2
					             ]

					             Collection:
					             [
					               OnlyLayer1,
					               Layer1AndLayer2
					             ]
					             """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableContainsTypeWithTheRuntimeTypeDependency_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(OnlyLayer1), typeof(Layer1AndLayer2),
				}.ToTestAsyncEnumerable<Type?>();
				Type type = typeof(TargetB);

				async Task Act()
				{
					await That(subject).DoNotDependOn(type);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none depend on type TargetB,
					             but 1 of 2 did

					             Matching items:
					             [
					               Layer1AndLayer2
					             ]

					             Collection:
					             [
					               OnlyLayer1,
					               Layer1AndLayer2
					             ]
					             """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableTypesDoNotDependOnRuntimeType_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ViaField), typeof(OnlyLayer1),
				}.ToTestAsyncEnumerable<Type?>();
				Type type = typeof(TargetB);

				async Task Act()
				{
					await That(subject).DoNotDependOn(type);
				}

				await That(Act).DoesNotThrow();
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
					             none depend on type TargetB,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               OnlyLayer1,
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
			public async Task WhenCollectionWithNullDoesNotDependOnRuntimeType_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					null,
				];
				Type type = typeof(TargetB);

				async Task Act()
				{
					await That(subject).DoNotDependOn(type);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none depend on type TargetB,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               OnlyLayer1,
					               <null>
					             ]
					             """)
					.Because("a null item's dependencies cannot be verified, so it must not slip through as \"does not depend\"");
			}

			[Test]
			public async Task WhenNegatedAndNoTypeDependsOnRuntimeType_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(OnlyLayer1),
				];
				Type type = typeof(TargetB);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotDependOn(type));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one depends on type TargetB,
					             but none of 2 did

					             Collection:
					             [
					               ViaField,
					               OnlyLayer1
					             ]
					             """);
			}

			[Test]
			public async Task WhenNoTypeDependsOnRuntimeType_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(OnlyLayer1),
					typeof(FrameworkConsumer),
				];
				Type type = typeof(TargetB);

				async Task Act()
				{
					await That(subject).DoNotDependOn(type);
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
			public async Task WhenNullSubjectDoesNotDependOnRuntimeType_ShouldFail()
			{
				IEnumerable<Type?>? subject = null;
				Type type = typeof(TargetB);

				async Task Act()
				{
					await That(subject).DoNotDependOn(type);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none depend on type TargetB,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenOnlySomeTypesDependOnRuntimeType_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(OnlyLayer2),
					typeof(Layer1AndLayer2),
				];
				Type type = typeof(TargetB);

				async Task Act()
				{
					await That(subject).DoNotDependOn(type);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none depend on type TargetB,
					             but 2 of 3 did

					             Matching items:
					             [
					               OnlyLayer2,
					               Layer1AndLayer2
					             ]

					             Collection:
					             [
					               OnlyLayer1,
					               OnlyLayer2,
					               Layer1AndLayer2
					             ]
					             """);
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
					             none depend on type TargetB,
					             but 2 of 3 did

					             Matching items:
					             [
					               OnlyLayer2,
					               Layer1AndLayer2
					             ]

					             Collection:
					             [
					               OnlyLayer1,
					               OnlyLayer2,
					               Layer1AndLayer2
					             ]
					             """)
					.Because("a plural negation means that none of the types depends on the type, not that not all do");
			}

			[Test]
			public async Task WhenRuntimeTypeIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1),];

				async Task Act()
				{
					await That(subject).DoNotDependOn((Type)null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The 'type' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenRuntimeTypeIsWidenedWithOrOn_ShouldFailIfAnyMatches()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
				];
				Type type = typeof(TargetB);
				Type additionalType = typeof(TargetA);

				async Task Act()
				{
					await That(subject).DoNotDependOn(type).OrOn(additionalType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none depend on type TargetB or TargetA,
					             but 1 of 1 did

					             Matching items:
					             [
					               OnlyLayer1
					             ]

					             Collection:
					             [
					               OnlyLayer1
					             ]
					             """);
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
					             none depend on type TargetB,
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
					             none depend on type TargetB or TargetA,
					             but 1 of 1 did

					             Matching items:
					             [
					               OnlyLayer1
					             ]

					             Collection:
					             [
					               OnlyLayer1
					             ]
					             """);
			}
		}
	}
}
