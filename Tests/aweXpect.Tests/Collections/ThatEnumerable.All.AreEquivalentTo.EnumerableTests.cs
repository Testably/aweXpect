using System.Collections;
using System.Linq;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEquivalentTo
		{
			public sealed class EnumerableTests
			{
				[Test]
				public async Task DoesNotEnumerateTwice()
				{
					IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(1)
							.And.All().AreEquivalentTo(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(1, o => o.IncludingFields());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to 1 for all items,
						             but only 2 of at least 3 were

						             Not matching items:
						             [2, (… and maybe more)]

						             Collection:
						             [1, 1, 2, (… and maybe more)]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Test]
				public async Task Using_ShouldThrowInvalidOperationException()
				{
					IEnumerable subject = Factory.GetFibonacciNumbers(20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(5).Using(new AllEqualComparer());

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with Equivalent.");
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					IEnumerable subject = Factory.GetFibonacciNumbers(20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(5);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to 5 for all items,
						             but only 1 of 20 were

						             Not matching items:
						             [
						               1,
						               1,
						               2,
						               3,
						               8,
						               13,
						               21,
						               34,
						               55,
						               89,
						               (… and 9 more)
						             ]

						             Collection:
						             [
						               1,
						               1,
						               2,
						               3,
						               5,
						               8,
						               13,
						               21,
						               34,
						               55,
						               (… and 10 more)
						             ]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Test]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					IEnumerable subject = Factory.GetConstantValueEnumerable(constantValue, 20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(constantValue);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					int constantValue = 42;
					IEnumerable? subject = null;

					async Task Act()
						=> await That(subject)!.All().AreEquivalentTo(constantValue);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to constantValue for all items,
						             but it was <null>

						             Equivalency options:
						              - include public fields and properties
						             """);
				}
			}

			public sealed class EnumerableNegatedTests
			{
				[Test]
				public async Task WhenAllItemsMatch_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 1, 1,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreEquivalentTo(1));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to 1 not for all items,
						             but all 3 were

						             Collection:
						             [1, 1, 1]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Test]
				public async Task WhenOneItemDoesNotMatch_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 2, 1,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreEquivalentTo(1));

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
