namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class LessThan
	{
		public sealed class NegatedComplyWithTests
		{
			[Fact]
			public async Task WhenLessThanTheMaximumComply_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.LessThan(4).ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for at least 4 items,
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
