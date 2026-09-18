namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public partial class Contains
	{
		public sealed class OnceTests
		{
			[Fact]
			public async Task WhenExpectedStringOccursExactly1Times_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "investigator";

				async Task Act()
					=> await That(subject).Contains(expected).Once();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "detective";

				async Task Act()
					=> await That(subject).Contains(expected).Once();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "detective" exactly once,
					             but it did not contain "detective" in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "word";

				async Task Act()
					=> await That(subject).Contains(expected).Once();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "word" exactly once,
					             but it contained "word" twice in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}
		}
	}
}
