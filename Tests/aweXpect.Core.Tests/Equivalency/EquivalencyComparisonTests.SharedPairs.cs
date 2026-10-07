using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed partial class EquivalencyComparisonTests
{
	public enum SharedShape
	{
		Members,
		List,
		Dictionary,
	}

	[Test]
	public async Task WhenSharedPairDiffers_ShouldReportItForEveryPath()
	{
		ReadBudget budget = new(1_000);
		SharedNode actual = SharedNode.Chain(budget, 2, SharedShape.Members);
		SharedNode expected = SharedNode.Chain(budget, 2, SharedShape.Members);
		expected.Left!.Left!.Value = 1;
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Left.Left.Value differed:
		                                                      Actual: 0
		                                                    Expected: 1
		                                                and
		                                                  Property Left.Right.Value differed:
		                                                      Actual: 0
		                                                    Expected: 1
		                                                and
		                                                  Property Right.Left.Value differed:
		                                                      Actual: 0
		                                                    Expected: 1
		                                                and
		                                                  Property Right.Right.Value differed:
		                                                      Actual: 0
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSharedPairHasAMemberToIgnoreOnOnePath_ShouldReportTheOtherPath()
	{
		Heavy actualShared = new()
		{
			Value = 1,
		};
		Heavy expectedShared = new()
		{
			Value = 2,
		};
		var actual = new
		{
			A = actualShared,
			B = actualShared,
		};
		var expected = new
		{
			A = expectedShared,
			B = expectedShared,
		};
		EquivalencyOptions options = new()
		{
			MembersToIgnore = [new MemberToIgnore.ByName("A.Value"),],
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("the pair is only equivalent on the path where its differing member is ignored");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property B.Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSharedPairHasANestedMemberToIgnoreOnOnePath_ShouldReportTheOtherPath()
	{
		Heavy actualShared = new()
		{
			Detail = new Light(1),
		};
		Heavy expectedShared = new()
		{
			Detail = new Light(2),
		};
		var actual = new
		{
			A = actualShared,
			B = actualShared,
		};
		var expected = new
		{
			A = expectedShared,
			B = expectedShared,
		};
		EquivalencyOptions options = new EquivalencyOptions().For<Light>(o => o with
		{
			MembersToIgnore = [new MemberToIgnore.ByName("A.Detail.Value"),],
		});
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("the members to ignore of a nested type make the enclosing pair depend on its path as well");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property B.Detail.Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSharedPairIsComparedAgainAfterItChanged_ShouldCompareItAgain()
	{
		Heavy actualShared = new();
		Heavy expectedShared = new();
		var actual = new
		{
			A = actualShared,
			B = actualShared,
		};
		var expected = new
		{
			A = expectedShared,
			B = expectedShared,
		};
		EquivalencyOptions options = new();
		StringBuilder failureBuilder = new();

		bool resultBefore = await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());
		actualShared.Value = 1;
		bool resultAfter = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(resultBefore).IsTrue();
		await That(resultAfter).IsFalse()
			.Because("a pair is only remembered within one comparison");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property A.Value differed:
		                                                      Actual: 1
		                                                    Expected: 0
		                                                and
		                                                  Property B.Value differed:
		                                                      Actual: 1
		                                                    Expected: 0
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSharedPairIsComparedInOrderAfterItWasComparedInAnyOrder_ShouldCompareItAgain()
	{
		Heavy[] actualShared =
		[
			new()
			{
				Value = 1,
			},
			new()
			{
				Value = 2,
			},
		];
		Heavy[] expectedShared =
		[
			new()
			{
				Value = 2,
			},
			new()
			{
				Value = 1,
			},
		];
		var actual = new
		{
			A = new WithItemsInAnyOrder(actualShared),
			B = actualShared,
		};
		var expected = new
		{
			A = new WithItemsInAnyOrder(expectedShared),
			B = expectedShared,
		};
		EquivalencyOptions options = new EquivalencyOptions().For<WithItemsInAnyOrder>(o => o with
		{
			IgnoreCollectionOrder = true,
		});
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("the arrays are only equivalent where their order is ignored");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property B[0].Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                and
		                                                  Property B[1].Value differed:
		                                                      Actual: 2
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSharedPairIsComparedWithMoreMembers_ShouldCompareItAgain()
	{
		Heavy actualShared = new()
		{
			Field = 1,
		};
		Heavy expectedShared = new()
		{
			Field = 2,
		};
		var actual = new
		{
			A = new
			{
				Item = actualShared,
			},
			B = new WithFields(actualShared),
		};
		var expected = new
		{
			A = new
			{
				Item = expectedShared,
			},
			B = new WithFields(expectedShared),
		};
		EquivalencyOptions options = new EquivalencyOptions
		{
			Fields = IncludeMembers.None,
		}.For<WithFields>(o => o with
		{
			Fields = IncludeMembers.Public,
		});
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("the pair is only equivalent where its fields are not compared");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field B.Item.Field differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSharedPairIsNestedDeeper_ShouldStillExceedTheRecursionLimit()
	{
		Heavy actualShared = new()
		{
			Chain = new NestedNode(4),
		};
		Heavy expectedShared = new()
		{
			Chain = new NestedNode(4),
		};
		var actual = new
		{
			A = actualShared,
			B = new
			{
				Nested = actualShared,
			},
		};
		var expected = new
		{
			A = expectedShared,
			B = new
			{
				Nested = expectedShared,
			},
		};
		EquivalencyOptions options = new()
		{
			MaxRecursionDepth = 6,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("the pair was found equivalent one level higher, where its chain just fits into the limit");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property B.Nested.Chain.Inner.Inner.Inner exceeded the maximum recursion depth of 6
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSharedPairIsNestedLessDeep_ShouldSucceed()
	{
		Heavy actualShared = new()
		{
			Chain = new NestedNode(4),
		};
		Heavy expectedShared = new()
		{
			Chain = new NestedNode(4),
		};
		var actual = new
		{
			A = new
			{
				Nested = actualShared,
			},
			B = actualShared,
		};
		var expected = new
		{
			A = new
			{
				Nested = expectedShared,
			},
			B = expectedShared,
		};
		EquivalencyOptions options = new()
		{
			MaxRecursionDepth = 7,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSharedPairIsSharedAtDifferentDepths_ShouldNotCompareItOncePerPath()
	{
		ReadBudget budget = new(1_000_000);
		SharedNode actual = SharedNode.SkippingChain(budget, 60);
		SharedNode expected = SharedNode.SkippingChain(budget, 60);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a pair that is reached deeper than before is compared again, which is still far from once per path");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	[Arguments(SharedShape.Members, false)]
	[Arguments(SharedShape.Members, true)]
	[Arguments(SharedShape.List, false)]
	[Arguments(SharedShape.List, true)]
	[Arguments(SharedShape.Dictionary, false)]
	public async Task WhenSharedPairIsSharedOnEveryLevel_ShouldNotCompareItOncePerPath(SharedShape shape,
		bool ignoreCollectionOrder)
	{
		ReadBudget budget = new(100_000);
		SharedNode actual = SharedNode.Chain(budget, 40, shape);
		SharedNode expected = SharedNode.Chain(budget, 40, shape);
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = ignoreCollectionOrder,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);
		bool isEquivalent = await EquivalencyComparison.IsEquivalent(actual, expected, options);

		await That(result).IsTrue()
			.Because("every level doubles the number of paths, so that there are 2^40 of them");
		await That(isEquivalent).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSharedPairOnlyRecursesIntoItself_ShouldNotCompareItOncePerPath()
	{
		ReadBudget budget = new(100_000);
		SharedNode actual = SharedNode.Chain(budget, 40, SharedShape.Members, node => node.Reference = node);
		SharedNode expected = SharedNode.Chain(budget, 40, SharedShape.Members, node => node.Reference = node);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue()
			.Because("a pair that only recurses into itself does not depend on a pair that encloses it");
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	public async Task WhenSharedPairRecursesIntoADifferingPair_ShouldReportItForEveryPath()
	{
		Heavy actualOuter = new()
		{
			Value = 1,
			Inner = new Heavy(),
		};
		actualOuter.Inner.Outer = actualOuter;
		Heavy expectedOuter = new()
		{
			Value = 2,
			Inner = new Heavy(),
		};
		expectedOuter.Inner.Outer = expectedOuter;
		var actual = new
		{
			A = actualOuter,
			B = actualOuter.Inner,
		};
		var expected = new
		{
			A = expectedOuter,
			B = expectedOuter.Inner,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property A.Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                and
		                                                  Property B.Outer.Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle()
			.Because("the inner pair only had no difference of its own while the outer pair was compared, as its reference to that pair was skipped as a recursion");
	}

	[Test]
	public async Task WhenSharedPairRecursesIntoAnElementThatIsNotMatched_ShouldNotBeEquivalentForAnotherElement()
	{
		Heavy actualOne = new()
		{
			Value = 1,
			Inner = new Heavy(),
		};
		actualOne.Inner.Outer = actualOne;
		Heavy actualTwo = new()
		{
			Value = 2,
			Inner = new Heavy(),
		};
		actualTwo.Inner.Outer = actualTwo;
		Heavy expectedInner = new();
		Heavy expectedOne = new()
		{
			Value = 1,
			Inner = expectedInner,
		};
		Heavy expectedTwo = new()
		{
			Value = 2,
			Inner = expectedInner,
		};
		expectedInner.Outer = expectedTwo;
		Heavy[] actual = [actualOne, actualTwo,];
		Heavy[] expected = [expectedTwo, expectedOne,];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = true,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);
		bool isEquivalent = await EquivalencyComparison.IsEquivalent(actual, expected, options);

		await That(result).IsFalse()
			.Because("the inner pair only had no difference while the first actual element was compared with the second expected one, which differ, as its reference to that pair was skipped as a recursion");
		await That(isEquivalent).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property [0].Inner.Outer.Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSharedPairsReferenceTheRoot_ShouldSucceed()
	{
		ReadBudget budget = new(10_000_000);
		List<SharedNode> actualNodes = [];
		SharedNode actual = SharedNode.Chain(budget, 10, SharedShape.Members, actualNodes.Add);
		actualNodes.ForEach(node => node.Reference = actual);
		List<SharedNode> expectedNodes = [];
		SharedNode expected = SharedNode.Chain(budget, 10, SharedShape.Members, expectedNodes.Add);
		expectedNodes.ForEach(node => node.Reference = expected);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsTrue();
		await That(failureBuilder.ToString()).IsEmpty();
	}

	[Test]
	[Arguments(false)]
	[Arguments(true)]
	public async Task WhenSharedPairsShareTheActualElementOnly_ShouldCompareBothExpectedElements(
		bool ignoreCollectionOrder)
	{
		Heavy actualShared = new()
		{
			Value = 1,
		};
		Heavy[] actual = [actualShared, actualShared,];
		Heavy[] expected =
		[
			new()
			{
				Value = 1,
			},
			new()
			{
				Value = 2,
			},
		];
		EquivalencyOptions options = new()
		{
			IgnoreCollectionOrder = ignoreCollectionOrder,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse()
			.Because("the shared element is only equivalent to the first of its two counterparts");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property [1].Value differed:
		                                                      Actual: 1
		                                                    Expected: 2
		                                                """).IgnoringNewlineStyle();
	}

	[Test]
	public async Task WhenSharedPairsShareTheExpectedInstanceOnly_ShouldCompareBothActualInstances()
	{
		Heavy expectedShared = new()
		{
			Value = 1,
		};
		var actual = new
		{
			A = new Heavy
			{
				Value = 1,
			},
			B = new Heavy
			{
				Value = 2,
			},
		};
		var expected = new
		{
			A = expectedShared,
			B = expectedShared,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse()
			.Because("the actual instances are told apart by reference, although their Equals considers them the same");
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property B.Value differed:
		                                                      Actual: 2
		                                                    Expected: 1
		                                                """).IgnoringNewlineStyle();
	}

	/// <remarks>
	///     Has enough nested objects to be remembered once it was found equivalent, and an <see cref="Equals" /> that
	///     considers every instance the same.
	/// </remarks>
	private sealed class Heavy
	{
		public int Field;
		public WithProperty[] Ballast { get; } = Enumerable.Range(0, 100).Select(i => new WithProperty(i)).ToArray();
		public NestedNode? Chain { get; set; }
		public Light? Detail { get; set; }
		public Heavy? Inner { get; set; }
		public Heavy? Outer { get; set; }
		public int Value { get; set; }

		public override bool Equals(object? obj) => obj is Heavy;

		public override int GetHashCode() => 0;
	}

	private sealed class Light(int value)
	{
		public int Value { get; } = value;
	}

	/// <summary>
	///     Counts how often the members of the <see cref="SharedNode" />s are read, and throws when the
	///     <paramref name="limit" /> is exceeded, so that a comparison that walks every path fails instead of running
	///     for days.
	/// </summary>
	private sealed class ReadBudget(int limit)
	{
		private int _reads;

		public T Read<T>(T value)
		{
			if (++_reads > limit)
			{
				throw new InvalidOperationException($"The members were read more than {limit} times.");
			}

			return value;
		}
	}

	/// <remarks>
	///     Its <see cref="Equals" /> and <see cref="GetHashCode" /> throw, as the comparison has to tell the instances
	///     apart by reference.
	/// </remarks>
	private sealed class SharedNode(ReadBudget budget)
	{
		private Dictionary<string, SharedNode>? _entries;
		private List<SharedNode>? _items;
		private SharedNode? _left;
		private SharedNode? _right;

		public Dictionary<string, SharedNode>? Entries => budget.Read(_entries);
		public List<SharedNode>? Items => budget.Read(_items);
		public SharedNode? Left => budget.Read(_left);
		public SharedNode? Reference { get; set; }
		public SharedNode? Right => budget.Read(_right);
		public int Value { get; set; }

		/// <summary>
		///     A chain of <paramref name="depth" /> levels, each of which references the next one twice, so that
		///     every level doubles the number of paths to the innermost node.
		/// </summary>
		public static SharedNode Chain(ReadBudget budget, int depth, SharedShape shape,
			Action<SharedNode>? configure = null)
		{
			SharedNode node = new(budget);
			configure?.Invoke(node);
			for (int i = 0; i < depth; i++)
			{
				node = new SharedNode(budget).Referencing(node, node, shape);
				configure?.Invoke(node);
			}

			return node;
		}

		/// <summary>
		///     A chain of <paramref name="depth" /> levels, each of which references the level after the next one
		///     before the next one, so that a node is first reached on the shortest path to it.
		/// </summary>
		public static SharedNode SkippingChain(ReadBudget budget, int depth)
		{
			SharedNode afterNext = new(budget);
			SharedNode next = new(budget);
			for (int i = 0; i < depth; i++)
			{
				(afterNext, next) = (next, new SharedNode(budget).Referencing(afterNext, next, SharedShape.Members));
			}

			return next;
		}

		public override bool Equals(object? obj) => throw new NotSupportedException();

		public override int GetHashCode() => throw new NotSupportedException();

		private SharedNode Referencing(SharedNode first, SharedNode second, SharedShape shape)
		{
			switch (shape)
			{
				case SharedShape.List:
					_items = [first, second,];
					break;
				case SharedShape.Dictionary:
					_entries = new Dictionary<string, SharedNode>
					{
						["first"] = first,
						["second"] = second,
					};
					break;
				default:
					_left = first;
					_right = second;
					break;
			}

			return this;
		}
	}

	private sealed class WithFields(Heavy item)
	{
		public Heavy Item { get; } = item;
	}

	private sealed class WithItemsInAnyOrder(Heavy[] items)
	{
		public Heavy[] Items { get; } = items;
	}
}
