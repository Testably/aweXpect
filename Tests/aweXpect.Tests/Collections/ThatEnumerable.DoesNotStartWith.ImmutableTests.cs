#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotStartWith
	{
		public sealed class ImmutableTests
		{
			[Test]
			public async Task AsPrefix_WhenTheItemsMatchTheirPatterns_ShouldFail()
			{
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

				async Task Act()
					=> await That(subject).DoesNotStartWith("# ", "## ").AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not start with ["# ", "## "] as prefix,
					             but it did start with [
					               "# Title",
					               "## Intro"
					             ]
					             """);
			}

			[Test]
			public async Task AsRegex_WhenTheItemsMatchTheirPatterns_ShouldFail()
			{
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

				async Task Act()
					=> await That(subject).DoesNotStartWith("^# ", "^## ").AsRegex();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not start with ["^# ", "^## "] as regex,
					             but it did start with [
					               "# Title",
					               "## Intro"
					             ]
					             """);
			}

			[Test]
			public async Task AsRegex_WhenUnexpectedContainsAnEmptyPattern_ShouldThrowArgumentException()
			{
				ImmutableArray<string?> subject = ["foo",];

				async Task Act()
					=> await That(subject).DoesNotStartWith("").AsRegexThroughOptions();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'unexpected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("unexpected")
					.Because("the negated expectation receives the patterns as 'unexpected'");
			}

			[Test]
			public async Task AsSuffix_WhenTheItemsMatchTheirPatterns_ShouldFail()
			{
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

				async Task Act()
					=> await That(subject).DoesNotStartWith("Title", "Intro").AsSuffix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not start with ["Title", "Intro"] as suffix,
					             but it did start with [
					               "# Title",
					               "## Intro"
					             ]
					             """);
			}

			[Test]
			public async Task AsWildcard_WhenTheItemsMatchTheirPatterns_ShouldFail()
			{
				ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

				async Task Act()
					=> await That(subject).DoesNotStartWith("# *", "## *").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not start with ["# *", "## *"] as wildcard,
					             but it did start with [
					               "# Title",
					               "## Intro"
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSupportCaseInsensitiveComparison()
			{
				ImmutableArray<string?> subject = ["FOO", "BAR",];

				async Task Act()
					=> await That(subject).DoesNotStartWith("foo").IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not start with ["foo"] ignoring case,
					             but it did start with [
					               "FOO"
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSupportEquivalent()
			{
				ImmutableArray<MyClass> subject = [..Factory.GetFibonacciNumbers(x => new MyClass(x), 20),];
				IEnumerable<MyClass> unexpected = [new(1), new(1), new(2),];

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected).Equivalent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not start with unexpected using equivalency,
					             but it did start with [
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
					               }
					             ]

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenCollectionsAreIdentical_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotStartWith(1, 2, 3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not start with [1, 2, 3],
					             but it did start with [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenEnumerableHasDifferentStartingElements_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<int> unexpected = [1, 3,];

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectStartsWithUnexpectedValues_ShouldFail()
			{
				ImmutableArray<string> subject = ["foo", "bar", "baz",];
				IEnumerable<string> unexpected = ["foo", "bar",];

				async Task Act()
					=> await That(subject)!.DoesNotStartWith(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not start with unexpected,
					             but it did start with [
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Test]
			public async Task WhenUnexpectedContainsAdditionalElements_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotStartWith(1, 2, 3, 4);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				ImmutableArray<int> subject = [1, 2,];

				async Task Act()
					=> await That(subject).DoesNotStartWith();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				ImmutableArray<int> subject = [1,];

				async Task Act()
					=> await That(subject).DoesNotStartWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}
		}
	}
}
#endif
