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
			public async Task WhenItemsUseWhose_ShouldIncludeMemberInExpectation()
			{
				MyClass[] subject = [new(1), new(2),];

				async Task Act()
					=> await That(subject).AtLeast(2).ComplyWith(x => x.Whose(o => o.Value, v => v.IsEqualTo(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Value is equal to 1 for at least 2 items,
					             but only 1 of 2 did

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 2
					               }
					             ]
					             """)
					.Because("the member text must survive the node tree rendering");
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
					             but none of 5 were

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
					             is greater than 2 for fewer than 2 items,
					             but 3 of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
