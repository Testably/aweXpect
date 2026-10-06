namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Between
	{
		public sealed class ComplyWithTests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int[]? subject = null;

				async Task Act()
					=> await That(subject).Between(1).And(2).ComplyWith(it => it.IsEqualTo(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 3 for between 1 and 2 items,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedComplyWithTests
		{
			[Test]
			public async Task WhenTheNumberOfComplyingItemsIsInRange_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.Between(2).And(4).ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for not between 2 and 4 items,
					             but 3 of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
