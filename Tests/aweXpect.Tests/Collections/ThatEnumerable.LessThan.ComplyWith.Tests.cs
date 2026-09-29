namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class LessThan
	{
		public sealed class ComplyWithTests
		{
			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int[]? subject = null;

				async Task Act()
					=> await That(subject).LessThan(2).ComplyWith(it => it.IsEqualTo(3));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 3 for fewer than 2 items,
					             but it was <null>
					             """);
			}
		}

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

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
