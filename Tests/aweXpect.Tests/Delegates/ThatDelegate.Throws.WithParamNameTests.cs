namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public class WithParamNameTests
		{
			public sealed class ContainingTests
			{
				[Test]
				[AutoArguments]
				public async Task WhenParamNameContainsExpected_ShouldSucceed(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().Containing("essag");

					await That(Act).DoesNotThrow();
				}

				[Test]
				[AutoArguments]
				public async Task WhenParamNameDoesNotContainExpected_ShouldFail(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().Containing("somethingElse");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an ArgumentException with param name containing "somethingElse",
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
				public async Task WhenAwaited_ShouldReturnThrownException(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					ArgumentException result = await That(Delegate).Throws<ArgumentException>()
						.WithParamName().EqualTo("message");

					await That(result).IsSameAs(exception)
						.Because("the continuation keeps the narrowed exception type of the result");
				}

				[Test]
				[AutoArguments]
				public async Task WhenParamNameIsDifferent_ShouldFail(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().EqualTo("somethingElse");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an ArgumentException with param name equal to "somethingElse",
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
				public async Task WhenParamNameMatchesPrefix_ShouldSucceed(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().EqualTo("mes").AsPrefix();

					await That(Act).DoesNotThrow()
						.Because("the continuation exposes the As… family of StringEqualityTypeResult");
				}
			}

			public sealed class NotContainingTests
			{
				[Test]
				[AutoArguments]
				public async Task WhenParamNameContainsUnexpected_ShouldFail(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().NotContaining("essag");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an ArgumentException with param name not containing "essag",
						             but it had param name "message"

						             Param name:
						             message
						             """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenParamNameDoesNotContainUnexpected_ShouldSucceed(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().NotContaining("somethingElse");

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NotEqualToTests
			{
				[Test]
				[AutoArguments]
				public async Task WhenParamNameIsDifferent_ShouldSucceed(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().NotEqualTo("somethingElse");

					await That(Act).DoesNotThrow();
				}

				[Test]
				[AutoArguments]
				public async Task WhenParamNameMatchesUnexpected_ShouldFail(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().NotEqualTo("message");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an ArgumentException with param name not equal to "message",
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
					ArgumentException exception = new(message);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName(null);

					await That(Act).DoesNotThrow()
						.Because("the shorthand compares a null argument as null instead of skipping the check");
				}

				[Test]
				[AutoArguments]
				public async Task WhenExpectedIsNull_AndParamNameIsSet_ShouldFail(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName(null);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an ArgumentException with param name equal to <null>,
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
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName("somethingElse");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an ArgumentException with param name equal to "somethingElse",
						             but it had param name "message", which differs at index 0:
						                ↓ (actual)
						               "message"
						               "somethingElse"
						                ↑ (expected)

						             Param name:
						             message
						             """)
						.Because("the shorthand renders exactly like the WithParamName().EqualTo(expected) continuation");
				}

				[Test]
				[AutoArguments]
				public async Task WhenParamNameMatchesExpected_ShouldSucceed(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName("message");

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
