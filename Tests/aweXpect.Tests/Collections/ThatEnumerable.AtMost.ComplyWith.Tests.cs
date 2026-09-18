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
					             is greater than 2 for not at most 3 items,
					             but found 3

					             Matching items:
					             [3, 4, 5]

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
