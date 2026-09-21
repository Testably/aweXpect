using System.Collections.Generic;
using System.Linq;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed class PredicateTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => false)
						.And.DoesNotHaveItem(_ => false).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).DoesNotHaveItem(a => a == 5).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item _ => true at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldSucceed()
			{
				List<int> subject =
				[
					0,
					1,
					2,
				];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => false).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed()
			{
				List<int> subject = [];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item _ => true,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item _ => true at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithAnyIndex_WhenAnItemMatches_ShouldFail()
			{
				IEnumerable<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(a => a == 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item a => a == 1,
					             but it had item 1

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WithInvalidMatch_ShouldSucceed()
			{
				IEnumerable<int> subject = [0, 1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).WithInvalidMatch();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ItemTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).DoesNotHaveItem(42)
						.And.DoesNotHaveItem(42).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed(
				List<int> subject, int expected)
			{
				subject.Add(0);
				subject.Add(1);
				subject.Insert(2, expected);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(expected - 1).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsUnexpectedItemAtGivenIndex_ShouldFail(
				List<int> subject, int unexpected)
			{
				subject.Add(0);
				subject.Add(1);
				subject.Insert(2, unexpected);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item {unexpected} at index 2,
					              but it had item {unexpected} at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed(int unexpected)
			{
				List<int> subject =
				[
					0,
					1,
					unexpected,
				];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).AtIndex(3);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed(int unexpected)
			{
				List<int> subject = [];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			[InlineData(-10)]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException(int index)
			{
				int[] subject = [];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(0).AtIndex(index);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("index").And
					.WithMessage("The index must be greater than or equal to 0.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
			{
				int unexpected = 42;
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item 42,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				int unexpected = 42;
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item 42 at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithAnyIndex_WhenAnItemMatches_ShouldFail()
			{
				IEnumerable<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item 1,
					             but it had item 1

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WithInvalidMatch_ShouldSucceed()
			{
				IEnumerable<int> subject = [0, 1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(2).WithInvalidMatch();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1).AtIndex(0).And.DoesNotHaveItem(2).AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item 1 at index 0 and does not have item 2 at index 1,
					             but it had item 1 at index 0 and it had item 2 at index 1

					             Collection:
					             [
					               1,
					               2,
					               3
					             ]
					             """);
			}
		}

		public sealed class StringItemTests
		{
			[Fact]
			public async Task AsPrefix_WhenItemDoesNotStartWithUnexpected_ShouldSucceed()
			{
				IEnumerable<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("f").AsPrefix().AtIndex(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsPrefix_WhenItemIsNull_ShouldSucceed()
			{
				IEnumerable<string?> subject = ["foo", null, "baz",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("b").AsPrefix().AtIndex(1);

				await That(Act).DoesNotThrow()
					.Because("the null philosophy applies to the subject, so a null item is simply no match");
			}

			[Fact]
			public async Task AsPrefix_WhenItemStartsWithUnexpected_ShouldFail()
			{
				IEnumerable<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("b").AsPrefix().AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item starting with "b" at index 1,
					             but it had item "bar" at index 1

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task AsRegex_WhenItemMatches_ShouldFail()
			{
				IEnumerable<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("b[aeiou]?r").AsRegex().AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item matching regex "b[aeiou]?r" at index 1,
					             but it had item "bar" at index 1

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task AsSuffix_WhenItemEndsWithUnexpected_ShouldFail()
			{
				IEnumerable<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("r").AsSuffix().AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item ending with "r" at index 1,
					             but it had item "bar" at index 1

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task AsWildcard_WhenItemMatches_ShouldFail()
			{
				IEnumerable<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("b?r").AsWildcard().AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item matching "b?r" at index 1,
					             but it had item "bar" at index 1

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldSupportIgnoringCase(bool ignoreCase)
			{
				IEnumerable<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("BAR").IgnoringCase(ignoreCase).AtIndex(1);

				await That(Act).Throws<XunitException>().OnlyIf(ignoreCase)
					.WithMessage("""
					             Expected that subject
					             does not have item equal to "BAR" ignoring case at index 1,
					             but it had item "bar" at index 1

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldSupportIgnoringIndentation(bool ignoreIndentation)
			{
				IEnumerable<string?> subject = ["a\n  b", "c\n  d", "e\n  f",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("c\nd").IgnoringIndentation(ignoreIndentation)
						.AtIndex(1);

				await That(Act).Throws<XunitException>().OnlyIf(ignoreIndentation)
					.WithMessage("""
					             Expected that subject
					             does not have item equal to "c\nd" ignoring indentation at index 1,
					             but it had item "c\n  d" at index 1

					             Collection:
					             [
					               "a\n  b",
					               "c\n  d",
					               "e\n  f"
					             ]
					             """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldSupportIgnoringNewlineStyle(bool ignoreNewlineStyle)
			{
				IEnumerable<string?> subject = ["a\nb", "c\nd", "e\nf",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("c\r\nd").IgnoringNewlineStyle(ignoreNewlineStyle)
						.AtIndex(1);

				await That(Act).Throws<XunitException>().OnlyIf(ignoreNewlineStyle)
					.WithMessage("""
					             Expected that subject
					             does not have item equal to "c\r\nd" ignoring newline style at index 1,
					             but it had item "c\nd" at index 1

					             Collection:
					             [
					               "a\nb",
					               "c\nd",
					               "e\nf"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed()
			{
				List<string> subject = ["a", "b", "bar",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("foo").AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsUnexpectedItemAtGivenIndex_ShouldFail()
			{
				List<string> subject = ["a", "b", "bar",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("bar").AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item equal to "bar" at index 2,
					              but it had item "bar" at index 2

					              Collection:
					              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				List<string> subject = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("c").AtIndex(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string?>? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotHaveItem("bar").AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item equal to "bar" at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithCustomStringComparer_WhenItemsMatch_ShouldFail()
			{
				IEnumerable<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("bAr").Using(new IgnoreCaseForVocalsComparer()).AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item equal to "bAr" using IgnoreCaseForVocalsComparer at index 1,
					             but it had item "bar" at index 1

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}
		}

		public sealed class EquivalentTests
		{
			[Fact]
			public async Task WhenEquivalentItemIsFound_ShouldFail()
			{
				IEnumerable<MyClass> subject = Factory.GetFibonacciNumbers(5).Select(x => new MyClass(x));
				MyClass unexpected = new(2);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).Equivalent().AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item equivalent to MyClass {
					               StringValue = "",
					               Value = 2
					             } at index 2,
					             but it had item MyClass {
					               StringValue = "",
					               Value = 2
					             } at index 2

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 2
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 3
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 5
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEquivalentItemIsNotFound_ShouldSucceed()
			{
				IEnumerable<MyClass> subject = Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x));
				MyClass unexpected = new(4);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).Equivalent();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class UsingTests
		{
			[Fact]
			public async Task WithAllDifferentComparer_ShouldSucceed()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1).Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAllEqualComparer_ShouldFail()
			{
				IEnumerable<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(4).Using(new AllEqualComparer()).AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item 4 using AllEqualComparer at index 1,
					             but it had item 2 at index 1

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class FromEndTests
		{
			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed(
				List<int> values, int expected)
			{
				values.Add(0);
				values.Add(1);
				values.Add(expected);
				values.Add(3);
				values.Add(4);
				IEnumerable<int> subject = values;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(expected - 1).AtIndex(2).FromEnd();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsUnexpectedItemAtGivenIndex_ShouldFail(
				List<int> values, int unexpected)
			{
				values.Add(0);
				values.Add(1);
				values.Add(unexpected);
				values.Add(3);
				values.Add(4);
				IEnumerable<int> subject = values;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).AtIndex(2).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item {unexpected} at index 2 from end,
					              but it had item {unexpected} at index 2 from end

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed(int unexpected)
			{
				IEnumerable<int> subject = new[]
				{
					unexpected, 3, 4,
				};

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).AtIndex(3).FromEnd();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			[InlineData(-10)]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException(int index)
			{
				int[] subject = [];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(0).AtIndex(index).FromEnd();

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("index").And
					.WithMessage("The index must be greater than or equal to 0.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int unexpected = 42;
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotHaveItem(unexpected).AtIndex(0).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item 42 at index 0 from end,
					             but it was <null>
					             """);
			}
		}
	}
}
