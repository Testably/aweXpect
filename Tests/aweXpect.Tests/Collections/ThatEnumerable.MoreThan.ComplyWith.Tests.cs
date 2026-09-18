namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class MoreThan
	{
		public sealed class NegatedComplyWithTests
		{
			[Fact]
			public async Task WhenMoreThanTheMinimumComply_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MoreThan(2).ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for not more than 2 items,
					             but found 3

					             Not matching items:
					             [1, 2]

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
