namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class AsSuffixTests
		{
			[Test]
			public async Task WhenSubjectDoesNotEndWithUnexpected_ShouldSucceed()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo("other").AsSuffix();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectEndsWithUnexpected_ShouldFail()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo("text").AsSuffix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not end with "text",
					             but it was "some text"
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo("text").AsSuffix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not end with "text",
					             but it was <null>
					             """)
					.Because("a null has no content to inspect, just as for DoesNotEndWith");
			}

			[Test]
			[Arguments("some text")]
			[Arguments(null)]
			public async Task WhenSuffixIsNull_ShouldThrowArgumentNullException(string? subject)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(null).AsSuffix();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' suffix cannot be null.").AsPrefix()
					.Because("a missing suffix is rejected before the subject is looked at");
			}
		}
	}
}
