namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class AtMost
	{
		public sealed class NegatedComplyWithTests
		{
			[Fact]
			public async Task WhenAtMostTheMaximumComplies_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.AtMost(3).ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for more than 3 items,
					             but 3 of 5 were

					             Matching items:
					             [3, 4, 5]

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
