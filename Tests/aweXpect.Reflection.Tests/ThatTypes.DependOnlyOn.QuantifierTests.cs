using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class DependOnlyOn
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenEnoughItemsDependOnlyOnTheNamespace_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1), typeof(ViaField), typeof(Layer1AndLayer2),];

				async Task Act()
				{
					await That(subject).AtLeast(2).DependOnlyOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsDependOnlyOnTheTargetCollection_ShouldListTheViolations()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1), typeof(Layer1AndLayer2),];

				async Task Act()
				{
					await That(subject).AtLeast(2).DependOnlyOn(In.Type<TargetA>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             depends only on in type TargetA for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               Layer1AndLayer2 depends on ["TargetB"]
					             ]

					             Collection:
					             [
					               OnlyLayer1,
					               Layer1AndLayer2 depends on ["TargetB"]
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenOrOnWidensTheNamespaces_ShouldCountTheWidenedNamespaces()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2), typeof(Layer1AndLayer2),];

				async Task Act()
				{
					await That(subject).Exactly(3).DependOnlyOn(Layer1Namespace).OrOn(Layer2Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemDependsOnlyOnTheNamespace_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2),];

				async Task Act()
				{
					await That(subject).None().DependOnlyOn(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on namespace "{Layer1Namespace}" for no items,
					              but 1 of 2 did

					              Matching items:
					              [
					                OnlyLayer1
					              ]

					              Collection:
					              [
					                OnlyLayer1,
					                OnlyLayer2 depends on ["{Layer2Namespace}"]
					              ]
					              """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [null, typeof(OnlyLayer1),];

				async Task Act()
				{
					await That(subject).AtLeast(1).DependOnlyOn(Layer1Namespace);
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AtLeast(1).DependOnlyOn(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on namespace "{Layer1Namespace}" for at least one item,
					              but for the item at index 0, it was <null>

					              Collection:
					              [
					                <null>,
					                OnlyLayer1
					              ]
					              """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on namespace "{Layer1Namespace}" for no items,
					              but for the item at index 0, it was <null>

					              Collection:
					              [
					                <null>,
					                OnlyLayer1
					              ]
					              """).Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsDependOnlyOnTheNamespace_ShouldFail()
			{
				ImmutableArray<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2),];

				async Task Act()
				{
					await That(subject).AtLeast(2).DependOnlyOn(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              depends only on namespace "{Layer1Namespace}" for at least 2 items,
					              but only 1 of 2 did

					              Not matching items:
					              [
					                OnlyLayer2 depends on ["{Layer2Namespace}"]
					              ]

					              Collection:
					              [
					                OnlyLayer1,
					                OnlyLayer2 depends on ["{Layer2Namespace}"]
					              ]
					              """);
			}

			[Test]
			public async Task Exactly_WhenOneItemDependsOnlyOnTheTargetCollection_ShouldSucceed()
			{
				ImmutableArray<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2),];

				async Task Act()
				{
					await That(subject).Exactly(1).DependOnlyOn(In.Type<TargetA>());
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsDependOnlyOnTheTargetCollection_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(OnlyLayer1), typeof(OnlyLayer2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).DependOnlyOn(In.Type<TargetA>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             depends only on in type TargetA for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               OnlyLayer2 depends on ["TargetB"]
					             ]

					             Collection:
					             [
					               OnlyLayer1,
					               OnlyLayer2 depends on ["TargetB"]
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenOneItemDependsOnlyOnTheNamespace_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(OnlyLayer1), typeof(OnlyLayer2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).Exactly(1).DependOnlyOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
