namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class DoesNotStartWith
	{
		public sealed class Tests
		{
			[Fact]
			public async Task IgnoringCase_WhenSubjectDoesNotStartWithWithExpected_ShouldSucceed()
			{
				string subject = "some text";
				string expected = "text";

				async Task Act()
					=> await That(subject).DoesNotStartWith(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task IgnoringCase_WhenSubjectDoesStartWithExpected_ShouldFail()
			{
				string subject = "some text";
				string expected = "some";

				async Task Act()
					=> await That(subject).DoesNotStartWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "some",
					             but it was "some text"
					             
					             Actual:
					             some text
					             
					             Expected:
					             some
					             """);
			}

			[Fact]
			public async Task
				Using_WhenSubjectDoesNotStartWithWithExpected_ShouldSucceed()
			{
				string subject = "some arbitrary text";
				string expected = "SOME";

				async Task Act()
					=> await That(subject).DoesNotStartWith(expected)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task
				Using_WhenSubjectDoesStartWithExpected_ShouldFail()
			{
				string subject = "some arbitrary text";
				string expected = "sOmE";

				async Task Act()
					=> await That(subject).DoesNotStartWith(expected)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "sOmE" using IgnoreCaseForVocalsComparer,
					             but it was "some arbitrary text"
					             
					             Actual:
					             some arbitrary text
					             
					             Expected:
					             sOmE
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "text";
				string? expected = null;

				async Task Act()
					=> await That(subject).DoesNotStartWith(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectDoesNotStartWithExpected_ShouldSucceed()
			{
				string subject = "some text";
				string expected = "TEXT";

				async Task Act()
					=> await That(subject).DoesNotStartWith(expected).IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectDoesStartWithExpected_ShouldFail()
			{
				string subject = "some text";
				string expected = "SOME";

				async Task Act()
					=> await That(subject).DoesNotStartWith(expected).IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "SOME" ignoring case,
					             but it was "some text"
					             
					             Actual:
					             some text
					             
					             Expected:
					             SOME
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsEqualToExpected_ShouldFail()
			{
				string subject = "some text";
				string expected = subject;

				async Task Act()
					=> await That(subject).DoesNotStartWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "some text",
					             but it was "some text"
					             
					             Actual:
					             some text
					             
					             Expected:
					             some text
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				string expected = "text";

				async Task Act()
					=> await That(subject).DoesNotStartWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "text",
					             but it was <null>
					             """);
			}
		}
	}
}
