namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class AsRegexTests
		{
			[Theory]
			[InlineData("some message", ".*me me.*", true)]
			[InlineData("some message", ".*ME ME.*", false)]
			public async Task ShouldDefaultToCaseSensitiveMatch(
				string subject, string pattern, bool expectMatch)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsRegex();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches regex {Formatter.Format(pattern)},
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (regex pattern)

					              Actual:
					              some message
					              
					              Expected:
					              .*ME ME.*
					              """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenIgnoringCase_ShouldIgnoreCase(
				bool ignoreCase)
			{
				string subject = "some message";
				string pattern = ".*ME ME.*";

				async Task Act()
					=> await That(subject).IsEqualTo(pattern)
						.AsRegex().IgnoringCase(ignoreCase);

				await That(Act).Throws().OnlyIf(!ignoreCase)
					.WithMessage("""
					             Expected that subject
					             matches regex ".*ME ME.*",
					             but it did not match:
					               ↓ (actual)
					               "some message"
					               ".*ME ME.*"
					               ↑ (regex pattern)

					             Actual:
					             some message
					             
					             Expected:
					             .*ME ME.*
					             """);
			}
		}
	}
}
