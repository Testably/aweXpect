namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Exactly
	{
		public sealed class ComplyWithTests
		{
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
					             but found 2

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

				await That(Act).Throws<XunitException>();
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
					             but found 1

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
					             but found 3

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
