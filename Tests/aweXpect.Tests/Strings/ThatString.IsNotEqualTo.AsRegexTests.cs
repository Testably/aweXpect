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
					             does not match regex ".*ME ME.*",
					             but it was "some message"
					             """);
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
		}
	}
}
