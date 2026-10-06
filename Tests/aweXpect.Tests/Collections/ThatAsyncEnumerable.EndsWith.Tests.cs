#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class EndsWith
	{
		public sealed class Tests
		{
			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).EndsWith(1)
						.And.EndsWith(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldSupportEquivalent()
			{
				IAsyncEnumerable<MyClass> subject = Factory.GetAsyncFibonacciNumbers(x => new MyClass(x), 6);

				async Task Act()
					=> await That(subject).EndsWith(
						new MyClass(3),
						new MyClass(5),
						new MyClass(8)
					).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCollectionsAreIdentical_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).EndsWith(1, 2, 3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableHasDifferentEndingElements_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 0, 1, 2, 3);
				IEnumerable<int> expected = [1, 3,];

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item 2 at index 3 instead of 1

					             Collection:
					             [0, 0, 1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenEvaluatedForSeveralItems_ShouldOnlyConsiderTheItemsOfEachOne()
			{
				IAsyncEnumerable<int>[] subject = [ToAsyncEnumerable(1, 2, 3), ToAsyncEnumerable<int>(),];

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.EndsWith(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with [3] for all items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               IAsyncEnumerable<int>
					             ]

					             Collection:
					             [
					               IAsyncEnumerable<int>,
					               IAsyncEnumerable<int>
					             ]

					             Collection (item [1]):
					             []
					             """)
					.Because("the second item is empty");
			}

			[Test]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).EndsWith(0, 0, 1, 2, 3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with [0, 0, 1, 2, 3],
					             but it contained only 3 items and lacked 2 items: [
					               0,
					               0
					             ]

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1);

				async Task Act()
					=> await That(subject).EndsWith();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1);

				async Task Act()
					=> await That(subject).EndsWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenRetriedAfterAMismatch_ShouldDescribeTheLastAttempt()
			{
				int attempts = 0;

				IAsyncEnumerable<int> GetSubject()
					=> attempts++ == 0 ? ToAsyncEnumerable(3) : ToAsyncEnumerable(2);

				async Task Act()
					=> await That(GetSubject).Eventually().WithinTwoAttempts(5.Seconds())
						.EndsWith(1, 2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that GetSubject
					             eventually ends with [1, 2] within 0:05,
					             but it contained only 1 item and lacked 1 item: [
					               1
					             ]

					             Collection:
					             [2]
					             """)
					.Because("the mismatch of the first attempt does not apply to the later ones");
			}

			[Test]
			public async Task WhenSubjectHasOnlyOneItemAndMissesOne_ShouldUseSingular()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable([2,]);

				async Task Act()
					=> await That(subject).EndsWith(1, 2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with [1, 2],
					             but it contained only 1 item and lacked 1 item: [
					               1
					             ]

					             Collection:
					             [2]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).EndsWith(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with [1],
					             but it was <null>
					             """);
			}
		}

		public sealed class StringTests
		{
			[Test]
			public async Task AsPrefix_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).EndsWith("### ", "te").AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with ["### ", "te"] as prefix,
					             but it contained item "## Intro" at index 1 instead of prefix "### "

					             Collection:
					             [
					               "# Title",
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Test]
			public async Task AsRegex_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).EndsWith("^### ", "^te").AsRegex();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with ["^### ", "^te"] as regex,
					             but it contained item "## Intro" at index 1 instead of regex "^### "

					             Collection:
					             [
					               "# Title",
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Test]
			public async Task AsRegex_WhenExpectedContainsAnEmptyPattern_ShouldThrowArgumentException()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo",]);

				async Task Act()
					=> await That(subject).EndsWith("").AsRegexThroughOptions();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("expected");
			}

			[Test]
			public async Task AsSuffix_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).EndsWith("Outro", "xt").AsSuffix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with ["Outro", "xt"] as suffix,
					             but it contained item "## Intro" at index 1 instead of suffix "Outro"

					             Collection:
					             [
					               "# Title",
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Test]
			public async Task AsWildcard_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).EndsWith("### *", "t*t").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with ["### *", "t*t"] as wildcard,
					             but it contained item "## Intro" at index 1 instead of wildcard "### *"

					             Collection:
					             [
					               "# Title",
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Test]
			public async Task ShouldIncludeOptionsInFailureMessage()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).EndsWith("FOO", "BAZ").IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with ["FOO", "BAZ"] ignoring case,
					             but it contained item "bar" at index 1 instead of "FOO"

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSupportIgnoringCase()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).EndsWith("BAR", "BAZ").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedContainsNull_ShouldMatchANullItem()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>("b", "a", null);

				async Task Act()
					=> await That(subject).EndsWith("a", null);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedContainsNull_WhenTheItemIsNotNull_ShouldFail()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>("a", "b");

				async Task Act()
					=> await That(subject).EndsWith("a", null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with ["a", <null>],
					             but it contained item "b" at index 1 instead of <null>

					             Collection:
					             [
					               "a",
					               "b"
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectEndsWithExpectedValues_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);
				IEnumerable<string> expected = ["bar", "baz",];

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
