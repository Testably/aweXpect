using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Equivalency;

namespace aweXpect.Internal.Tests.Helpers;

public sealed partial class EquivalencyComparerTests
{
	public sealed class CollectionTests
	{
		[Fact]
		public async Task ArrayAndListWithSameValues_ShouldBeConsideredEqual()
		{
			int[] actual = [1, 2, 3, 4, 5,];
			List<int> expected = [1, 2, 3, 4, 5,];
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Fact]
		public async Task ListAndArrayWithSameValues_ShouldBeConsideredEqual()
		{
			List<int> actual = [1, 2, 3, 4, 5,];
			int[] expected = [1, 2, 3, 4, 5,];
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Fact]
		public async Task WhenActualHasFewerValues_ShouldNotBeConsideredEqual()
		{
			int[] actual = [1, 2, 3, 4,];
			List<int> expected = [1, 2, 3, 4, 5,];
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                Element [4] was missing 5
			                              """);
		}

		[Fact]
		public async Task WhenActualHasMoreValues_ShouldNotBeConsideredEqual()
		{
			int[] actual = [1, 2, 3, 4, 5,];
			List<int> expected = [1, 2, 3, 4,];
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                Element [4] had superfluous 5
			                              """);
		}

		[Fact]
		public async Task WhenCollectionsDifferInOrder_ShouldNotBeConsideredEqual()
		{
			int[] actual = [1, 2, 3, 4, 5,];
			List<int> expected = [1, 5, 2, 3, 4,];
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                Element [1] differed:
			                                    Actual: 2
			                                  Expected: 5
			                              and
			                                Element [2] differed:
			                                    Actual: 3
			                                  Expected: 2
			                              and
			                                Element [3] differed:
			                                    Actual: 4
			                                  Expected: 3
			                              and
			                                Element [4] differed:
			                                    Actual: 5
			                                  Expected: 4
			                              """);
		}
	}
}
