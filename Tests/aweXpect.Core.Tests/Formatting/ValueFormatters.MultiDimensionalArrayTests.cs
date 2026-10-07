using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class MultiDimensionalArrayTests
	{
		[Test]
		public async Task InFailureMessage_ShouldListTheItemsOfEachDimensionInTheCollectionContext()
		{
			IEnumerable subject = new[,]
			{
				{
					1, 2,
				},
				{
					3, 4,
				},
			};

			async Task Act()
				=> await That(subject).Contains(9);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 9 at least once,
				             but it did not contain it

				             Collection:
				             [[1, 2], [3, 4]]
				             """);
		}

		[Test]
		public async Task ShouldCountAnEmptyDimensionAsOneItemOfTheMaximum()
		{
			int[,] value = new int[12, 0];

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[[], [], [], [], [], [], [], [], [], [], (… and 2 more)]")
				.Because("an array without items must not be written without a limit");
		}

		[Test]
		public async Task ShouldFormatEachDimensionAsANestedCollection()
		{
			string expectedResult = "[[1, 2], [3, 4]]";
			int[,] value =
			{
				{
					1, 2,
				},
				{
					3, 4,
				},
			};
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, (IEnumerable)value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task ShouldFormatTheItemsLikeTheItemsOfAnyOtherCollection()
		{
			object?[,] value =
			{
				{
					"a", null,
				},
				{
					'b', new List<int>
					{
						1,
						2,
					},
				},
			};

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[[\"a\", <null>], ['b', [1, 2]]]");
		}

		[Test]
		public async Task ShouldLimitTheItemsOfTheWholeArray()
		{
			int[,] value =
			{
				{
					1, 2, 3, 4,
				},
				{
					5, 6, 7, 8,
				},
				{
					9, 10, 11, 12,
				},
			};

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[[1, 2, 3, 4], [5, 6, 7, 8], [9, 10, (… and 2 more)]]")
				.Because("the maximum counts the items across all dimensions, and the remaining ones are named once");
		}

		[Test]
		public async Task ShouldNotLimitAnArrayWithExactlyTheMaximumNumberOfItems()
		{
			int[,] value =
			{
				{
					1, 2, 3, 4, 5,
				},
				{
					6, 7, 8, 9, 10,
				},
			};

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[[1, 2, 3, 4, 5], [6, 7, 8, 9, 10]]");
		}

		[Test]
		public async Task ShouldTellArraysWithTheSameItemsInDifferentDimensionsApart()
		{
			int[,] row =
			{
				{
					1, 2, 3, 4,
				},
			};
			int[,] column =
			{
				{
					1,
				},
				{
					2,
				},
				{
					3,
				},
				{
					4,
				},
			};
			int[] oneDimensional = [1, 2, 3, 4,];

			string rowResult = Formatter.Format(row);
			string columnResult = Formatter.Format(column);
			string oneDimensionalResult = Formatter.Format((object)oneDimensional);

			await That(rowResult).IsEqualTo("[[1, 2, 3, 4]]");
			await That(columnResult).IsEqualTo("[[1], [2], [3], [4]]");
			await That(oneDimensionalResult).IsEqualTo("[1, 2, 3, 4]");
		}

		[Test]
		public async Task WhenADimensionIsEmpty_ShouldFormatAnEmptyCollectionInEachItemOfTheOuterDimensions()
		{
			string emptyFirstDimensionResult = Formatter.Format(new int[0, 2]);
			string emptyLastDimensionResult = Formatter.Format(new int[2, 0]);
			string threeDimensionalResult = Formatter.Format(new int[1, 2, 0]);

			await That(emptyFirstDimensionResult).IsEqualTo("[]")
				.Because("the lengths of the dimensions inside an empty dimension cannot be shown");
			await That(emptyLastDimensionResult).IsEqualTo("[[], []]");
			await That(threeDimensionalResult).IsEqualTo("[[[], []]]");
		}

		[Test]
		public async Task WhenArrayContainsItself_ShouldDetectTheRecursion()
		{
			object[,] value = new object[1, 2];
			value[0, 0] = value;
			value[0, 1] = 1;

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[[[ *recursive* ], 1]]");
		}

		[Test]
		public async Task WhenFormattingAnItemThrows_ShouldRenderThePlaceholderLikeInAJaggedArray()
		{
			ThrowingToString item = new();
			ThrowingToString[,] value =
			{
				{
					item,
				},
			};
			ThrowingToString[][] jagged =
			[
				[item,],
			];

			string result = Formatter.Format(value);

			await That(result).IsEqualTo(Formatter.Format(jagged));
			await That(result).Contains("did throw an InvalidOperationException");
		}

		[Test]
		[NotInParallel(nameof(ValueFormatterTests))]
		public async Task WhenItemFormatterIsRegistered_ShouldUseItForTheItems()
		{
			int[,] value =
			{
				{
					1, 2,
				},
			};
			string result;
			using (ValueFormatter.Register(new CurrentFlowFormatter()))
			{
				result = Formatter.Format(value);
			}

			await That(result).IsEqualTo("[[custom, custom]]");
		}

		[Test]
		public async Task WhenNestedDeeperThanTheMaximumDepth_ShouldLeaveOutTheItemsOfTheArray()
		{
			List<object> value = [];
			List<object> current = value;
			for (int i = 0; i < 19; i++)
			{
				List<object> inner = [];
				current.Add(inner);
				current = inner;
			}

			current.Add(new[,]
			{
				{
					1, 2,
				},
			});
			string expectedResult = new string('[', 20) + "[ … ]" + new string(']', 20);

			string result = Formatter.Format(value);

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenSameInstanceIsContainedTwice_ShouldFormatBoth()
		{
			int[,] item =
			{
				{
					1, 2,
				},
			};
			object[] value = [item, item,];

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[[[1, 2]], [[1, 2]]]");
		}

		[Test]
		public async Task WhenTheMaximumIsReachedAtTheEndOfADimension_ShouldNameTheRemainingItemsInTheOuterDimension()
		{
			int[,] value = new int[4, 5];
			for (int i = 0; i < 20; i++)
			{
				value[i / 5, i % 5] = i + 1;
			}

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[[1, 2, 3, 4, 5], [6, 7, 8, 9, 10], (… and 10 more)]");
		}

		[Test]
		public async Task WithIndentation_ShouldFormatLikeAJaggedArray()
		{
			int[,] value =
			{
				{
					1, 2,
				},
				{
					3, 4,
				},
			};
			int[][] jagged =
			[
				[1, 2,],
				[3, 4,],
			];

			string result = Formatter.Format(value, FormattingOptions.Indented());

			await That(result).IsEqualTo(Formatter.Format(jagged, FormattingOptions.Indented()));
		}

		[Test]
		public async Task WithLineBreaks_ShouldFormatEachDimensionOnSeparateLines()
		{
			int[,] value =
			{
				{
					1, 2,
				},
				{
					3, 4,
				},
			};
			int[][] jagged =
			[
				[1, 2,],
				[3, 4,],
			];

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo("""
			                             [
			                               [
			                                 1,
			                                 2
			                               ],
			                               [
			                                 3,
			                                 4
			                               ]
			                             ]
			                             """);
			await That(result).IsEqualTo(Formatter.Format(jagged, FormattingOptions.MultipleLines))
				.Because("it is formatted like the jagged array with the same items");
		}

		[Test]
		public async Task WithLineBreaks_ShouldNameTheRemainingItemsOnTheLineOfTheNextItem()
		{
			int[,] value =
			{
				{
					1, 2, 3, 4, 5, 6,
				},
				{
					7, 8, 9, 10, 11, 12,
				},
			};

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo("""
			                             [
			                               [
			                                 1,
			                                 2,
			                                 3,
			                                 4,
			                                 5,
			                                 6
			                               ],
			                               [
			                                 7,
			                                 8,
			                                 9,
			                                 10,
			                                 (… and 2 more)
			                               ]
			                             ]
			                             """);
		}

		[Test]
		public async Task WithRankThree_ShouldNestACollectionPerDimension()
		{
			int[,,] value =
			{
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				},
				{
					{
						5, 6,
					},
					{
						7, 8,
					},
				},
			};

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[[[1, 2], [3, 4]], [[5, 6], [7, 8]]]");
		}

		[Test]
		public async Task WithType_ShouldIncludeTheTypeOnce()
		{
			string expectedResult = "int[,] [[1, 2], [3, 4]]";
			int[,] value =
			{
				{
					1, 2,
				},
				{
					3, 4,
				},
			};
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			Formatter.Format(sb, (IEnumerable)value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		/// <remarks>
		///     Registrations are process-wide, so it only formats values in the test that created it and leaves the
		///     tests running in parallel alone.
		/// </remarks>
		private sealed class CurrentFlowFormatter : IValueFormatter
		{
			private readonly AsyncLocal<bool> _isCurrentFlow = new()
			{
				Value = true,
			};

			public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
			{
				if (_isCurrentFlow.Value && value is int)
				{
					stringBuilder.Append("custom");
					return true;
				}

				return false;
			}
		}

		private sealed class ThrowingToString
		{
			public override string ToString()
				=> throw new InvalidOperationException("no text");
		}
	}
}
