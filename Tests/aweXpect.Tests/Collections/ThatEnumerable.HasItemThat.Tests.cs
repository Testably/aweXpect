using System.Collections.Generic;
using System.Linq;
using System.Threading;
using aweXpect.Core;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItemThat
	{
		public sealed class Tests
		{
			[Test]
			public async Task AllowsNestedIs()
			{
				List<Base> subject =
				[
					new Derived
					{
						Name = "foo",
					},
				];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.Is<Derived>());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(int.MinValue))
						.And.HasItemThat(it => it.IsNotEqualTo(int.MinValue)).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(5));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAnEarlierAttemptHadAnItemAtTheIndex_ShouldDescribeTheLastAttempt()
			{
				int calls = 0;
				Func<int[]> subject = () => calls++ == 0 ? [2,] : [];

				async Task Act()
					=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
						.HasItemThat(x => x.IsEqualTo(1)).AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             eventually has an item that is equal to 1 at index 0 within 0:05,
					             but it had no item at index 0

					             Collection:
					             []
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenCancellationLeavesAnItemOfAnImmutableArrayUndecided_ShouldReportTheEvaluationAsCanceled()
			{
				using CancellationTokenSource cts = new();
				ImmutableArray<IEnumerable<int>> subject = [GetCancellingEnumerable(1, cts, 4), [1, 2,],];

				async Task Act()
					=> await That(subject).HasItemThat(x => x.HasCount(3)).WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             has an item that has exactly 3 items,
					             but it could not be verified, because the evaluation was already canceled

					             Collection:
					             [
					               [
					                 0,
					                 1,
					                 2,
					                 3
					               ],
					               [
					                 1,
					                 2
					               ]
					             ]
					             """)
					.Because("the first item was canceled before its count was known");
			}
#endif

			[Test]
			public async Task WhenCancellationLeavesAnItemUndecided_ShouldReportTheEvaluationAsCanceled()
			{
				using CancellationTokenSource cts = new();
				IEnumerable<int>[] subject = [GetCancellingEnumerable(1, cts, 4), [1, 2,],];

				async Task Act()
					=> await That(subject).HasItemThat(x => x.HasCount(3)).WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             has an item that has exactly 3 items,
					             but it could not be verified, because the evaluation was already canceled

					             Collection:
					             [
					               [
					                 0,
					                 1,
					                 2,
					                 3
					               ],
					               [
					                 1,
					                 2
					               ]
					             ]
					             """)
					.Because("the first item was canceled before its count was known");
			}

			[Test]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(1)).AtIndex(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has an item that is equal to 1 at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldNotReadFurther()
			{
				int readItems = 0;
				IEnumerable<int> subject = new[] { 2, 3, 4, }.Select(x =>
				{
					readItems++;
					return x;
				});

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(1)).AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is equal to 1 at index 0,
					             but it had item 2 at index 0

					             Collection:
					             [2, (… and maybe more)]
					             """);
				await That(readItems).IsEqualTo(1)
					.Because("the item at index 0 decides the outcome, so no further item must be read");
			}

			[Test]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(2)).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
			{
				List<int> subject =
				[
					0,
					1,
					2
				];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(3)).AtIndex(3);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has an item that is equal to 3 at index 3,
					              but it had no item at index 3

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenEnumerableContainsNoMatchingItem_ShouldFail()
			{
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(0));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is equal to 0,
					             but it had no matching item

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				List<int> subject = [];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is not equal to 0,
					             but it had no item

					             Collection:
					             []
					             """);
			}

			[Test]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasItemThat(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenItemAtIndexDoesNotComply_ShouldShowTheContextsOfTheItem()
			{
				int[][] subject = [[1, 2,], [1, 3,],];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo([1, 2,])).AtIndex(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is equal to collection [1, 2,] in order at index 1,
					             but it had item [1, 3] at index 1

					             Collection:
					             [
					               [
					                 1,
					                 2
					               ],
					               [
					                 1,
					                 3
					               ]
					             ]

					             Collection (item [1]):
					             [1, 3]

					             Expected (item [1]):
					             [1, 2]
					             """)
					.Because("exactly the item at the index decides the outcome");
			}

			[Test]
			public async Task WhenItemsDoNotComplyWithAndEnumerableIsEmpty_ShouldNegateExpectation()
			{
				List<int> subject = [];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.DoesNotComplyWith(x => x.IsEqualTo(1).Or.IsEqualTo(2)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is not equal to 1 and is not equal to 2,
					             but it had no item

					             Collection:
					             []
					             """);
			}

			[Test]
			public async Task WhenItemsHaveAsyncReasonAndEnumerableIsEmpty_ShouldIncludeReasonInExpectation()
			{
				List<int> subject = [];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(1).Because(Task.FromResult<string?>("of reasons")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is equal to 1, because of reasons,
					             but it had no item

					             Collection:
					             []
					             """);
			}

			[Test]
			public async Task WhenItemsHaveReasonAndIndex_ShouldIncludeReasonAfterTheIndex()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(1).Because("of reasons")).AtIndex(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has an item that is equal to 1 at index 2, because of reasons,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenItemsUseNestedWhose_ShouldRevertToWhoseForTheInnerMember()
			{
				MyClass[] subject = [new(1, "foo"),];

				async Task Act()
					=> await That(subject).HasItemThat(it
						=> it.Whose(o => o.StringValue, s => s.Whose(v => v.Length, l => l.IsEqualTo(5))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that has StringValue whose Length is equal to 5,
					             but it had no matching item

					             Collection:
					             [
					               MyClass {
					                 StringValue = "foo",
					                 Value = 1
					               }
					             ]
					             """)
					.Because("the outer member introduces its own subject again");
			}

			[Test]
			public async Task WhenItemsUseWhose_ShouldIncludeMemberInExpectation()
			{
				MyClass[] subject = [new(1),];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.Whose(o => o.Value, v => v.IsEqualTo(5)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that has Value that is equal to 5,
					             but it had no matching item

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               }
					             ]
					             """)
					.Because("the member text must survive the node tree rendering");
			}

			[Test]
			public async Task WhenItemsUseWhoseAfterAWhichMember_ShouldKeepWhose()
			{
				Exception[] subject = [new("a", new InvalidOperationException("b")),];

				async Task Act()
					=> await That(subject).HasItemThat(it
						=> it.HasInner<InvalidOperationException>(i => i.Whose(e => e.Message, m => m.IsEqualTo("x"))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that has an inner InvalidOperationException whose Message is equal to "x",
					             but it had no matching item

					             Collection:
					             [
					               Exception: a
					             ]
					             """)
					.Because("a member separated by \"which\" introduces the subject again and can drop the \"which\"");
			}

			[Test]
			public async Task WhenItemsUseWhoseWithAsyncMember_ShouldIncludeMemberInExpectation()
			{
				MyClass[] subject = [new(1),];

				async Task Act()
					=> await That(subject)
						.HasItemThat(it => it.Whose(o => Task.FromResult(o.Value), v => v.IsEqualTo(5)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that has Task.FromResult(o.Value) that is equal to 5,
					             but it had no matching item

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               }
					             ]
					             """)
					.Because("an async member follows the same rule as a synchronous one");
			}

			[Test]
			public async Task WhenNestedItemsDoNotComplyWithAndInnerEnumerableIsEmpty_ShouldNegateExpectation()
			{
				List<int[][]> subject = [[],];

				async Task Act()
					=> await That(subject).HasItemThat(it
						=> it.Any().ComplyWith(x => x.HasItemThat(y => y.DoesNotComplyWith(z => z.IsEqualTo(1).Or.IsEqualTo(2)))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that has an item that is not equal to 1 and is not equal to 2 for at least one item,
					             but it had no matching item

					             Collection:
					             [
					               []
					             ]
					             """);
			}

			[Test]
			public async Task WhenNoItemComplies_ShouldNotShowTheContextsOfAnItem()
			{
				int[][] subject = [[1, 3,], [1, 4,],];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo([1, 2,]));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is equal to collection [1, 2,] in order,
					             but it had no matching item

					             Collection:
					             [
					               [
					                 1,
					                 3
					               ],
					               [
					                 1,
					                 4
					               ]
					             ]
					             """)
					.Because("no single item decides the outcome when any item could match");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is not equal to 0,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0)).AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is not equal to 0 at index 0,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				IEnumerable<int> subject = [0, 1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(2)).WithInvalidMatch();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is equal to 2 with invalid match,
					             but it had no item with invalid match

					             Collection:
					             [0, 1, 2, 3, 4]
					             """);
			}

			[Test]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).HasItemThat(x => x.StartsWith("a").And.EndsWith("b")).AtIndex(0).And
						.HasItemThat(x => x.Contains("c").IgnoringCase()).AtIndex(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that starts with "a" and ends with "b" at index 0 and has an item that contains "c" ignoring case at least once at index 1,
					             but it had item "a" at index 0 and had item "b" at index 1

					             Collection:
					             [
					               "a",
					               "b",
					               (… and maybe more)
					             ]
					             """);
			}
		}

		public sealed class FromEndTests
		{
			[Test]
			[AutoArguments]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail(
				List<int> values, int expected)
			{
				values.Add(0);
				values.Add(1);
				values.Add(expected);
				values.Add(3);
				values.Add(4);
				IEnumerable<int> subject = values;

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected - 1)).AtIndexFromEnd(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has an item that is equal to {expected - 1} at index 2 from end,
					              but it had item {expected} at index 2 from end

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed(
				List<int> values, int expected)
			{
				values.Add(0);
				values.Add(1);
				values.Add(expected);
				values.Add(3);
				values.Add(4);
				IEnumerable<int> subject = values;

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected)).AtIndexFromEnd(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail(int expected)
			{
				IEnumerable<int> subject = [expected, 3, 4,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected)).AtIndexFromEnd(3);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has an item that is equal to {expected} at index 3 from end,
					              but it had no item at index 3 from end

					              Collection:
					              [{expected}, 3, 4]
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenEnumerableIsEmpty_ShouldFail(int expected)
			{
				IEnumerable<int> subject = Array.Empty<int>();

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected)).AtIndexFromEnd(0);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has an item that is equal to {expected} at index 0 from end,
					              but it had no item at index 0 from end

					              Collection:
					              []
					              """);
			}

			[Test]
			[Arguments(-1)]
			[Arguments(-10)]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException(int index)
			{
				int[] subject = [];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(0)).AtIndexFromEnd(index);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("index").And
					.WithMessage("The index must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenItemAtIndexDoesNotComply_ShouldShowTheContextsOfTheItem()
			{
				int[][] subject = [[1, 3,], [1, 2,],];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo([1, 2,])).AtIndexFromEnd(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is equal to collection [1, 2,] in order at index 1 from end,
					             but it had item [1, 3] at index 1 from end

					             Collection:
					             [
					               [
					                 1,
					                 3
					               ],
					               [
					                 1,
					                 2
					               ]
					             ]

					             Collection (item [0]):
					             [1, 3]

					             Expected (item [0]):
					             [1, 2]
					             """)
					.Because("exactly the item at the index decides the outcome");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int expected = 42;
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject!).HasItemThat(it => it.IsEqualTo(expected)).AtIndexFromEnd(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is equal to 42 at index 0 from end,
					             but it was <null>
					             """);
			}
		}
		
		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.IsEqualTo(1)).AtIndex(2));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldFail()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.IsEqualTo(2)).AtIndex(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item that is equal to 2 at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Test]
			public async Task WhenItemsUseWhose_ShouldIncludeMemberInExpectation()
			{
				MyClass[] subject = [new(1),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.Whose(o => o.Value, v => v.IsEqualTo(1))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item that has Value that is equal to 1,
					             but it had item MyClass {
					               StringValue = "",
					               Value = 1
					             }

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               }
					             ]
					             """)
					.Because("the negation belongs to the quantifier, so the member keeps its positive form");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.IsNotEqualTo(0)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item that is not equal to 0,
					             but it was <null>
					             """);
			}
		}
	}
}
