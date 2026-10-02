#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class DoesNotEndWith
	{
		public sealed class Tests
		{
			[Fact]
			public async Task AsPrefix_WhenTheItemsMatchTheirPatterns_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).DoesNotEndWith("## ", "te").AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with ["## ", "te"] as prefix,
					             but it did end with [
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Fact]
			public async Task AsRegex_WhenTheItemsMatchTheirPatterns_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).DoesNotEndWith("^## ", "^te").AsRegex();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with ["^## ", "^te"] as regex,
					             but it did end with [
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Fact]
			public async Task AsRegex_WhenUnexpectedContainsAnEmptyPattern_ShouldThrowArgumentException()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo",]);

				async Task Act()
					=> await That(subject).DoesNotEndWith("").AsRegexThroughOptions();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'unexpected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("unexpected")
					.Because("the negated expectation receives the patterns as 'unexpected'");
			}

			[Fact]
			public async Task AsSuffix_WhenTheItemsMatchTheirPatterns_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).DoesNotEndWith("Intro", "xt").AsSuffix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with ["Intro", "xt"] as suffix,
					             but it did end with [
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Fact]
			public async Task AsWildcard_WhenTheItemsMatchTheirPatterns_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

				async Task Act()
					=> await That(subject).DoesNotEndWith("## *", "t*t").AsWildcard();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with ["## *", "t*t"] as wildcard,
					             but it did end with [
					               "## Intro",
					               "text"
					             ]
					             """);
			}

			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).DoesNotEndWith(0)
						.And.DoesNotEndWith(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldSupportCaseInsensitiveComparison()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["FOO", "BAR",]);

				async Task Act()
					=> await That(subject).DoesNotEndWith("bar").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with ["bar"] ignoring case,
					             but it did end with [
					               "BAR"
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				IAsyncEnumerable<MyClass> subject = Factory.GetAsyncFibonacciNumbers(x => new MyClass(x), 6);

				async Task Act()
					=> await That(subject).DoesNotEndWith(
						new MyClass(3),
						new MyClass(5),
						new MyClass(8)
					).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [MyClass { StringValue = "", Value = 3 }, MyClass { StringValue = "", Value = 5 }, MyClass { StringValue = "", Value = 8 }] using equivalency,
					             but it did end with [
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
					               }
					             ]

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Fact]
			public async Task WhenCollectionsAreIdentical_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotEndWith(1, 2, 3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [1, 2, 3],
					             but it did end with [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableHasDifferentEndingElements_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 0, 1, 2, 3);
				IEnumerable<int> unexpected = [1, 3,];

				async Task Act()
					=> await That(subject).DoesNotEndWith(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectEndsWithUnexpectedValues_ShouldOnlyReportTheMatchingSuffix()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5);

				async Task Act()
					=> await That(subject).DoesNotEndWith(4, 5);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [4, 5],
					             but it did end with [4, 5]
					             """);
			}

			[Fact]
			public async Task WhenSubjectEndsWithUnexpectedValues_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);
				IEnumerable<string> unexpected = ["bar", "baz",];

				async Task Act()
					=> await That(subject).DoesNotEndWith(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with unexpected,
					             but it did end with [
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotEndWith(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [1],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedContainsAdditionalElements_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotEndWith(0, 0, 1, 2, 3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2);

				async Task Act()
					=> await That(subject).DoesNotEndWith();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1);

				async Task Act()
					=> await That(subject).DoesNotEndWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}
		}
	}
}
#endif
