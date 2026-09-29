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
				[Fact]
				public async Task WhenNumberOfUniqueItemsIsInRange_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).Between(1).And(2).AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).Between(1).And(2).AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for between 1 and 2 items,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenTooManyItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3, 4,]);

					async Task Act()
						=> await That(subject).Between(1).And(2).AreUnique();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task WhenNumberOfDuplicateItemsIsInRange_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).Between(1).And(2).AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTooFewItemsAreNotUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).Between(1).And(2).AreNotUnique();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task WhenNumberOfUniqueItemsIsInRange_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Between(1).And(2).AreUnique());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for not between 1 and 2 items,
						             but 2 of 4 were

						             Collection:
						             [1, 1, 2, 3]
						             """);
				}

				[Fact]
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
