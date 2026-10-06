namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class AtMost
	{
		public sealed class ComplyWithTests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int[]? subject = null;

				async Task Act()
					=> await That(subject).AtMost(1).ComplyWith(it => it.IsEqualTo(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 3 for at most one item,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedComplyWithTests
		{
			[Test]
			public async Task WhenAtMostTheMaximumComplies_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.AtMost(3).ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for more than 3 items,
					             but 3 of 5 were

					             Not matching items:
					             [1, 2]

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
