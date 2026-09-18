namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class AtLeast
	{
		public sealed class ComplyWithTests
		{
			[Fact]
			public async Task WhenAtLeastOneItemMatches_ShouldSucceed()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).AtLeast(1).ComplyWith(it => it.IsEqualTo(3));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoItemsMatch_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).AtLeast(1).ComplyWith(it => it.IsEqualTo(99));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 99 for at least one item,
					             but found only 0

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}

		public sealed class NegatedComplyWithTests
		{
			[Fact]
			public async Task WhenAtLeastTheMinimumComplies_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.AtLeast(2).ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for not at least 2 items,
					             but found 3

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
