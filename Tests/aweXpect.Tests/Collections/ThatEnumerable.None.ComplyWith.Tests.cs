namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class None
	{
		public sealed class NegatedComplyWithTests
		{
			[Fact]
			public async Task WhenNoItemComplies_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.None().ComplyWith(x => x.IsGreaterThan(5)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 5 for at least one item,
					             but none of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
