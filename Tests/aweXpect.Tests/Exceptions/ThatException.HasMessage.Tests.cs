namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public sealed class HasMessage
	{
		public sealed class ContainingTests
		{
			[Test]
			public async Task CanCompareCaseInsensitive()
			{
				string message = "_FOO_BAR";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().Containing("foo").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldCompareCaseSensitive()
			{
				string message = "FOO";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().Containing("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that exception
					             has message containing "foo",
					             but it had message "FOO"

					             Message:
					             FOO
					             """);
			}

			[Test]
			public async Task ShouldIgnorePrecedingText()
			{
				string message = "some text before foo";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().Containing("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldIgnoreSucceedingText()
			{
				string message = "foo and some other text";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().Containing("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				MyException exception = new("foo and some other text");

				async Task Act()
					=> await That(exception).HasMessage().Containing("");

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' string cannot be empty.").AsPrefix().And
					.WithParamName("expected");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				MyException exception = new("foo and some other text");

				async Task Act()
					=> await That(exception).HasMessage().Containing(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasMessage().Containing("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has message containing "foo",
					             but it was <null>
					             """);
			}
		}

		public sealed class EndingWithTests
		{
			[Test]
			public async Task WhenMessageDoesNotEndWithExpected_ShouldFail()
			{
				MyException exception = new("foo and some other text");

				async Task Act()
					=> await That(exception).HasMessage().EndingWith("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that exception
					             has message ending with "foo",
					             but it had message "foo and some other text"*

					             Message:
					             foo and some other text
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMessageEndsWithExpected_ShouldSucceed()
			{
				MyException exception = new("some text before foo");

				async Task Act()
					=> await That(exception).HasMessage().EndingWith("foo");

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EqualToTests
		{
			[Test]
			public async Task AsNumber_WhenExpectedIsNoNumber_ShouldThrowArgumentException()
			{
				Exception subject = new("1");

				async Task Act()
					=> await That(subject).HasMessage().EqualTo("foo").AsNumber();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The value \"foo\" is no number.").AsPrefix();
			}

			[Test]
			public async Task AsNumber_WhenReadingTheMessageThrows_ShouldThrowArgumentException()
			{
				Exception subject = new ThrowingMessageException(new InvalidOperationException("message failed"));

				async Task Act()
					=> await That(subject).HasMessage().EqualTo("foo").AsNumber();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The value \"foo\" is no number.").AsPrefix()
					.Because("an unusable expected value is rejected also when no message is compared with it");
			}

			[Test]
			public async Task AsNumber_WhenSubjectIsNull_ShouldThrowArgumentException()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasMessage().EqualTo("foo").AsNumber();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The value \"foo\" is no number.").AsPrefix();
			}

			[Test]
			public async Task AsRegex_WhenExpectedIsAnEmptyPattern_ShouldThrowArgumentException()
			{
				Exception subject = new("foo");

				async Task Act()
					=> await That(subject).HasMessage().EqualTo("").AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("expected");
			}

			[Test]
			public async Task CanUseWildcardCheck()
			{
				Exception subject = new("foo-bar");

				async Task Act()
					=> await That(subject).HasMessage().EqualTo("foo*").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenStringsAreEqual_ShouldSucceed(string actual)
			{
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage().EqualTo(actual);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringsDiffer_ShouldFail()
			{
				string actual = "actual text";
				string expected = "expected other text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has message equal to "expected other text",
					             but it had message "actual text", which differs at index 0:
					                ↓ (actual)
					               "actual text"
					               "expected other text"
					                ↑ (expected)

					             Message:
					             actual text
					             """)
					.Because("the continuation renders exactly like the HasMessage(expected) shorthand");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenContainingIsNegated_ShouldReadAsNotContaining()
			{
				Exception subject = new("foo and bar");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(e => e.HasMessage().Containing("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have message containing "foo",
					             but it had message "foo and bar"

					             Message:
					             foo and bar
					             """)
					.Because("the negation of the continuation reads like NotContaining spelled out");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				Exception subject = new("foo");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(e => e.HasMessage(null));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNotContainingIsNegated_ShouldReadAsContaining()
			{
				Exception subject = new("actual text");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(e => e.HasMessage().NotContaining("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has message containing "foo",
					             but it had message "actual text"

					             Message:
					             actual text
					             """)
					.Because("negating an inverted constraint restores the positive expectation");
			}

			[Test]
			public async Task WhenReadingTheMessageThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("message failed");
				Exception subject = new ThrowingMessageException(exception);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(e => e.HasMessage("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have message equal to "foo",
					             but message did throw an InvalidOperationException:
					               message failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a message that was never read cannot prove inequality either");
			}

			[Test]
			public async Task WhenStringsAreEqual_ShouldFail()
			{
				string actual = "my text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(e => e.HasMessage(actual));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have message equal to "my text",
					             but it had message "my text"

					             Message:
					             my text
					             """);
			}

			[Test]
			public async Task WhenStringsDiffer_ShouldSucceed()
			{
				string actual = "actual text";
				string expected = "expected other text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(e => e.HasMessage(expected));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NotContainingTests
		{
			[Test]
			public async Task ShouldCompareCaseSensitive()
			{
				string message = "FOO";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().NotContaining("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringsDiffer_ShouldSucceed()
			{
				string actual = "actual text";
				string expected = "expected other text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage().NotContaining(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasMessage().NotContaining("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have message containing "foo",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTextIsFollowedByOtherText_ShouldFail()
			{
				string message = "foo and some other text";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().NotContaining("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that exception
					             does not have message containing "foo",
					             but it had message "foo and some other text"

					             Message:
					             foo and some other text
					             """);
			}

			[Test]
			public async Task WhenTextIsPrecededByOtherText_ShouldFail()
			{
				string message = "some text before foo";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().NotContaining("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that exception
					             does not have message containing "foo",
					             but it had message "some text before foo"

					             Message:
					             some text before foo
					             """);
			}

			[Test]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				MyException exception = new("foo and some other text");

				async Task Act()
					=> await That(exception).HasMessage().NotContaining("");

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'unexpected' string cannot be empty.").AsPrefix().And
					.WithParamName("unexpected");
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				MyException exception = new("foo and some other text");

				async Task Act()
					=> await That(exception).HasMessage().NotContaining(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}
		}

		public sealed class NotEndingWithTests
		{
			[Test]
			public async Task WhenMessageDoesNotEndWithUnexpected_ShouldSucceed()
			{
				MyException exception = new("foo and some other text");

				async Task Act()
					=> await That(exception).HasMessage().NotEndingWith("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMessageEndsWithUnexpected_ShouldFail()
			{
				MyException exception = new("some text before foo");

				async Task Act()
					=> await That(exception).HasMessage().NotEndingWith("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that exception
					             does not have message ending with "foo",
					             but it had message "some text before foo"

					             Message:
					             some text before foo
					             """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Test]
			public async Task WhenStringsAreEqual_ShouldFail()
			{
				string actual = "my text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage().NotEqualTo(actual);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have message equal to "my text",
					             but it had message "my text"

					             Message:
					             my text
					             """);
			}

			[Test]
			public async Task WhenStringsDiffer_ShouldSucceed()
			{
				string actual = "actual text";
				string expected = "expected other text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage().NotEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasMessage().NotEqualTo("expected text");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have message equal to "expected text",
					             but it was <null>
					             """);
			}
		}

		public sealed class NotStartingWithTests
		{
			[Test]
			public async Task WhenMessageDoesNotStartWithUnexpected_ShouldSucceed()
			{
				MyException exception = new("some text before foo");

				async Task Act()
					=> await That(exception).HasMessage().NotStartingWith("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMessageStartsWithUnexpected_ShouldFail()
			{
				MyException exception = new("foo and some other text");

				async Task Act()
					=> await That(exception).HasMessage().NotStartingWith("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that exception
					             does not have message starting with "foo",
					             but it had message "foo and some other text"

					             Message:
					             foo and some other text
					             """);
			}
		}

		public sealed class StartingWithTests
		{
			[Test]
			public async Task WhenMessageDoesNotStartWithExpected_ShouldFail()
			{
				MyException exception = new("some text before foo");

				async Task Act()
					=> await That(exception).HasMessage().StartingWith("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that exception
					             has message starting with "foo",
					             but it had message "some text before foo"*

					             Message:
					             some text before foo
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMessageStartsWithExpected_ShouldSucceed()
			{
				MyException exception = new("foo and some other text");

				async Task Act()
					=> await That(exception).HasMessage().StartingWith("foo");

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenChainedWithHasParamName_ShouldApplyBoth()
			{
				ArgumentException subject = new("outer", "paramName");

				async Task Act()
					=> await That(subject).HasMessage().StartingWith("outer").And.HasParamName("paramName");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Exception subject = new("foo");

				async Task Act()
					=> await That(subject).HasMessage(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has message equal to <null>,
					             but it had message "foo"

					             Message:
					             foo
					             """);
			}

			[Test]
			public async Task WhenParamNameDiffersInAChainWithHasParamName_ShouldFail()
			{
				ArgumentException subject = new("outer", "paramName");

				async Task Act()
					=> await That(subject).HasMessage().StartingWith("outer").And.HasParamName("other");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has message starting with "outer" and has param name equal to "other",
					             but it had param name "paramName", which differs at index 0:
					                ↓ (actual)
					               "paramName"
					               "other"
					                ↑ (expected)

					             Param name:
					             paramName
					             """)
					.Because("the message was met, so it does not explain the failure");
			}

			[Test]
			public async Task WhenReadingTheMessageThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("message failed");
				Exception subject = new ThrowingMessageException(exception);

				async Task Act()
					=> await That(subject).HasMessage("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has message equal to "foo",
					             but message did throw an InvalidOperationException:
					               message failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a message that cannot be read fails the expectation instead of aborting its evaluation");
			}

			[Test]
			[AutoArguments]
			public async Task WhenStringsAreEqual_ShouldSucceed(string actual)
			{
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage(actual);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringsDiffer_ShouldFail()
			{
				string actual = "actual text";
				string expected = "expected other text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has message equal to "expected other text",
					             but it had message "actual text", which differs at index 0:
					                ↓ (actual)
					               "actual text"
					               "expected other text"
					                ↑ (expected)

					             Message:
					             actual text
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasMessage("expected text");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has message equal to "expected text",
					             but it was <null>
					             """);
			}
		}

		private sealed class ThrowingMessageException(Exception exception) : Exception
		{
			public override string Message => throw exception;
		}
	}
}
