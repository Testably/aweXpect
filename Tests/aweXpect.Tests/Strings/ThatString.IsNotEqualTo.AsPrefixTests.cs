namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class AsPrefixTests
		{
			[Theory]
			[InlineData("some text")]
			[InlineData(null)]
			public async Task WhenPrefixIsNull_ShouldThrowArgumentNullException(string? subject)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(null).AsPrefix();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' prefix cannot be null.").AsPrefix()
					.Because("a missing prefix is rejected before the subject is looked at");
			}

			[Fact]
			public async Task WhenSubjectDoesNotStartWithUnexpected_ShouldSucceed()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo("other").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo("text").AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "text",
					             but it was <null>
					             """)
					.Because("a null has no content to inspect, just as for DoesNotStartWith");
			}

			[Fact]
			public async Task WhenSubjectStartsWithUnexpected_ShouldFail()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo("some").AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "some",
					             but it was "some text"
					             """);
			}
		}
	}
}
