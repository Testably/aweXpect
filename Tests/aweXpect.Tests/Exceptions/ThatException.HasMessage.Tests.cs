namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public sealed class HasMessage
	{
		public sealed class ContainingTests
		{
			[Fact]
			public async Task CanCompareCaseInsensitive()
			{
				string message = "_FOO_BAR";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().Containing("foo").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldCompareCaseSensitive()
			{
				string message = "FOO";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().Containing("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that exception
					             has Message containing "foo",
					             but it was "FOO"

					             Message:
					             FOO
					             """);
			}

			[Fact]
			public async Task ShouldIgnorePrecedingText()
			{
				string message = "some text before foo";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().Containing("foo");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldIgnoreSucceedingText()
			{
				string message = "foo and some other text";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().Containing("foo");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string message = "foo and some other text";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().Containing(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that exception
					             has Message containing <null>,
					             but it was "foo and some other text"

					             Message:
					             foo and some other text
					             """)
					.Because("a null value is compared like any other value instead of skipping the check");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasMessage().Containing("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has Message containing "foo",
					             but it was <null>
					             """);
			}
		}

		public sealed class EqualToTests
		{
			[Fact]
			public async Task CanUseWildcardCheck()
			{
				Exception subject = new("foo-bar");

				async Task Act()
					=> await That(subject).HasMessage().EqualTo("foo*").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenStringsAreEqual_ShouldSucceed(string actual)
			{
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage().EqualTo(actual);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringsDiffer_ShouldFail()
			{
				string actual = "actual text";
				string expected = "expected other text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has Message equal to "expected other text",
					             but it was "actual text" which differs at index 0:
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
			[Fact]
			public async Task WhenContainingIsNegated_ShouldReadAsNotContaining()
			{
				Exception subject = new("foo and bar");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(e => e.HasMessage().Containing("foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has Message not containing "foo",
					             but it was "foo and bar"

					             Message:
					             foo and bar
					             """)
					.Because("the negation of the continuation reads like NotContaining spelled out");
			}

			[Fact]
			public async Task WhenNotContainingIsNegated_ShouldReadAsContaining()
			{
				Exception subject = new("actual text");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(e => e.HasMessage().NotContaining("foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has Message containing "foo",
					             but it was "actual text"

					             Message:
					             actual text
					             """)
					.Because("negating an inverted constraint restores the positive expectation");
			}

			[Fact]
			public async Task WhenStringsAreEqual_ShouldFail()
			{
				string actual = "my text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(e => e.HasMessage(actual));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has Message not equal to "my text",
					             but it was "my text"

					             Message:
					             my text
					             """);
			}

			[Fact]
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
			[Fact]
			public async Task ShouldCompareCaseSensitive()
			{
				string message = "FOO";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().NotContaining("foo");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringsDiffer_ShouldSucceed()
			{
				string actual = "actual text";
				string expected = "expected other text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage().NotContaining(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasMessage().NotContaining("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has Message not containing "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				string message = "foo and some other text";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().NotContaining(null);

				await That(Act).DoesNotThrow()
					.Because("a null value is compared like any other value instead of skipping the check");
			}

			[Fact]
			public async Task WhenTextIsFollowedByOtherText_ShouldFail()
			{
				string message = "foo and some other text";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().NotContaining("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that exception
					             has Message not containing "foo",
					             but it was "foo and some other text"

					             Message:
					             foo and some other text
					             """);
			}

			[Fact]
			public async Task WhenTextIsPrecededByOtherText_ShouldFail()
			{
				string message = "some text before foo";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessage().NotContaining("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that exception
					             has Message not containing "foo",
					             but it was "some text before foo"

					             Message:
					             some text before foo
					             """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Fact]
			public async Task WhenStringsAreEqual_ShouldFail()
			{
				string actual = "my text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage().NotEqualTo(actual);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has Message not equal to "my text",
					             but it was "my text"

					             Message:
					             my text
					             """);
			}

			[Fact]
			public async Task WhenStringsDiffer_ShouldSucceed()
			{
				string actual = "actual text";
				string expected = "expected other text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage().NotEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasMessage().NotEqualTo("expected text");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has Message not equal to "expected text",
					             but it was <null>
					             """);
			}
		}

		public sealed class Tests
		{
			[Theory]
			[AutoData]
			public async Task WhenStringsAreEqual_ShouldSucceed(string actual)
			{
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage(actual);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringsDiffer_ShouldFail()
			{
				string actual = "actual text";
				string expected = "expected other text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessage(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has Message equal to "expected other text",
					             but it was "actual text" which differs at index 0:
					                ↓ (actual)
					               "actual text"
					               "expected other text"
					                ↑ (expected)

					             Message:
					             actual text
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasMessage("expected text");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has Message equal to "expected text",
					             but it was <null>
					             """);
			}
		}
	}
}
