using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class IsOneOf
	{
		public sealed class Tests
		{
			[Test]
			public async Task AsPrefix_WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				IEnumerable<string?> expected = ["foo", "bar",];

				async Task Act()
					=> await That(subject).IsOneOf(expected).AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected as prefix,
					             but it was <null>

					             Expected values:
					             ["foo", "bar"]
					             """)
					.Because("a null has no content to inspect, just as for StartsWith");
			}

			[Test]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				string subject = "foo";
				string[] values = ["bar", "baz",];
				IEnumerable<string> expected = Factory.GetSingleUseEnumerable(values);

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}

					              Expected values:
					              ["bar", "baz"]
					              """)
					.Because("the values are cached while they are enumerated, so the comparison and the message share one enumeration");
			}

			[Test]
			public async Task WhenExpectedIsAnEmptySequence_ShouldThrowArgumentException()
			{
				string subject = "foo";
				IEnumerable<string> expected = Factory.GetSingleUseEnumerable<string>();

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("a lazily evaluated sequence is also checked when the expectation is built");
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "foo";
				string[] expected = [];

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenExpectedIsInfiniteAndContainsTheSubject_ShouldSucceed()
			{
				string subject = "item-8";
				IEnumerable<string> expected = Factory.GetFibonacciNumbers(i => $"item-{i}");

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow()
					.Because("the values are only enumerated until the subject is found");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";
				string[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedThrows_ShouldThrowTheExceptionOfTheExpectedItems()
			{
				string subject = "foo";

				IEnumerable<string> GetExpected()
				{
					yield return "bar";
					throw new InvalidOperationException("the expected values are broken");
				}

				async Task Act()
					=> await That(subject).IsOneOf(GetExpected());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("the expected values are broken")
					.Because("an exception of the expected values is not reported as if the subject threw it");
			}

			[Test]
			public async Task WhenExpectedThrowsForTheFirstItem_ShouldThrowTheExceptionOfTheExpectedItems()
			{
				string subject = "foo";

				IEnumerable<string> GetExpected()
				{
					if (subject.Length > 0)
					{
						throw new InvalidOperationException("the expected values are broken");
					}

					yield return "bar";
				}

				async Task Act()
					=> await That(subject).IsOneOf(GetExpected());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("the expected values are broken")
					.Because("the check for an empty sequence passes on the exception of its first item unchanged");
			}

			[Test]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "foo";
				string?[] expected = [];

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";
				string?[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				IEnumerable<string> expected = ["foo", "bar",];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected,
					             but it was <null>

					             Expected values:
					             ["foo", "bar"]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
			{
				string? subject = null;
				IEnumerable<string?> expected = ["foo", null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("foo", "bar", "baz")]
			public async Task WhenValueIsDifferentToAllExpected_ShouldFail(
				string? subject, params string?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments("foo", "bar", "foo", "baz")]
			public async Task WhenValueIsEqualToAnyExpected_ShouldSucceed(
				string? subject, params string?[] expected)
			{
				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueIsEqualToAnyExpectedExceptForTheIndentation_ShouldSucceed()
			{
				string subject = "  foo\n    bar";

				async Task Act()
					=> await That(subject).IsOneOf("baz", "foo\nbar").IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenValueIsNull_ShouldFail(
				params string?[] expected)
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}
		}
	}
}
