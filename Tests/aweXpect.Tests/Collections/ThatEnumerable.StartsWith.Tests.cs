using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class StartsWith
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).StartsWith(1)
						.And.StartsWith(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).StartsWith(1, 1, 2, 3, 5);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				IEnumerable<MyClass> subject = Factory.GetFibonacciNumbers(x => new MyClass(x), 20);

				async Task Act()
					=> await That(subject).StartsWith(new MyClass(1), new MyClass(1), new MyClass(2)).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenCollectionsAreIdentical_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).StartsWith(1, 2, 3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableHasDifferentStartingElements_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				IEnumerable<int> expected = [1, 3,];

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with expected,
					             but it contained item 2 at index 1 instead of 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				IEnumerable<int> expected = [1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with expected,
					             but it contained only 3 items and lacked 1 item: [
					               4
					             ]

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = ToEnumerable([1,]);

				async Task Act()
					=> await That(subject).StartsWith();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1,]);

				async Task Act()
					=> await That(subject).StartsWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenLazySubjectHasMoreItemsThanTheFormatterLimit_ShouldNotNameTheRemainingItems()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,]);

				async Task Act()
					=> await That(subject).StartsWith(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with [2],
					             but it contained item 1 at index 0 instead of 2

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and maybe more)
					             ]
					             """)
					.Because("the enumeration stops early, so the number of remaining items is unknown");
			}

			[Fact]
			public async Task WhenSubjectHasMoreItemsThanTheFormatterLimit_ShouldNameTheRemainingItems()
			{
				int[] subject = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,];

				async Task Act()
					=> await That(subject).StartsWith(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with [2],
					             but it contained item 1 at index 0 instead of 2

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 10 more)
					             ]
					             """)
					.Because("the number of items of an array is known");
			}

			[Fact]
			public async Task WhenSubjectHasOnlyOneItemAndMissesOne_ShouldUseSingular()
			{
				IEnumerable<int> subject = ToEnumerable([1,]);

				async Task Act()
					=> await That(subject).StartsWith(1, 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with [1, 2],
					             but it contained only 1 item and lacked 1 item: [
					               2
					             ]

					             Collection:
					             [1]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).StartsWith(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with [1],
					             but it was <null>
					             """);
			}
		}

		public sealed class StringTests
		{
			[Fact]
			public async Task AsPrefix_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).StartsWith("# ", "### ").AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with ["# ", "### "] as prefix,
					             but it contained item "## Intro" at index 1 instead of prefix "### "

					             Collection:
					             [
					               "# Title",
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Fact]
			public async Task AsPrefix_WhenExpectedContainsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<string> subject = ToEnumerable(["foo",]);
				string[] expected = [null!,];

				async Task Act()
					=> await That(subject).StartsWith(expected).AsPrefix();

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expected' prefix cannot be null.").AsPrefix().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task AsRegex_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).StartsWith("^# ", "^### ").AsRegex();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with ["^# ", "^### "] as regex,
					             but it contained item "## Intro" at index 1 instead of regex "^### "

					             Collection:
					             [
					               "# Title",
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Fact]
			public async Task AsRegex_WhenExpectedContainsAnEmptyPattern_ShouldThrowArgumentException()
			{
				IEnumerable<string> subject = ToEnumerable(["foo",]);

				async Task Act()
					=> await That(subject).StartsWith("").AsRegexThroughOptions();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task AsSuffix_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).StartsWith("Title", "Outro").AsSuffix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with ["Title", "Outro"] as suffix,
					             but it contained item "## Intro" at index 1 instead of suffix "Outro"

					             Collection:
					             [
					               "# Title",
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Fact]
			public async Task AsWildcard_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).StartsWith("# *", "### *").AsWildcard();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with ["# *", "### *"] as wildcard,
					             but it contained item "## Intro" at index 1 instead of wildcard "### *"

					             Collection:
					             [
					               "# Title",
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldIncludeOptionsInFailureMessage()
			{
				IEnumerable<string> subject = ToEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).StartsWith("FOO", "BAZ").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with ["FOO", "BAZ"] ignoring case,
					             but it contained item "bar" at index 1 instead of "BAZ"

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				IEnumerable<string> subject = ToEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).StartsWith("FOO", "BAR").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedContainsNull_ShouldMatchANullItem()
			{
				string?[] subject = ["a", null, "b",];

				async Task Act()
					=> await That(subject).StartsWith("a", null);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedContainsNull_WhenTheItemIsNotNull_ShouldFail()
			{
				string?[] subject = ["a", "b",];

				async Task Act()
					=> await That(subject).StartsWith("a", null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with ["a", <null>],
					             but it contained item "b" at index 1 instead of <null>

					             Collection:
					             [
					               "a",
					               "b"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectStartsWithExpectedValues_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["foo", "bar", "baz",]);
				IEnumerable<string> expected = ToEnumerable(["foo", "bar",]);

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectDoesNotStartWithExpected_ShouldSucceed()
			{
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.StartsWith(2, 3));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectStartsWithExpected_ShouldFail()
			{
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.StartsWith(1, 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with [1, 2],
					             but it did start with [1, 2]
					             """);
			}
		}
	}
}
