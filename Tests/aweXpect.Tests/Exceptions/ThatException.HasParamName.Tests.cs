namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public class HasParamName
	{
		public sealed class ContainingTests
		{
			[Theory]
			[AutoData]
			public async Task WhenParamNameContainsExpected_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().Containing("essag");

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenParamNameDoesNotContainExpected_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().Containing("somethingElse");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ParamName containing "somethingElse",
					             but it was "message" with a length of 7 which is shorter than the expected length of 13
					             """);
			}
		}

		public sealed class EqualToTests
		{
			[Theory]
			[AutoData]
			public async Task WhenParamNameIsDifferent_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().EqualTo("somethingElse");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ParamName equal to "somethingElse",
					             but it was "message" which differs at index 0:
					                ↓ (actual)
					               "message"
					               "somethingElse"
					                ↑ (expected)
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenParamNameMatchesExpected_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().EqualTo("message");

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenParamNameMatchesPrefix_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().EqualTo("mes").AsPrefix();

				await That(Act).DoesNotThrow()
					.Because("the continuation exposes the As… family of StringEqualityTypeResult");
			}

			[Theory]
			[AutoData]
			public async Task WhenSubjectHasNoParamName_ShouldFail(string message)
			{
				ArgumentException subject = new(message);

				async Task Act()
					=> await That(subject).HasParamName().EqualTo("message");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ParamName equal to "message",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ArgumentException? subject = null;

				async Task Act()
					=> await That(subject).HasParamName().EqualTo("message");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ParamName equal to "message",
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[AutoData]
			public async Task WhenExpectedIsNull_AndParamNameIsEmpty_ShouldFail(string message)
			{
				ArgumentException subject = new(message);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.HasParamName(null));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a ParamName,
					             but it had
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenExpectedIsNull_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.HasParamName(null));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a ParamName,
					             but it had
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenParamNameIsDifferent_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.HasParamName("somethingElse"));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenParamNameMatchesExpected_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.HasParamName("message"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have ParamName "message",
					             but it had
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ArgumentException? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.HasParamName("message"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have ParamName "message",
					             but it was <null>
					             """);
			}
		}

		public sealed class NotContainingTests
		{
			[Theory]
			[AutoData]
			public async Task WhenParamNameContainsUnexpected_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().NotContaining("essag");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ParamName not containing "essag",
					             but it was "message"
					             """);
			}

			[Theory]
			[AutoData]
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
			[Theory]
			[AutoData]
			public async Task WhenParamNameIsDifferent_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().NotEqualTo("somethingElse");

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenParamNameMatchesUnexpected_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName().NotEqualTo("message");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ParamName not equal to "message",
					             but it was "message"
					             """);
			}
		}

		public sealed class Tests
		{
			[Theory]
			[AutoData]
			public async Task WhenExpectedIsNull_AndParamNameIsEmpty_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message);

				async Task Act()
					=> await That(subject).HasParamName(null);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenExpectedIsNull_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName(null);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenParamNameIsDifferent_ShouldFail(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName("somethingElse");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ParamName "somethingElse",
					             but it had ParamName "message"
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenParamNameMatchesExpected_ShouldSucceed(string message)
			{
				ArgumentException subject = new(message, nameof(message));

				async Task Act()
					=> await That(subject).HasParamName("message");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ArgumentException? subject = null;

				async Task Act()
					=> await That(subject).HasParamName("message");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ParamName "message",
					             but it was <null>
					             """);
			}
		}
	}
}
