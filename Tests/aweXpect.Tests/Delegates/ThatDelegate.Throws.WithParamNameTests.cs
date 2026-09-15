namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public class WithParamNameTests
		{
			[Theory]
			[AutoData]
			public async Task WhenExpectedIsNull_AndParamNameIsEmpty_ShouldSucceed(string message)
			{
				ArgumentException exception = new(message);
				void Delegate() => throw exception;

				async Task Act()
					=> await That(Delegate).Throws<ArgumentException>()
						.WithParamName(null);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenExpectedIsNull_ShouldSucceed(string message)
			{
				ArgumentException exception = new(message, nameof(message));
				void Delegate() => throw exception;

				async Task Act()
					=> await That(Delegate).Throws<ArgumentException>()
						.WithParamName(null);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenParamNameIsDifferent_ShouldFail(string message)
			{
				ArgumentException exception = new(message, nameof(message));
				void Delegate() => throw exception;

				async Task Act()
					=> await That(Delegate).Throws<ArgumentException>()
						.WithParamName("somethingElse");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             throws an ArgumentException with ParamName "somethingElse",
					             but it had ParamName "message"
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenParamNameMatchesExpected_ShouldSucceed(string message)
			{
				ArgumentException exception = new(message, nameof(message));
				void Delegate() => throw exception;

				async Task Act()
					=> await That(Delegate).Throws<ArgumentException>()
						.WithParamName("message");

				await That(Act).DoesNotThrow();
			}

			public sealed class ContainingTests
			{
				[Theory]
				[AutoData]
				public async Task WhenParamNameContainsExpected_ShouldSucceed(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().Containing("essag");

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[AutoData]
				public async Task WhenParamNameDoesNotContainExpected_ShouldFail(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().Containing("somethingElse");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an ArgumentException with ParamName containing "somethingElse",
						             but it was "message" with a length of 7 which is shorter than the expected length of 13
						             """);
				}
			}

			public sealed class EqualToTests
			{
				[Theory]
				[AutoData]
				public async Task WhenAwaited_ShouldReturnThrownException(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					ArgumentException result = await That(Delegate).Throws<ArgumentException>()
						.WithParamName().EqualTo("message");

					await That(result).IsSameAs(exception)
						.Because("the continuation keeps the narrowed exception type of the result");
				}

				[Theory]
				[AutoData]
				public async Task WhenParamNameIsDifferent_ShouldFail(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().EqualTo("somethingElse");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an ArgumentException with ParamName equal to "somethingElse",
						             but it was "message" which differs at index 0:
						                ↓ (actual)
						               "message"
						               "somethingElse"
						                ↑ (expected)
						             """);
				}

				[Theory]
				[AutoData]
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

			public sealed class NotEqualToTests
			{
				[Theory]
				[AutoData]
				public async Task WhenParamNameIsDifferent_ShouldSucceed(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().NotEqualTo("somethingElse");

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[AutoData]
				public async Task WhenParamNameMatchesUnexpected_ShouldFail(string message)
				{
					ArgumentException exception = new(message, nameof(message));
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<ArgumentException>()
							.WithParamName().NotEqualTo("message");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an ArgumentException with ParamName not equal to "message",
						             but it was "message"
						             """);
				}
			}
		}
	}
}
