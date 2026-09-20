namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class None
	{
		public sealed class ComplyWithTests
		{
			[Fact]
			public async Task WhenItemsUseWhose_ShouldIncludeMemberInExpectation()
			{
				MyClass[] subject = [new(1), new(2),];

				async Task Act()
					=> await That(subject).None().ComplyWith(x => x.Whose(o => o.Value, v => v.IsEqualTo(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Value is equal to 1 for no items,
					             but 1 of 2 were

					             Matching items:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
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
		}

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
