using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Between
	{
		public sealed class AreUnique
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenNumberOfUniqueItemsIsInRange_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).Between(1).And(2).AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).Between(1).And(2).AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for between 1 and 2 items,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenTooManyItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3, 4,]);

					async Task Act()
						=> await That(subject).Between(1).And(2).AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for between 1 and 2 items,
						             but 3 of 5 were

						             Collection:
						             [1, 1, 2, 3, 4]
						             """);
				}
			}

			public sealed class AreNotUniqueTests
			{
				[Test]
				public async Task WhenNumberOfDuplicateItemsIsInRange_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).Between(1).And(2).AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTooFewItemsAreNotUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).Between(1).And(2).AreNotUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for between 1 and 2 items,
						             but none of 3 were

						             Collection:
						             [1, 2, 3]
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenNumberOfUniqueItemsIsInRange_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Between(1).And(2).AreUnique());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for not between 1 and 2 items,
						             but 2 of 4 were

						             Collection:
						             [1, 1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenTooManyItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3, 4,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Between(1).And(2).AreUnique());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
