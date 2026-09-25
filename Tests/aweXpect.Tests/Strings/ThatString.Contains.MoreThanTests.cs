namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public partial class Contains
	{
		public sealed class MoreThanTests
		{
			[Fact]
			public async Task WhenExpectedStringDoesNotOccurAtAll_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "text that does not occur";

				async Task Act()
					=> await That(subject).Contains(expected).MoreThan().Once();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "text that does not occur" more than once,
					             but it did not contain "text that does not occur" in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringOccursEnoughTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).MoreThan(2);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(3)]
			[InlineData(5)]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldFail(int minimum)
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).MoreThan(minimum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains "in" more than {minimum} times,
					              but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					              """);
			}

			[Fact]
			public async Task WhenMinimumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).MoreThan(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("The minimum must not be negative.").AsPrefix().And
					.WithParamName("minimum");
			}
		}
	}
}
