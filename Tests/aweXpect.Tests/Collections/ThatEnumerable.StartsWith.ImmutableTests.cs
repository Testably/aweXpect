#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class StartsWith
	{
		public sealed class ImmutableTests
		{
			[Test]
			public async Task ShouldSupportEquivalent()
			{
				ImmutableArray<MyClass> subject = [..Factory.GetFibonacciNumbers(x => new MyClass(x), 20),];

				async Task Act()
					=> await That(subject).StartsWith(new MyClass(1), new MyClass(1), new MyClass(2)).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCollectionsAreIdentical_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).StartsWith(1, 2, 3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableHasDifferentStartingElements_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<int> expected = [1, 3,];

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             starts with expected,
					             but it contained item 2 at index 1 instead of 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<int> expected = [1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				ImmutableArray<int> subject = [1,];

				async Task Act()
					=> await That(subject).StartsWith();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}
		}

		public sealed class ImmutableStringTests
		{
			[Test]
			public async Task AsPrefix_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

				async Task Act()
					=> await That(subject).StartsWith("# ", "### ").AsPrefix();

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task AsRegex_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

				async Task Act()
					=> await That(subject).StartsWith("^# ", "^### ").AsRegex();

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task AsRegex_WhenExpectedContainsAnEmptyPattern_ShouldThrowArgumentException()
			{
				ImmutableArray<string?> subject = ["foo",];

				async Task Act()
					=> await That(subject).StartsWith("").AsRegexThroughOptions();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("expected");
			}

			[Test]
			public async Task AsSuffix_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

				async Task Act()
					=> await That(subject).StartsWith("Title", "Outro").AsSuffix();

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task AsWildcard_WhenAnItemDoesNotMatchItsPattern_ShouldFail()
			{
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

				async Task Act()
					=> await That(subject).StartsWith("# *", "### *").AsWildcard();

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task ShouldIncludeOptionsInFailureMessage()
			{
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).StartsWith("FOO", "BAZ").IgnoringCase();

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task ShouldSupportIgnoringCase()
			{
				ImmutableArray<string> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject)!.StartsWith("FOO", "BAR").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsAnEnumerable_ShouldNameItsExpression()
			{
				ImmutableArray<string> subject = ["foo", "bar", "baz",];
				IEnumerable<string> expected = ToEnumerable(["foo", "baz",]);

				async Task Act()
					=> await That(subject)!.StartsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             starts with expected,
					             but it contained item "bar" at index 1 instead of "baz"

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectStartsWithExpectedValues_ShouldSucceed()
			{
				ImmutableArray<string> subject = ["foo", "bar", "baz",];
				IEnumerable<string> expected = ToEnumerable(["foo", "bar",]);

				async Task Act()
					=> await That(subject)!.StartsWith(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
