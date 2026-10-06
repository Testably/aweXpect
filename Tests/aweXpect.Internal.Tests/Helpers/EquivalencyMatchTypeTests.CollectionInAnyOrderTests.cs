using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Equivalency;

namespace aweXpect.Internal.Tests.Helpers;

public sealed partial class EquivalencyMatchTypeTests
{
	public sealed class CollectionInAnyOrderTests
	{
		[Test]
		public async Task ArrayAndListWithSameValues_ShouldBeConsideredEqual()
		{
			int[] actual = [3, 2, 4, 1, 5,];
			List<int> expected = [1, 4, 3, 2, 5,];
			EquivalencyMatchType sut = new(new EquivalencyOptions().IgnoringCollectionOrder());

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Test]
		public async Task ListAndArrayWithSameValues_ShouldBeConsideredEqual()
		{
			List<int> actual = [1, 5, 3, 4, 2,];
			int[] expected = [5, 4, 3, 2, 1,];
			EquivalencyMatchType sut = new(new EquivalencyOptions().IgnoringCollectionOrder());

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Test]
		public async Task WhenActualHasFewerValues_ShouldNotBeConsideredEqual()
		{
			int[] actual = [1, 4, 3, 2,];
			List<int> expected = [1, 5, 3, 4, 2,];
			EquivalencyMatchType sut = new(new EquivalencyOptions().IgnoringCollectionOrder());

			IObjectMatchResult explanation = await sut.AreConsideredEqualWithExplanation(actual, expected);
			bool result = explanation.IsMatch;
			string failure = explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                Element [1] was missing 5
			                              """)
				.Because("the index is the position of the missing value in the expected collection");
		}

		[Test]
		public async Task WhenActualHasMoreValues_ShouldNotBeConsideredEqual()
		{
			int[] actual = [1, 5, 4, 3, 2,];
			List<int> expected = [1, 3, 2, 4,];
			EquivalencyMatchType sut = new(new EquivalencyOptions().IgnoringCollectionOrder());

			IObjectMatchResult explanation = await sut.AreConsideredEqualWithExplanation(actual, expected);
			bool result = explanation.IsMatch;
			string failure = explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                Element [1] had superfluous 5
			                              """);
		}

		[Test]
		[Arguments(true)]
		[Arguments(false)]
		public async Task WhenCollectionsDifferInOrder_ShouldBeConsideredEqualWhenCollectionOrderIsIgnored(
			bool ignoreCollectionOrder)
		{
			int[] actual = [1, 2, 3, 4, 5,];
			List<int> expected = [1, 5, 2, 3, 4,];
			EquivalencyMatchType sut = new(new EquivalencyOptions().IgnoringCollectionOrder(ignoreCollectionOrder));

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(ignoreCollectionOrder);
		}
	}
}
