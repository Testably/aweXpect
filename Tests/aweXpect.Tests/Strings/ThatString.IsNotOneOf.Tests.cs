using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class IsNotOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task AsPrefix_WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				IEnumerable<string?> unexpected = ["foo", "bar",];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} as prefix,
					              but it was <null>
					              """)
					.Because("a null has no content to inspect, just as for DoesNotStartWith");
			}

			[Fact]
			public async Task AsPrefix_WhenUnexpectedContainsNull_ShouldFail()
			{
				string? subject = null;
				IEnumerable<string?> unexpected = ["foo", null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} as prefix,
					              but it was <null>
					              """)
					.Because("a null value inspects nothing, so it remains a plain equality check");
			}

			[Fact]
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

			[Fact]
			public async Task AsWildcard_WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				IEnumerable<string?> unexpected = ["fo*", "ba*",];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).AsWildcard();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} as wildcard,
					              but it was <null>
					              """)
					.Because("a null has no content to match the pattern against");
			}

			[Fact]
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

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";
				string[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected cannot be null.").AsPrefix();
			}

			[Fact]
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

			[Fact]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";
				string?[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				string? subject = null;
				IEnumerable<string> unexpected = ["foo", "bar",];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
			{
				string? subject = null;
				IEnumerable<string?> expected = ["foo", null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task WhenUnexpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				string subject = "foo";
				string[] values = ["bar", "foo",];
				IEnumerable<string> unexpected = Factory.GetSingleUseEnumerable(values);

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(values)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("the values are cached while they are enumerated, so the comparison and the message share one enumeration");
			}

			[Fact]
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

			[Fact]
			public async Task WhenUnexpectedIsInfiniteAndContainsTheSubject_ShouldFail()
			{
				string subject = "item-8";
				IEnumerable<string> unexpected = Factory.GetFibonacciNumbers(i => $"item-{i}");

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("the values are only enumerated until the subject is found");
			}

			[Theory]
			[AutoData]
			public async Task WhenUnexpectedIsNull_ShouldSucceed(
				string subject)
			{
				string? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("foo", "bar", "baz")]
			public async Task WhenValueIsDifferentToAllUnexpected_ShouldSucceed(string subject,
				params string?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("foo", "bar", "foo", "baz")]
			public async Task WhenValueIsEqualToAnyUnexpected_ShouldFail(string subject,
				params string?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
