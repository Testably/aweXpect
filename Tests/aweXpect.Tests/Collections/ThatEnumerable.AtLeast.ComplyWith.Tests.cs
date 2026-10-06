namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class AtLeast
	{
		public sealed class ComplyWithTests
		{
			[Test]
			public async Task WhenAtLeastOneItemMatches_ShouldSucceed()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).AtLeast(1).ComplyWith(it => it.IsEqualTo(3));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsUseWhose_ShouldIncludeMemberInExpectation()
			{
				MyClass[] subject = [new(1), new(2),];

				async Task Act()
					=> await That(subject).AtLeast(2).ComplyWith(x => x.Whose(o => o.Value, v => v.IsEqualTo(1)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose Value is equal to 1 for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 2
					               }
					             ]

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

			[Test]
			public async Task WhenNoItemsMatch_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).AtLeast(1).ComplyWith(it => it.IsEqualTo(99));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 99 for at least one item,
					             but none of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int[]? subject = null;

				async Task Act()
					=> await That(subject).AtLeast(1).ComplyWith(it => it.IsEqualTo(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 3 for at least one item,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedComplyWithTests
		{
			[Test]
			public async Task WhenAtLeastTheMinimumComplies_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.AtLeast(2).ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for fewer than 2 items,
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
