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
	public sealed class DependOn
	{
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";

		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesDependOnNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(ViaProperty),
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject).DependOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoNamespaceIsSpecified_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1),];

				async Task Act()
				{
					await That(subject).DependOn();
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("namespaces").And
					.WithMessage("The 'namespaces' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenSomeTypeDoesNotDependOnNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(FrameworkConsumer),
				];

				async Task Act()
				{
					await That(subject).DependOn(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all depend on namespace "{Layer1Namespace}",
					              but it contained types without the dependency

					              Not matching items:
					              [
					                FrameworkConsumer
					              ]
					              """);
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
					await That(subject).DependOn("Non.Existent.Namespace").OrOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Test]
			public async Task WhenAllTypesDependOnTargetCollection_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(ViaProperty),
					typeof(OnlyLayer1),
				];

				async Task Act()
				{
					await That(subject).DependOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypeDoesNotDependOnTargetCollection_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(FrameworkConsumer),
				];

				async Task Act()
				{
					await That(subject).DependOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              all depend on types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it contained types without the dependency

					              Not matching items:
					              [
					                FrameworkConsumer
					              ]
					              """);
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
					await That(subject).DependOn(Types.InNamespace("Non.Existent.Namespace"))
						.OrOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class SpecificTypeTests
		{
			[Test]
			public async Task WhenAllTypesDependOnRuntimeType_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(ViaProperty),
					typeof(Layer1AndLayer2),
				];
				Type type = typeof(TargetA);

				async Task Act()
				{
					await That(subject).DependOn(type);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllTypesDependOnType_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(ViaProperty),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).DependOn<TargetA>();
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableContainsTypeWithoutTheDependency_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ViaField), typeof(OnlyLayer2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DependOn<TargetA>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all depend on type TargetA,
					             but it contained types without the dependency

					             Not matching items:
					             [
					               OnlyLayer2
					             ]
					             """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableContainsTypeWithoutTheRuntimeTypeDependency_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ViaField), typeof(OnlyLayer2),
				}.ToTestAsyncEnumerable<Type?>();
				Type type = typeof(TargetA);

				async Task Act()
				{
					await That(subject).DependOn(type);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all depend on type TargetA,
					             but it contained types without the dependency

					             Not matching items:
					             [
					               OnlyLayer2
					             ]
					             """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableTypesDependOnRuntimeType_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ViaField), typeof(Layer1AndLayer2),
				}.ToTestAsyncEnumerable<Type?>();
				Type type = typeof(TargetA);

				async Task Act()
				{
					await That(subject).DependOn(type);
				}

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableTypesDependOnType_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ViaField), typeof(Layer1AndLayer2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DependOn<TargetA>();
				}

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			public async Task WhenCollectionContainsNull_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					null,
				];

				async Task Act()
				{
					await That(subject).DependOn<TargetA>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all depend on type TargetA,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ViaField,
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [];

				async Task Act()
				{
					await That(subject).DependOn<TargetA>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCollectionWithNullDependsOnRuntimeType_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					null,
				];
				Type type = typeof(TargetA);

				async Task Act()
				{
					await That(subject).DependOn(type);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all depend on type TargetA,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ViaField,
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenNegatedAndAllTypesDependOnRuntimeType_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(Layer1AndLayer2),
				];
				Type type = typeof(TargetA);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DependOn(type));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all depend on type TargetA,
					             but it only contained types with the dependency

					             Collection:
					             [
					               ViaField,
					               Layer1AndLayer2
					             ]
					             """);
			}

			[Test]
			public async Task WhenNegatedAndAllTypesDependOnType_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(Layer1AndLayer2),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DependOn<TargetA>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all depend on type TargetA,
					             but it only contained types with the dependency

					             Collection:
					             [
					               ViaField,
					               Layer1AndLayer2
					             ]
					             """);
			}

			[Test]
			public async Task WhenNullSubjectDependsOnRuntimeType_ShouldFail()
			{
				IEnumerable<Type?>? subject = null;
				Type type = typeof(TargetA);

				async Task Act()
				{
					await That(subject).DependOn(type);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all depend on type TargetA,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenRuntimeTypeIsGenericTypeDefinition_ShouldMatchAnyConstruction()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaGenericArgument),
				];

				async Task Act()
				{
					await That(subject).DependOn(typeof(List<>));
				}

				await That(Act).DoesNotThrow()
					.Because("ViaGenericArgument references List<TargetA>, a construction of List<>");
			}

			[Test]
			public async Task WhenRuntimeTypeIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<Type?> subject = [typeof(ViaField),];

				async Task Act()
				{
					await That(subject).DependOn((Type)null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The 'type' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenRuntimeTypeIsWidenedWithOrOn_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(OnlyLayer2),
				];
				Type type = typeof(TargetA);
				Type additionalType = typeof(TargetB);

				async Task Act()
				{
					await That(subject).DependOn(type).OrOn(additionalType);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypesDoNotDependOnRuntimeType_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(OnlyLayer2),
					typeof(FrameworkConsumer),
				];
				Type type = typeof(TargetA);

				async Task Act()
				{
					await That(subject).DependOn(type);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all depend on type TargetA,
					             but it contained types without the dependency

					             Not matching items:
					             [
					               OnlyLayer2,
					               FrameworkConsumer
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeTypesDoNotDependOnType_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ViaField),
					typeof(OnlyLayer2),
					typeof(FrameworkConsumer),
				];

				async Task Act()
				{
					await That(subject).DependOn<TargetA>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all depend on type TargetA,
					             but it contained types without the dependency

					             Not matching items:
					             [
					               OnlyLayer2,
					               FrameworkConsumer
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<Type?>? subject = null;

				async Task Act()
				{
					await That(subject).DependOn<TargetA>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all depend on type TargetA,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(OnlyLayer1),
					typeof(OnlyLayer2),
				];

				async Task Act()
				{
					await That(subject).DependOn<TargetA>().OrOn<TargetB>();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
