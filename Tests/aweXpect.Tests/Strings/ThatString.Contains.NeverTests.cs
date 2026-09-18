namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public partial class Contains
	{
		public sealed class NeverTests
		{
			[Fact]
			public async Task WhenExpectedStringDoesNotOccur_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "detective";

				async Task Act()
					=> await That(subject).Contains(expected).Never();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "investigator";

				async Task Act()
					=> await That(subject).Contains(expected).Never();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "investigator",
					             but it contained "investigator" once in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}
		}
	}
}
