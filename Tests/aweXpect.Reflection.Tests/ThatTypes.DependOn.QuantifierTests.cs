using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class DependOn
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenEnoughItemsDependOnTheNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2), typeof(Layer1AndLayer2),];

				async Task Act()
				{
					await That(subject).AtLeast(2).DependOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtMost_WhenTooManyItemsDependOnTheType_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2), typeof(Layer1AndLayer2),];

				async Task Act()
				{
					await That(subject).AtMost(1).DependOn<TargetB>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             depends on type TargetB for at most one item,
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
			public async Task Exactly_WhenOrOnWidensTheTargetCollections_ShouldCountTheWidenedTargets()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2), typeof(FrameworkConsumer),];

				async Task Act()
				{
					await That(subject).Exactly(2).DependOn(Types.InNamespace("Non.Existent.Namespace"))
						.OrOn(Types.InNamespace(Layer1Namespace), In.Type<TargetB>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemDependsOnTheNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer2), typeof(Layer1AndLayer2),];

				async Task Act()
				{
					await That(subject).None().DependOn(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              depends on namespace "{Layer1Namespace}" for no items,
					              but 1 of 2 did

					              Matching items:
					              [
					                Layer1AndLayer2
					              ]

					              Collection:
					              [
					                OnlyLayer2,
					                Layer1AndLayer2
					              ]
					              """);
			}

			[Test]
			public async Task None_WhenOrOnAddsANamespaceOfAnItem_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer2), typeof(FrameworkConsumer),];
				Type type = typeof(TargetA);

				async Task Act()
				{
					await That(subject).None().DependOn(type).OrOn<TargetB>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             depends on type TargetA or TargetB for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               OnlyLayer2
					             ]

					             Collection:
					             [
					               OnlyLayer2,
					               FrameworkConsumer
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [null, typeof(OnlyLayer1),];

				async Task Act()
				{
					await That(subject).AtLeast(1).DependOn<TargetA>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AtLeast(1).DependOn<TargetA>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             depends on type TargetA for at least one item,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               OnlyLayer1
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             depends on type TargetA for no items,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               OnlyLayer1
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNegated_ShouldFailWhenTheQuantifierIsMet()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2),];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AtLeast(1).DependOn(Types.InNamespace(Layer1Namespace)));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              depends on types within namespace "{Layer1Namespace}" in all loaded assemblies for no items,
					              but 1 of 2 did

					              Matching items:
					              [
					                OnlyLayer1
					              ]

					              Collection:
					              [
					                OnlyLayer1,
					                OnlyLayer2
					              ]
					              """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsDependOnTheType_ShouldFail()
			{
				ImmutableArray<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2),];

				async Task Act()
				{
					await That(subject).AtLeast(2).DependOn(typeof(TargetA));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             depends on type TargetA for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               OnlyLayer2
					             ]

					             Collection:
					             [
					               OnlyLayer1,
					               OnlyLayer2
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenOneItemDependsOnTheTargetCollection_ShouldSucceed()
			{
				ImmutableArray<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2),];

				async Task Act()
				{
					await That(subject).Exactly(1).DependOn(In.Type<TargetB>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenNoItemDependsOnTheNamespace_ShouldSucceed()
			{
				ImmutableArray<Type?> subject = [typeof(OnlyLayer2), typeof(FrameworkConsumer),];

				async Task Act()
				{
					await That(subject).None().DependOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsDependOnTheType_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(OnlyLayer1), typeof(OnlyLayer2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).DependOn<TargetA>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             depends on type TargetA for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               OnlyLayer2
					             ]

					             Collection:
					             [
					               OnlyLayer1,
					               OnlyLayer2
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenOneItemDependsOnTheTargetCollection_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(OnlyLayer1), typeof(OnlyLayer2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).Exactly(1).DependOn(In.Type<TargetB>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenNoItemDependsOnTheNamespace_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(OnlyLayer2), typeof(FrameworkConsumer),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).None().DependOn(Layer1Namespace, "Non.Existent.Namespace");
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
