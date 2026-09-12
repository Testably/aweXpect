namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public sealed class HasMessageContaining
	{
		public sealed class Tests
		{
			[Fact]
			public async Task CanCompareCaseInsensitive()
			{
				string message = "_FOO_BAR";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessageContaining("foo").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CanUseWildcardCheck()
			{
				string message = "_foo-BAR";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessageContaining("f?o-");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldCompareCaseSensitive()
			{
				string message = "FOO";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessageContaining("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that exception
					             contains Message matching "foo",
					             but it did not match:
					               ↓ (actual)
					               "FOO"
					               "foo"
					               ↑ (wildcard pattern)

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
					=> await That(exception).HasMessageContaining("foo");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldIgnoreSucceedingText()
			{
				string message = "foo and some other text";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessageContaining("foo");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldIncludeExceptionType()
			{
				string message = "FOO";
				Exception exception = new CustomException(message);

				async Task Act()
					=> await That(exception).HasMessageContaining("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that exception
					             contains Message matching "foo",
					             but it did not match:
					               ↓ (actual)
					               "FOO"
					               "foo"
					               ↑ (wildcard pattern)

					             Message:
					             FOO
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				string message = "foo and some other text";
				MyException exception = new(message);

				async Task Act()
					=> await That(exception).HasMessageContaining(null);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMessagesAreDifferent_ShouldFail()
			{
				string actual = "expected actual text";
				string expected = "expected other text";
				CustomException subject = new(actual);

				async Task Act()
					=> await That(subject).HasMessageContaining(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains Message matching "expected other text",
					             but it did not match:
					               ↓ (actual)
					               "expected actual text"
					               "expected other text"
					               ↑ (wildcard pattern)

					             Message:
					             expected actual text
					             """);
			}
			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasMessageContaining("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains Message matching "foo",
					             but it was <null>
					             """);
			}		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenStringsAreEqual_ShouldFail()
			{
				string actual = "my text";
				Exception subject = new(actual);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(e => e.HasMessageContaining(actual));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain Message matching "my text",
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
					=> await That(subject).DoesNotComplyWith(e => e.HasMessageContaining(expected));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
