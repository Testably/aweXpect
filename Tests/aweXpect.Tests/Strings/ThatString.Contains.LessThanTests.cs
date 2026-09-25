namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public partial class Contains
	{
		public sealed class LessThanTests
		{
			[Fact]
			public async Task WhenExpectedStringDoesNotOccurAtAll_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "text that does not occur";

				async Task Act()
					=> await That(subject).Contains(expected).LessThan().Once();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursEqualTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).LessThan(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" less than 3 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).LessThan(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" less than once,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringSufficientlyFewTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).LessThan(4);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMaximumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).LessThan(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("The maximum must not be negative.").AsPrefix().And
					.WithParamName("maximum");
			}
		}
	}
}
