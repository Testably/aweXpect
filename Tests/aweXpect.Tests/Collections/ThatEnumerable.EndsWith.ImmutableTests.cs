#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class EndsWith
	{
		public sealed class ImmutableTests
		{
			[Test]
			public async Task ShouldSupportEquivalent()
			{
				ImmutableArray<MyClass> subject = [..Factory.GetFibonacciNumbers(x => new MyClass(x), 6),];

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
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).EndsWith(1, 2, 3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableHasDifferentEndingElements_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 0, 1, 2, 3,];
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
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

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
				ImmutableArray<int> subject = [1,];

				async Task Act()
					=> await That(subject).EndsWith();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				ImmutableArray<int> subject = [1,];

				async Task Act()
					=> await That(subject).EndsWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}
		}

		public sealed class ImmutableStringTests
		{
			[Test]
			public async Task AsPrefix_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

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
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

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
				ImmutableArray<string?> subject = ["foo",];

				async Task Act()
					=> await That(subject).EndsWith("").AsRegexThroughOptions();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("expected");
			}

			[Test]
			public async Task AsSuffix_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

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
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

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
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];

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
				ImmutableArray<string> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject)!.EndsWith("BAR", "BAZ").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsAnEnumerable_ShouldNameItsExpression()
			{
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];
				IEnumerable<string> expected = ["foo", "baz",];

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item "bar" at index 1 instead of "foo"

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectEndsWithExpectedValues_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];
				IEnumerable<string> expected = ["bar", "baz",];

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
