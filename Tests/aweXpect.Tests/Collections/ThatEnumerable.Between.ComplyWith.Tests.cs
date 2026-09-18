namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Between
	{
		public sealed class NegatedComplyWithTests
		{
			[Fact]
			public async Task WhenTheNumberOfComplyingItemsIsInRange_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.Between(2).And(4).ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for not between 2 and 4 items,
					             but found 3

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
