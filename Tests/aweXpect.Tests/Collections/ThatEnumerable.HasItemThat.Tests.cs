using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItemThat
	{
		public sealed class Tests
		{
			[Fact]
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

			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(int.MinValue))
						.And.HasItemThat(it => it.IsNotEqualTo(int.MinValue)).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(5));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(1)).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has an item that is equal to 1 at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(2)).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has an item that is equal to 3 at index 3,
					              but it did not contain any item at index 3

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				List<int> subject = [];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is not equal to 0,
					             but it did not contain any item

					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasItemThat(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenItemsUseNestedWhose_ShouldRevertToWhoseForTheInnerMember()
			{
				MyClass[] subject = [new(1, "foo"),];

				async Task Act()
					=> await That(subject).HasItemThat(it
						=> it.Whose(o => o.StringValue, s => s.Whose(v => v.Length, l => l.IsEqualTo(5))));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item that has StringValue whose Length is equal to 5,
					             but it had item MyClass {
					               StringValue = "foo",
					               Value = 1
					             }

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

			[Fact]
			public async Task WhenItemsUseWhose_ShouldIncludeMemberInExpectation()
			{
				MyClass[] subject = [new(1),];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.Whose(o => o.Value, v => v.IsEqualTo(5)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item that has Value that is equal to 5,
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
					.Because("the member text must survive the node tree rendering");
			}

			[Fact]
			public async Task WhenItemsUseWhoseAfterAWhichMember_ShouldKeepWhose()
			{
				Exception[] subject = [new("a", new InvalidOperationException("b")),];

				async Task Act()
					=> await That(subject).HasItemThat(it
						=> it.HasInner<InvalidOperationException>(i => i.Whose(e => e.Message, m => m.IsEqualTo("x"))));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item that has an inner InvalidOperationException whose Message is equal to "x",
					             but it had item Exception: a

					             Collection:
					             [
					               Exception: a
					             ]
					             """)
					.Because("a member separated by \"which\" introduces the subject again and can drop the \"which\"");
			}

			[Fact]
			public async Task WhenItemsUseWhoseWithAsyncMember_ShouldIncludeMemberInExpectation()
			{
				MyClass[] subject = [new(1),];

				async Task Act()
					=> await That(subject)
						.HasItemThat(it => it.Whose(o => Task.FromResult(o.Value), v => v.IsEqualTo(5)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item that has Task.FromResult(o.Value) that is equal to 5,
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
					.Because("an async member follows the same rule as a synchronous one");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is not equal to 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0)).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is not equal to 0 at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				IEnumerable<int> subject = [0, 1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(2)).WithInvalidMatch();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is equal to 2 with invalid match,
					             but it did not contain any item with invalid match

					             Collection:
					             [0, 1, 2, 3, 4]
					             """);
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).HasItemThat(x => x.StartsWith("a").And.EndsWith("b")).AtIndex(0).And
						.HasItemThat(x => x.Contains("c").IgnoringCase()).AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item that starts with "a" and ends with "b" at index 0 and has an item that contains "c" at least once ignoring case at index 1,
					             but it had item "a" at index 0 and it had item "b" at index 1

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class FromEndTests
		{
			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed(
				List<int> values, int expected)
			{
				values.Add(0);
				values.Add(1);
				values.Add(expected);
				values.Add(3);
				values.Add(4);
				IEnumerable<int> subject = values;

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected - 1)).AtIndex(2).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has an item that is equal to {expected - 1} at index 2 from end,
					              but it had item {expected} at index 2 from end

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Theory]
			[AutoData]
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
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected)).AtIndex(2).FromEnd();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail(int expected)
			{
				IEnumerable<int> subject = [expected, 3, 4,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected)).AtIndex(3).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has an item that is equal to {expected} at index 3 from end,
					              but it did not contain any item at index 3 from end

					              Collection:
					              [{expected}, 3, 4]
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableIsEmpty_ShouldFail(int expected)
			{
				IEnumerable<int> subject = Array.Empty<int>();

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected)).AtIndex(0).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has an item that is equal to {expected} at index 0 from end,
					              but it did not contain any item at index 0 from end

					              Collection:
					              []
					              """);
			}

			[Theory]
			[InlineData(-1)]
			[InlineData(-10)]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException(int index)
			{
				int[] subject = [];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(0)).AtIndex(index).FromEnd();

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("index").And
					.WithMessage("The index must not be negative.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int expected = 42;
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject!).HasItemThat(it => it.IsEqualTo(expected)).AtIndex(0).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is equal to 42 at index 0 from end,
					             but it was <null>
					             """);
			}
		}
		
		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.IsEqualTo(1)).AtIndex(2));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldFail()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.IsEqualTo(2)).AtIndex(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item that is equal to 2 at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenItemsUseWhose_ShouldIncludeMemberInExpectation()
			{
				MyClass[] subject = [new(1),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.Whose(o => o.Value, v => v.IsEqualTo(1))));

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.IsNotEqualTo(0)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item that is not equal to 0,
					             but it was <null>
					             """);
			}
		}
	}
}
