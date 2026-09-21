#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class HasItem
	{
		public sealed class PredicateTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).HasItem(_ => true)
						.And.HasItem(_ => true).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasItem(a => a == 5);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).HasItem(_ => false).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => false at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).HasItem(_ => true).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).HasItem(_ => true).AtIndex(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => true at index 3,
					             but it did not contain any item at index 3

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasItem(_ => true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => true,
					             but it did not contain any item

					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasItem(_ => true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => true,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasItem(_ => true).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => true at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2, 3, 4);

				async Task Act()
					=> await That(subject).HasItem(_ => true).WithInvalidMatch();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => true with invalid match,
					             but it did not contain any item with invalid match

					             Collection:
					             [0, 1, 2, 3, 4]
					             """);
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).HasItem(_ => false).AtIndex(0).And.HasItem(_ => false).AtIndex(1).And
						.HasItem(_ => false)
				;

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => false at index 0 and has item _ => false at index 1 and has item _ => false,
					             but it had item "a" at index 0 and it had item "b" at index 1 and it did not match at any index

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class ItemTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).HasItem(1)
						.And.HasItem(1).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasItem(5);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2, 3, 4, 5);

				async Task Act()
					=> await That(subject).HasItem(3).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 3 at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2, 3, 4, 5]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2, 3, 4, 5);

				async Task Act()
					=> await That(subject).HasItem(2).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).HasItem(5).AtIndex(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 5 at index 3,
					             but it did not contain any item at index 3

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableIsEmpty_ShouldFail(int expected)
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasItem(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item {expected},
					              but it did not contain any item

					              Collection:
					              []
					              """);
			}

			[Theory]
			[InlineData(-1)]
			[InlineData(-10)]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException(int index)
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasItem(0).AtIndex(index);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("index").And
					.WithMessage("The index must be greater than or equal to 0.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int expected = 42;
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasItem(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 42,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				int expected = 42;
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasItem(expected).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 42 at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithFixedIndexFromEnd_ShouldFail()
			{
				int expected = 42;
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasItem(expected).AtIndex(0).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 42 at index 0 from end,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2, 3, 4);

				async Task Act()
					=> await That(subject).HasItem(2).WithInvalidMatch();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 2 with invalid match,
					             but it did not contain any item with invalid match

					             Collection:
					             [0, 1, 2, 3, 4]
					             """);
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).HasItem(4).AtIndex(0).And.HasItem(5).AtIndex(1).And.HasItem(6)
				;

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 4 at index 0 and has item 5 at index 1 and has item 6,
					             but it had item 1 at index 0 and it had item 2 at index 1 and it did not match at any index

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class StringItemTests
		{
			[Fact]
			public async Task AsPrefix_WhenItemDoesNotStartWithExpected_ShouldFail()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("f").AsPrefix().AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item starting with "f" at index 1,
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
			public async Task AsPrefix_WhenItemStartsWithExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("b").AsPrefix().AtIndex(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsRegex_WhenItemDoesNotMatch_ShouldFail()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("f[aeiou]?o").AsRegex().AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item matching regex "f[aeiou]?o" at index 1,
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
			public async Task AsRegex_WhenItemMatches_ShouldSucceed()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("b[aeiou]?r").AsRegex().AtIndex(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsSuffix_WhenItemDoesNotEndWithExpected_ShouldFail()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("o").AsSuffix().AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item ending with "o" at index 1,
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
			public async Task AsSuffix_WhenItemEndsWithExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("r").AsSuffix().AtIndex(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsWildcard_WhenItemDoesNotMatch_ShouldFail()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("f*o").AsWildcard().AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item matching "f*o" at index 1,
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
			public async Task AsWildcard_WhenItemMatches_ShouldSucceed()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("b?r").AsWildcard().AtIndex(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldSupportIgnoringCase(bool ignoreCase)
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("BAR").IgnoringCase(ignoreCase).AtIndex(1);

				await That(Act).Throws<XunitException>().OnlyIf(!ignoreCase)
					.WithMessage("""
					             Expected that subject
					             has item equal to "BAR" at index 1,
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
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["a\n  b", "c\n  d", "e\n  f",]);

				async Task Act()
					=> await That(subject).HasItem("c\nd").IgnoringIndentation(ignoreIndentation).AtIndex(1);

				await That(Act).Throws<XunitException>().OnlyIf(!ignoreIndentation)
					.WithMessage("""
					             Expected that subject
					             has item equal to "c\nd" at index 1,
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
			public async Task ShouldSupportIgnoringLeadingWhiteSpace(bool ignoreLeadingWhiteSpace)
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable([" foo", "\tbar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("bar").IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace).AtIndex(1);

				await That(Act).Throws<XunitException>().OnlyIf(!ignoreLeadingWhiteSpace)
					.WithMessage("""
					             Expected that subject
					             has item equal to "bar" at index 1,
					             but it had item "\tbar" at index 1

					             Collection:
					             [
					               " foo",
					               "\tbar",
					               "baz"
					             ]
					             """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldSupportIgnoringNewlineStyle(bool ignoreNewlineStyle)
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["a\nb", "c\nd", "e\nf",]);

				async Task Act()
					=> await That(subject).HasItem("c\r\nd").IgnoringNewlineStyle(ignoreNewlineStyle).AtIndex(1);

				await That(Act).Throws<XunitException>().OnlyIf(!ignoreNewlineStyle)
					.WithMessage("""
					             Expected that subject
					             has item equal to "c\r\nd" at index 1,
					             but it had item "c\nd" at index 1

					             Collection:
					             [
					               "a\nb",
					               "c\nd",
					               "e\nf"
					             ]
					             """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldSupportIgnoringTrailingWhiteSpace(bool ignoreTrailingWhiteSpace)
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo ", "bar\t", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("bar").IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace)
						.AtIndex(1);

				await That(Act).Throws<XunitException>().OnlyIf(!ignoreTrailingWhiteSpace)
					.WithMessage("""
					             Expected that subject
					             has item equal to "bar" at index 1,
					             but it had item "bar\t" at index 1

					             Collection:
					             [
					               "foo ",
					               "bar\t",
					               "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "bar", "c",]);

				async Task Act()
					=> await That(subject).HasItem("foo").AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item equal to "foo" at index 2,
					             but it had item "bar" at index 2

					             Collection:
					             [
					               "a",
					               "b",
					               "bar",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "bar", "c",]);

				async Task Act()
					=> await That(subject).HasItem("bar").AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).HasItem("c").AtIndex(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item equal to "c" at index 3,
					             but it did not contain any item at index 3

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(Array.Empty<string>());

				async Task Act()
					=> await That(subject).HasItem("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item equal to "foo",
					             but it did not contain any item

					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
			{
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject!).HasItem("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item equal to "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				IAsyncEnumerable<string?>? subject = null;

				async Task Act()
					=> await That(subject!).HasItem("bar").AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item equal to "bar" at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithCustomStringComparer_WhenItemsDoNotMatch_ShouldFail()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("BaR").Using(new IgnoreCaseForVocalsComparer()).AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item equal to "BaR" using IgnoreCaseForVocalsComparer at index 1,
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
			public async Task WithCustomStringComparer_WhenItemsMatch_ShouldSucceed()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("bAr").Using(new IgnoreCaseForVocalsComparer()).AtIndex(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).HasItem("foo").WithInvalidMatch();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item equal to "foo" with invalid match,
					             but it did not contain any item with invalid match

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).HasItem("d").AtIndex(0).And.HasItem("e").AtIndex(1).And.HasItem("f")
				;

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item equal to "d" at index 0 and has item equal to "e" at index 1 and has item equal to "f",
					             but it had item "a" at index 0 and it had item "b" at index 1 and it did not match at any index

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class EquivalentTests
		{
			[Fact]
			public async Task WhenEquivalentItemIsFound_ShouldSucceed()
			{
				IAsyncEnumerable<MyClass> subject =
					ToAsyncEnumerable(Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x)).ToArray());
				MyClass expected = new(5);

				async Task Act()
					=> await That(subject).HasItem(expected).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEquivalentItemIsNotFound_ShouldFail()
			{
				IAsyncEnumerable<MyClass> subject =
					ToAsyncEnumerable(Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x)).ToArray());
				MyClass expected = new(4);

				async Task Act()
					=> await That(subject).HasItem(expected).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item equivalent to MyClass {
					               StringValue = "",
					               Value = 4
					             },
					             but it did not match at any index

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
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 8
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 13
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 21
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 34
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 55
					               },
					               …
					             ]
					             """);
			}
		}

		public sealed class UsingTests
		{
			[Fact]
			public async Task WithAllDifferentComparer_ShouldFail()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).HasItem(1).Using(new AllDifferentComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 1 using AllDifferentComparer,
					             but it did not match at any index

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task WithAllEqualComparer_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).HasItem(4).Using(new AllEqualComparer());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class FromEndTests
		{
			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed(int expected)
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, expected, 3, 4);

				async Task Act()
					=> await That(subject).HasItem(expected - 1).AtIndex(2).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item {expected - 1} at index 2 from end,
					              but it had item {expected} at index 2 from end

					              Collection:
					              [0, 1, {expected}, 3, 4]
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed(int expected)
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, expected, 3, 4);

				async Task Act()
					=> await That(subject).HasItem(expected).AtIndex(2).FromEnd();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail(int expected)
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(expected, 3, 4);

				async Task Act()
					=> await That(subject).HasItem(expected).AtIndex(3).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item {expected} at index 3 from end,
					              but it did not contain any item at index 3 from end

					              Collection:
					              [{expected}, 3, 4]
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableIsEmpty_ShouldFail(int expected)
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasItem(expected).AtIndex(0).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item {expected} at index 0 from end,
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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasItem(0).AtIndex(index).FromEnd();

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("index").And
					.WithMessage("The index must be greater than or equal to 0.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int expected = 42;
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject!).HasItem(expected).AtIndex(0).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 42 at index 0 from end,
					             but it was <null>
					             """);
			}
		}
	}
}
#endif
