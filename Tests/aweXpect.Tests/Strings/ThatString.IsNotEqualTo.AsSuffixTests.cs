namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class AsSuffixTests
		{
			[Fact]
			public async Task WhenSubjectDoesNotEndWithUnexpected_ShouldSucceed()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo("other").AsSuffix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectEndsWithUnexpected_ShouldFail()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo("text").AsSuffix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with "text",
					             but it was "some text"
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo("text").AsSuffix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with "text",
					             but it was <null>
					             """)
					.Because("a null has no content to inspect, just as for DoesNotEndWith");
			}

			[Theory]
			[InlineData("some text")]
			[InlineData(null)]
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
