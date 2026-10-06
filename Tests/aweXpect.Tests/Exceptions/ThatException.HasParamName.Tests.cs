namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public class HasParamName
	{
		public sealed class ContainingTests
		{
			[Test]
			[AutoArguments]
			public async Task WhenParamNameContainsExpected_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().Containing("essag");

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenParamNameDoesNotContainExpected_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().Containing("somethingElse");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has param name containing "somethingElse",
					             but it had param name "message" with a length of 7, which is shorter than the expected length of 13

					             Param name:
					             message
					             """);
			}
		}

		public sealed class EqualToTests
		{
			[Test]
			[AutoArguments]
			public async Task WhenParamNameIsDifferent_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().EqualTo("somethingElse");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has param name equal to "somethingElse",
					             but it had param name "message", which differs at index 0:
					                ↓ (actual)
					               "message"
					               "somethingElse"
					                ↑ (expected)

					             Param name:
					             message
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenParamNameMatchesExpected_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().EqualTo("message");

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenParamNameMatchesPrefix_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().EqualTo("mes").AsPrefix();

				await That(Act).DoesNotThrow()
					.Because("the continuation exposes the As… family of StringEqualityTypeResult");
			}

			[Test]
			[AutoArguments]
			public async Task WhenSubjectHasNoParamName_ShouldFail(string message)
			{
				ArgumentException subject = new(message);

				async Task Act()
					=> await That(subject).HasParamName().EqualTo("message");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has param name equal to "message",
					             but it had param name <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ArgumentException? subject = null;

				async Task Act()
					=> await That(subject).HasParamName().EqualTo("message");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has param name equal to "message",
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[AutoArguments]
			public async Task WhenExpectedIsNull_AndParamNameIsNull_ShouldFail(string message)
			{
				ArgumentException subject = new(message);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.HasParamName(null));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have param name equal to <null>,
					             but it had param name <null>
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenExpectedIsNull_AndParamNameIsSet_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.HasParamName(null));

				await That(Act).DoesNotThrow()
					.Because("the shorthand compares a null argument as null instead of skipping the check");
			}

			[Test]
			[AutoArguments]
			public async Task WhenParamNameIsDifferent_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.HasParamName("somethingElse"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenParamNameMatchesExpected_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.HasParamName("message"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have param name equal to "message",
					             but it had param name "message"

					             Param name:
					             message
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ArgumentException? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.HasParamName("message"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have param name equal to "message",
					             but it was <null>
					             """);
			}
		}

		public sealed class NotContainingTests
		{
			[Test]
			[AutoArguments]
			public async Task WhenParamNameContainsUnexpected_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().NotContaining("essag");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have param name containing "essag",
					             but it had param name "message"

					             Param name:
					             message
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenParamNameDoesNotContainUnexpected_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().NotContaining("somethingElse");

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NotEqualToTests
		{
			[Test]
			[AutoArguments]
			public async Task WhenParamNameIsDifferent_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().NotEqualTo("somethingElse");

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenParamNameMatchesUnexpected_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().NotEqualTo("message");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have param name equal to "message",
					             but it had param name "message"

					             Param name:
					             message
					             """);
			}
		}

		public sealed class Tests
		{
			[Test]
			[AutoArguments]
			public async Task WhenExpectedIsNull_AndParamNameIsNull_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message);

				async Task Act()
					=> await That(subject).HasParamName(null);

				await That(Act).DoesNotThrow()
					.Because("the shorthand compares a null argument as null instead of skipping the check");
			}

			[Test]
			[AutoArguments]
			public async Task WhenExpectedIsNull_AndParamNameIsSet_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has param name equal to <null>,
					             but it had param name "message"

					             Param name:
					             message
					             """)
					.Because("the shorthand compares a null argument as null instead of skipping the check");
			}

			[Test]
			[AutoArguments]
			public async Task WhenParamNameIsDifferent_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName("somethingElse");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has param name equal to "somethingElse",
					             but it had param name "message", which differs at index 0:
					                ↓ (actual)
					               "message"
					               "somethingElse"
					                ↑ (expected)

					             Param name:
					             message
					             """)
					.Because("the shorthand renders exactly like the HasParamName().EqualTo(expected) continuation");
			}

			[Test]
			[AutoArguments]
			public async Task WhenParamNameMatchesExpected_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName("message");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ArgumentException? subject = null;

				async Task Act()
					=> await That(subject).HasParamName("message");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has param name equal to "message",
					             but it was <null>
					             """);
			}
		}
	}
}
