using System.Collections.Generic;
using System.Text;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed partial class EquivalencyComparisonTests
{
	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndAnElementOfAMultiDimensionalArrayIsIgnored_ShouldIgnoreItInBothArrays()
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		int[,] expected =
		{
			{
				4, 3,
			},
			{
				99, 1,
			},
		};
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
			MembersToIgnore = [new MemberToIgnore.ByPredicate((path, _) => path == "[1,0]", "index 1,0"),],
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("the ignored elements at [1,0] leave the actual 1, 2, 4 and the expected 4, 3, 1");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [0,1] differed:
		                                                      Actual: 2
		                                                    Expected: 3
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndMultiDimensionalArraysHaveDifferentDimensions_ShouldReportTheDimensions()
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		int[,] expected =
		{
			{
				4, 3, 2, 1,
			},
		};
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse().Because("only the order of the items is ignored");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It had dimensions [2,2] instead of [1,4]
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndMultiDimensionalArraysHaveDifferentItems_ShouldReportTheIndexOfEachDimension()
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		int[,] expected =
		{
			{
				4, 2,
			},
			{
				1, 5,
			},
		};
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1,0] differed:
		                                                      Actual: 3
		                                                    Expected: 5
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenCollectionOrderIsIgnored_AndMultiDimensionalArraysHaveTheSameItems_ShouldMatchThemAtAnyPosition()
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		int[,] expected =
		{
			{
				4, 2,
			},
			{
				1, 3,
			},
		};
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenJaggedArrayElementDiffers_ShouldReportTheNestedIndices()
	{
		int[][] actual = [[1, 2,], [3, 4,],];
		int[][] expected = [[1, 2,], [5, 4,],];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1][0] differed:
		                                                      Actual: 3
		                                                    Expected: 5
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenJaggedArrayIsComparedWithAMultiDimensionalArray_ShouldReportTheRank()
	{
		int[][] actual = [[1, 2,], [3, 4,],];
		int[,] expected =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It had rank 1 instead of 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenJaggedArraysHaveRowsOfDifferentLengths_ShouldCompareThemAsNestedArrays()
	{
		int[][] actual = [[1,], [2, 3,],];
		int[][] expected = [[1,], [2, 3,],];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenMultiDimensionalArrayElementDiffers_ShouldReportTheIndexOfEachDimension()
	{
		var actual = new
		{
			Values = new[,]
			{
				{
					1, 2, 3,
				},
				{
					4, 5, 6,
				},
			},
		};
		var expected = new
		{
			Values = new[,]
			{
				{
					1, 2, 3,
				},
				{
					7, 5, 8,
				},
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element Values[1,0] differed:
		                                                      Actual: 4
		                                                    Expected: 7
		                                                and
		                                                  Element Values[1,2] differed:
		                                                      Actual: 6
		                                                    Expected: 8
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenMultiDimensionalArrayElementIsIgnored_ShouldMatchItByTheIndexOfEachDimension()
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		int[,] expected =
		{
			{
				1, 2,
			},
			{
				3, 99,
			},
		};
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByPredicate((path, _) => path == "[1,1]", "index 1,1"),],
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenMultiDimensionalArrayHasThreeDimensions_ShouldReportTheIndexOfEachDimension()
	{
		int[,,] actual = new int[2, 3, 4];
		int[,,] expected = new int[2, 3, 4];
		expected[1, 2, 3] = 1;
		expected[1, 0, 2] = 2;
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1,0,2] differed:
		                                                      Actual: 0
		                                                    Expected: 2
		                                                and
		                                                  Element [1,2,3] differed:
		                                                      Actual: 0
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenMultiDimensionalArrayIsAnItemOfACollection_ShouldAppendItsIndicesToThePath()
	{
		List<string[,]> actual =
		[
			new[,]
			{
				{
					"a", "b",
				},
			},
			new[,]
			{
				{
					"c", "d",
				},
			},
		];
		List<string[,]> expected =
		[
			new[,]
			{
				{
					"a", "b",
				},
			},
			new[,]
			{
				{
					"c", "x",
				},
			},
		];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Element [1][0,1] differed:
		                                                      Actual: "d"
		                                                    Expected: "x"
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenMultiDimensionalArrayIsComparedWithAList_ShouldReportTheRank()
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		List<int> expected = [1, 2, 3, 4,];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It had rank 2 instead of 1
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenMultiDimensionalArrayIsComparedWithAOneDimensionalArray_ShouldReportTheRank()
	{
		var actual = new
		{
			Values = new[,]
			{
				{
					1, 2,
				},
				{
					3, 4,
				},
			},
		};
		var expected = new
		{
			Values = new[]
			{
				1, 2, 3, 4,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Values had rank 2 instead of 1
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenMultiDimensionalArraysAreEquivalent_ShouldSucceed()
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		int[,] expected =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenMultiDimensionalArraysAreEquivalent_WithIsNotEquivalentTo_ShouldFail()
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		int[,] unexpected =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};

		async Task Act()
			=> await That(actual).IsNotEquivalentTo(unexpected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is not equivalent to unexpected,
			             but it was [
			                 [
			                   1,
			                   2
			                 ],
			                 [
			                   3,
			                   4
			                 ]
			               ], which is considered equivalent

			             Equivalency options:
			              - include public fields and properties
			             """);
	}

	[Test]
	[Arguments(1, 4)]
	[Arguments(4, 1)]
	public async Task WhenMultiDimensionalArraysHaveDifferentDimensions_ShouldReportTheDimensions(
		int rows, int columns)
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		int[,] expected = new int[rows, columns];
		int value = 1;
		for (int row = 0; row < rows; row++)
		{
			for (int column = 0; column < columns; column++)
			{
				expected[row, column] = value++;
			}
		}

		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo($"""

		                                                   It had dimensions [2,2] instead of [{rows},{columns}]
		                                                 """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenMultiDimensionalArraysHaveDifferentDimensions_WithIsEquivalentTo_ShouldFail()
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		int[,] expected =
		{
			{
				1, 2, 3, 4,
			},
		};

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that actual
			             is equivalent to expected,
			             but it was not:
			               It had dimensions [2,2] instead of [1,4]

			             Equivalency options:
			              - include public fields and properties
			             """);
	}

	[Test]
	public async Task WhenMultiDimensionalArraysHaveDifferentDimensions_WithIsNotEquivalentTo_ShouldSucceed()
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		int[,] unexpected =
		{
			{
				1, 2, 3, 4,
			},
		};

		async Task Act()
			=> await That(actual).IsNotEquivalentTo(unexpected);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenMultiDimensionalArraysHaveDifferentRanks_ShouldReportTheRank()
	{
		int[,] actual =
		{
			{
				1, 2,
			},
			{
				3, 4,
			},
		};
		int[,,] expected =
		{
			{
				{
					1, 2,
				},
				{
					3, 4,
				},
			},
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It had rank 2 instead of 3
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenMultiDimensionalArraysHaveTheSameEmptyDimensions_ShouldSucceed()
	{
		int[,] actual = new int[0, 2];
		int[,] expected = new int[0, 2];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenMultiDimensionalArraysWithoutItemsHaveDifferentDimensions_ShouldReportTheDimensions()
	{
		int[,] actual = new int[0, 2];
		int[,] expected = new int[2, 0];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It had dimensions [0,2] instead of [2,0]
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenMultiDimensionalArrayWithoutItemsIsComparedWithAnEmptyArray_ShouldReportTheRank()
	{
		int[] actual = [];
		int[,] expected = new int[0, 2];
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  It had rank 1 instead of 2
		                                                """).IgnoringNewlineStyle();
	}
}
