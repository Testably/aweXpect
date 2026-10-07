using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class IsNotOneOf
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("foo")]
			[Arguments("baz")]
			public async Task AsPrefix_WhenSeveralUnexpectedValuesAreUnusable_ShouldThrowForTheFirstOne(string subject)
			{
				string?[] unexpected = ["foo", "", null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsPrefix();

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' prefix cannot be empty.").AsPrefix()
					.Because("the values are validated in their order");
			}

			[Test]
			public async Task AsPrefix_WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				IEnumerable<string?> unexpected = ["foo", "bar",];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not one of unexpected as prefix,
					             but it was <null>

					             Unexpected values:
					             ["foo", "bar"]
					             """)
					.Because("a null has no content to inspect, just as for DoesNotStartWith");
			}

			[Test]
			[Arguments("foo", true)]
			[Arguments("foo", false)]
			[Arguments("bar", true)]
			[Arguments("bar", false)]
			[Arguments("baz", true)]
			[Arguments("baz", false)]
			[Arguments(null, true)]
			[Arguments(null, false)]
			public async Task AsPrefix_WhenUnexpectedContainsAnEmptyString_ShouldThrowArgumentException(
				string? subject, bool isFirst)
			{
				string?[] unexpected = isFirst ? ["", "foo", "bar",] : ["foo", "bar", "",];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsPrefix();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' prefix cannot be empty.").AsPrefix()
					.Because("an empty prefix is rejected whichever value the subject matches");
			}

			[Test]
			[Arguments("foo", true)]
			[Arguments("foo", false)]
			[Arguments("bar", true)]
			[Arguments("bar", false)]
			[Arguments("baz", true)]
			[Arguments("baz", false)]
			[Arguments(null, true)]
			[Arguments(null, false)]
			public async Task AsPrefix_WhenUnexpectedContainsNull_ForEverySubjectAndPosition_ShouldThrowArgumentNullException(
				string? subject, bool isFirst)
			{
				string?[] unexpected = isFirst ? [null, "foo", "bar",] : ["foo", "bar", null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsPrefix();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' prefix cannot be null.").AsPrefix()
					.Because("a missing prefix is rejected whichever value the subject matches");
			}

			[Test]
			public async Task AsPrefix_WhenUnexpectedContainsNull_ShouldThrowArgumentNullException()
			{
				string? subject = null;
				IEnumerable<string?> unexpected = ["foo", null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsPrefix();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' prefix cannot be null.").AsPrefix()
					.Because("a missing prefix is rejected like a missing regex pattern");
			}

			[Test]
			public async Task AsPrefix_WhenUnexpectedIsInfiniteAndContainsAPrefixOfTheSubject_ShouldFail()
			{
				string subject = "item-8 and more";
				IEnumerable<string> unexpected = Factory.GetFibonacciNumbers(i => $"item-{i}");

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not one of unexpected as prefix,
					             but it was "item-8 and more"

					             Unexpected values:
					             ["item-1", "item-1", "item-2", "item-3", "item-5", "item-8", "item-13", "item-21", "item-34", "item-55", (… and maybe more)]
					             """)
					.Because("a sequence that is not a collection is only enumerated until the subject is found");
			}

			[Test]
			[Arguments("foo", true)]
			[Arguments("foo", false)]
			[Arguments("bar", true)]
			[Arguments("bar", false)]
			[Arguments("baz", true)]
			[Arguments("baz", false)]
			[Arguments(null, true)]
			[Arguments(null, false)]
			public async Task AsRegex_WhenUnexpectedContainsAnEmptyString_ShouldThrowArgumentException(
				string? subject, bool isFirst)
			{
				string?[] unexpected = isFirst ? ["", "foo", "bar",] : ["foo", "bar", "",];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' regex pattern cannot be empty.").AsPrefix()
					.Because("an empty pattern is rejected whichever value the subject matches");
			}

			[Test]
			[Arguments("foo", true)]
			[Arguments("foo", false)]
			[Arguments("bar", true)]
			[Arguments("bar", false)]
			[Arguments("baz", true)]
			[Arguments("baz", false)]
			[Arguments(null, true)]
			[Arguments(null, false)]
			public async Task AsRegex_WhenUnexpectedContainsAnInvalidPattern_ShouldThrowArgumentException(
				string? subject, bool isFirst)
			{
				string?[] unexpected = isFirst ? ["[", "foo", "bar",] : ["foo", "bar", "[",];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' regex pattern is invalid: ").AsPrefix()
					.Because("an invalid pattern is rejected whichever value the subject matches");
			}

			[Test]
			[Arguments("foo", true)]
			[Arguments("foo", false)]
			[Arguments("bar", true)]
			[Arguments("bar", false)]
			[Arguments("baz", true)]
			[Arguments("baz", false)]
			[Arguments(null, true)]
			[Arguments(null, false)]
			public async Task AsRegex_WhenUnexpectedContainsNull_ForEverySubjectAndPosition_ShouldThrowArgumentNullException(
				string? subject, bool isFirst)
			{
				string?[] unexpected = isFirst ? [null, "foo", "bar",] : ["foo", "bar", null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsRegex();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' regex pattern cannot be null.").AsPrefix()
					.Because("a missing pattern is rejected whichever value the subject matches");
			}

			[Test]
			public async Task AsRegex_WhenUnexpectedContainsNull_ShouldThrowArgumentNullException()
			{
				string subject = "bar";
				IEnumerable<string?> unexpected = [null, "foo",];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsRegex();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' regex pattern cannot be null.").AsPrefix()
					.Because("the negated expectation receives the patterns as 'unexpected'");
			}

			[Test]
			[Arguments("foo", true)]
			[Arguments("foo", false)]
			[Arguments("bar", true)]
			[Arguments("bar", false)]
			[Arguments("baz", true)]
			[Arguments("baz", false)]
			[Arguments(null, true)]
			[Arguments(null, false)]
			public async Task AsSuffix_WhenUnexpectedContainsAnEmptyString_ShouldThrowArgumentException(
				string? subject, bool isFirst)
			{
				string?[] unexpected = isFirst ? ["", "foo", "bar",] : ["foo", "bar", "",];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsSuffix();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' suffix cannot be empty.").AsPrefix()
					.Because("an empty suffix is rejected whichever value the subject matches");
			}

			[Test]
			[Arguments("foo", true)]
			[Arguments("foo", false)]
			[Arguments("bar", true)]
			[Arguments("bar", false)]
			[Arguments("baz", true)]
			[Arguments("baz", false)]
			[Arguments(null, true)]
			[Arguments(null, false)]
			public async Task AsSuffix_WhenUnexpectedContainsNull_ShouldThrowArgumentNullException(
				string? subject, bool isFirst)
			{
				string?[] unexpected = isFirst ? [null, "foo", "bar",] : ["foo", "bar", null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsSuffix();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' suffix cannot be null.").AsPrefix()
					.Because("a missing suffix is rejected whichever value the subject matches");
			}

			[Test]
			public async Task AsWildcard_WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				IEnumerable<string?> unexpected = ["fo*", "ba*",];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not one of unexpected as wildcard,
					             but it was <null>

					             Unexpected values:
					             ["fo*", "ba*"]
					             """)
					.Because("a null has no content to match the pattern against");
			}

			[Test]
			[Arguments("foo", true)]
			[Arguments("foo", false)]
			[Arguments("bar", true)]
			[Arguments("bar", false)]
			[Arguments("baz", true)]
			[Arguments("baz", false)]
			[Arguments(null, true)]
			[Arguments(null, false)]
			public async Task AsWildcard_WhenUnexpectedContainsNull_ShouldThrowArgumentNullException(
				string? subject, bool isFirst)
			{
				string?[] unexpected = isFirst ? [null, "foo", "bar",] : ["foo", "bar", null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsWildcard();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' wildcard pattern cannot be null.").AsPrefix()
					.Because("a missing pattern is rejected whichever value the subject matches");
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "foo";
				string[] expected = [];

				object Act()
					=> That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";
				string[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "foo";
				string?[] expected = [];

				object Act()
					=> That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";
				string?[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				string? subject = null;
				IEnumerable<string> unexpected = ["foo", "bar",];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
			{
				string? subject = null;
				IEnumerable<string?> expected = ["foo", null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not one of expected,
					             but it was <null>

					             Unexpected values:
					             ["foo", <null>]
					             """);
			}

			[Test]
			public async Task WhenUnexpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				string subject = "foo";
				string[] values = ["bar", "foo",];
				IEnumerable<string> unexpected = Factory.GetSingleUseEnumerable(values);

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected,
					              but it was {Formatter.Format(subject)}

					              Unexpected values:
					              ["bar", "foo"]
					              """)
					.Because("the values are cached while they are enumerated, so the comparison and the message share one enumeration");
			}

			[Test]
			public async Task WhenUnexpectedIsAnEmptySequence_ShouldThrowArgumentException()
			{
				string subject = "foo";
				IEnumerable<string> unexpected = Factory.GetSingleUseEnumerable<string>();

				object Act()
					=> That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("a lazily evaluated sequence is also checked when the expectation is built");
			}

			[Test]
			public async Task WhenUnexpectedIsInfiniteAndContainsTheSubject_ShouldFail()
			{
				string subject = "item-8";
				IEnumerable<string> unexpected = Factory.GetFibonacciNumbers(i => $"item-{i}");

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected,
					              but it was {Formatter.Format(subject)}

					              Unexpected values:
					              ["item-1", "item-1", "item-2", "item-3", "item-5", "item-8", "item-13", "item-21", "item-34", "item-55", (… and maybe more)]
					              """)
					.Because("the values are only enumerated until the subject is found");
			}

			[Test]
			[AutoArguments]
			public async Task WhenUnexpectedIsNull_ShouldSucceed(
				string subject)
			{
				string? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("foo", "bar", "baz")]
			public async Task WhenValueIsDifferentToAllUnexpected_ShouldSucceed(string subject,
				params string?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("foo", "bar", "foo", "baz")]
			public async Task WhenValueIsEqualToAnyUnexpected_ShouldFail(string subject,
				params string?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
