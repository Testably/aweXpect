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
	public sealed partial class HaveDependenciesOutside
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtMost_WhenTooManyItemsHaveDependenciesOutsideTheNamespace_ShouldListTheViolations()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2), typeof(Layer1AndLayer2),];

				async Task Act()
				{
					await That(subject).AtMost(1).HaveDependenciesOutside(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}" for at most one item,
					              but 2 of 3 did

					              Matching items:
					              [
					                OnlyLayer2 depends on ["{Layer2Namespace}"],
					                Layer1AndLayer2 depends on ["{Layer2Namespace}"]
					              ]

					              Collection:
					              [
					                OnlyLayer1,
					                OnlyLayer2 depends on ["{Layer2Namespace}"],
					                Layer1AndLayer2 depends on ["{Layer2Namespace}"]
					              ]
					              """);
			}

			[Test]
			public async Task Exactly_WhenOneItemHasDependenciesOutsideTheTargetCollection_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2),];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveDependenciesOutside(In.Type<TargetA>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenOrOnWidensTheNamespaces_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2), typeof(Layer1AndLayer2),];

				async Task Act()
				{
					await That(subject).None().HaveDependenciesOutside(Layer1Namespace).OrOn(Layer2Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(OnlyLayer2), null,];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveDependenciesOutside(Layer1Namespace);
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AtLeast(2).HaveDependenciesOutside(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}" for at least 2 items,
					              but for the item at index 1, it was <null>

					              Collection:
					              [
					                OnlyLayer2 depends on ["{Layer2Namespace}"],
					                <null>
					              ]
					              """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}" for fewer than 2 items,
					              but for the item at index 1, it was <null>

					              Collection:
					              [
					                OnlyLayer2 depends on ["{Layer2Namespace}"],
					                <null>
					              ]
					              """).Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveDependenciesOutsideTheTargetCollection_ShouldFail()
			{
				ImmutableArray<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2),];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveDependenciesOutside(In.Type<TargetA>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies outside in type TargetA for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               OnlyLayer1
					             ]

					             Collection:
					             [
					               OnlyLayer1,
					               OnlyLayer2 depends on ["TargetB"]
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenOneItemHasDependenciesOutsideTheNamespace_ShouldSucceed()
			{
				ImmutableArray<Type?> subject = [typeof(OnlyLayer1), typeof(OnlyLayer2),];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveDependenciesOutside(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveDependenciesOutsideTheNamespace_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(OnlyLayer1), typeof(OnlyLayer2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveDependenciesOutside(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has dependencies outside namespace "{Layer1Namespace}" for at least 2 items,
					              but only 1 of 2 did

					              Not matching items:
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
			public async Task Exactly_WhenOneItemHasDependenciesOutsideTheTargetCollection_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(OnlyLayer1), typeof(OnlyLayer2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).Exactly(1).HaveDependenciesOutside(In.Type<TargetA>());
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
