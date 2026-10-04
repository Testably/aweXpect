namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Exactly
	{
		public sealed class ComplyWithTests
		{
			[Fact]
			public async Task WhenAnEarlierAttemptStoppedAtAnItem_ShouldOnlyCountTheItemsOfTheLastAttempt()
			{
				int calls = 0;
				Func<int?[]> subject = () => calls++ == 0 ? [1, 2, null,] : [1, 2, 3,];

				async Task Act()
					=> await That(subject).Eventually().Within(5.Seconds()).CheckEvery(10.Milliseconds())
						.Exactly(3).ComplyWith(it => it.Satisfies(x => x!.Value > 0));

				await That(Act).DoesNotThrow();
				await That(calls).IsEqualTo(2)
					.Because("the items that the first attempt recorded before it stopped do not count for the second one");
			}

			[Fact]
			public async Task WhenExactlyOneItemMatches_ShouldSucceed()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).Exactly(1).ComplyWith(it => it.IsEqualTo(3));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMoreItemsMatchThanExpected_ShouldFail()
			{
				int[] subject = [1, 2, 3, 2, 5,];

				async Task Act()
					=> await That(subject).Exactly(1).ComplyWith(it => it.IsEqualTo(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 for exactly one item,
					             but 2 of 5 were

					             Collection:
					             [1, 2, 3, 2, 5]
					             """);
			}

			[Fact]
			public async Task WhenNoItemsMatch_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).Exactly(1).ComplyWith(it => it.IsEqualTo(99));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 99 for exactly one item,
					             but none of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}

			[Fact]
			public async Task WhenRichExpectationMatchesExactCount_ShouldSucceed()
			{
				int[] subject = [5, 10, 15, 20, 25, 30, 35,];

				async Task Act()
					=> await That(subject).Exactly(3)
						.ComplyWith(it => it.IsGreaterThan(10).And.IsLessThan(30));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int[]? subject = null;

				async Task Act()
					=> await That(subject).Exactly(1).ComplyWith(it => it.IsEqualTo(3));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 3 for exactly one item,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedComplyWithTests
		{
			[Fact]
			public async Task WhenExactlyOneItemMatches_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.Exactly(1).ComplyWith(x => x.IsEqualTo(3)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 3 for not exactly one item,
					             but 1 of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}

			[Fact]
			public async Task WhenExactlyTheExpectedNumberComplies_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.Exactly(3).ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for not exactly 3 items,
					             but 3 of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
