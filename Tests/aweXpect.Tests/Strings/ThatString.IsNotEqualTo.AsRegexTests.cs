namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class AsRegexTests
		{
			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenIgnoringCase_ShouldIgnoreCase(
				bool ignoreCase)
			{
				string subject = "some message";
				string pattern = ".*ME ME.*";

				async Task Act()
					=> await That(subject).IsNotEqualTo(pattern)
						.AsRegex().IgnoringCase(ignoreCase);

				await That(Act).Throws().OnlyIf(ignoreCase)
					.WithMessage("""
					             Expected that subject
					             does not match regex ".*ME ME.*" ignoring case,
					             but it was "some message"
					             """);
			}

			[Fact]
			public async Task WhenPatternDoesNotCompleteInTime_ShouldThrowArgumentException()
			{
				string subject = new('a', 30);

				async Task Act()
					=> await That(subject + "!").IsNotEqualTo("(a+)+$").AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"""The regex "(a+)+$" did not complete within 0:01. Simplify the pattern to avoid catastrophic backtracking.""")
					.AsPrefix().And
					.WithParamName("unexpected")
					.Because("a pattern that cannot be evaluated says nothing about the subject in either polarity");
			}

			[Fact]
			public async Task WhenPatternIsAnchoredToALineOfTheSubject_ShouldSucceed()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).IsNotEqualTo("^b$").AsRegex();

				await That(Act).DoesNotThrow()
					.Because("'^' and '$' bind to the complete subject, which is more than the matched line");
			}

			[Fact]
			public async Task WhenPatternIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "some message";

				async Task Act()
					=> await That(subject).IsNotEqualTo("").AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'unexpected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("unexpected")
					.Because("an empty pattern matches every subject, so the expectation could never succeed");
			}

			[Fact]
			public async Task WhenPatternIsNullAndSubjectIsNull_ShouldThrowArgumentNullException()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(null).AsRegex();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' regex pattern cannot be null.").AsPrefix()
					.Because("the missing pattern is a setup error that outranks the null subject");
			}

			[Fact]
			public async Task WhenPatternIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "some message";

				async Task Act()
					=> await That(subject).IsNotEqualTo(null).AsRegex();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' regex pattern cannot be null.").AsPrefix()
					.Because("a missing pattern matches no subject, so the negated expectation could never fail");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(".*").AsRegex();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not match regex ".*",
					             but it was <null>
					             """)
					.Because("a null has no content to inspect, just as for the dedicated negated expectations");
			}
		}
	}
}
