using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed class QuantifiedCollectionConstraintTests
{
	[Test]
	public async Task WhenAllItemsAreEven_ShouldSucceed()
	{
		int[] values = [2, 4, 6,];

		async Task Act()
			=> await That(values).All().AreEven();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenEvaluatedAgain_ShouldOnlyCountTheItemsOfTheCurrentCollection()
	{
		int[][] values = [[1,], [2,],];

		async Task Act()
			=> await That(values).Exactly(1).ComplyWith(v => v.All().AreEven());

		await That(Act).DoesNotThrow()
			.Because("the constraint is evaluated again for each collection and must not count the previous items");
	}

	[Test]
	public async Task WhenEvaluatedAgainForAnEmptyCollection_ShouldNotCountThePreviousItems()
	{
		int[][] values = [[1,], [],];

		async Task Act()
			=> await That(values).Exactly(1).ComplyWith(v => v.All().AreEven());

		await That(Act).DoesNotThrow()
			.Because("all items of an empty collection are even");
	}

	[Test]
	public async Task WhenNegated_ShouldNegateTheQuantifierAndShowTheMatchingItems()
	{
		int[] values = [2, 4, 6,];

		async Task Act()
			=> await That(values).DoesNotComplyWith(v => v.AtLeast(2).AreEven());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that values
			             is even for fewer than 2 items,
			             but 3 of 3 were

			             Matching items:
			             [2, 4, 6]
			             """)
			.Because("the extension renders the negated quantifier like the built-in Satisfy");
	}

	[Test]
	public async Task WhenNegatedAndNested_ShouldUseTheComplementOfTheQuantifier()
	{
		Dictionary<string, int> values = new()
		{
			["a"] = 2,
			["b"] = 3,
		};

		async Task Act()
			=> await That(values).DoesNotComplyWith(v => v.Values.AtMost(1).AreEven());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that values
			             has values of which more than one is even,
			             but 1 of 2 were

			             Not matching items (values):
			             [3]
			             """)
			.Because("the extension renders the nested negated quantifier like the built-in Satisfy");
	}

	[Test]
	public async Task WhenNested_ShouldUseTheVerbNumberOfTheQuantifier()
	{
		Dictionary<string, int> values = new()
		{
			["a"] = 1,
			["b"] = 2,
			["c"] = 3,
		};

		async Task Act()
			=> await That(values).Values.AtLeast(2).AreEven();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that values
			             has values of which at least 2 are even,
			             but only 1 of 3 were

			             Not matching items (values):
			             [1, 3]
			             """)
			.Because("the extension renders the nested quantifier like the built-in Satisfy");
	}

	[Test]
	public async Task WhenSomeItemsAreOdd_ShouldShowTheNotMatchingItems()
	{
		int[] values = [1, 2, 3,];

		async Task Act()
			=> await That(values).All().AreEven();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that values
			             is even for all items,
			             but only 1 of 3 were

			             Not matching items:
			             [1, 3]
			             """)
			.Because("the extension renders the quantifier like the built-in Satisfy");
	}

	[Test]
	public async Task WhenSubjectIsNull_ShouldFail()
	{
		int[]? values = null;

		async Task Act()
			=> await That(values).DoesNotComplyWith(v => v.All().AreEven());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that values
			             is even not for all items,
			             but it was <null>
			             """)
			.Because("a null subject fails the expectation and its negation alike");
	}
}
