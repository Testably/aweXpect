namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public partial class Contains
	{
		public sealed class BetweenTests
		{
			[Fact]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).Between(4).And(9);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" between 4 and 9 times,
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
					=> await That(subject).Contains(expected).Between(1).And(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" between 1 and 2 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringOccursSufficientTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).Between(1).And(4);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMaximumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).Between(1).And(-3);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("*'maximum'*").AsWildcard().And
					.WithParamName("maximum");
			}

			[Fact]
			public async Task WhenMinimumEqualsMaximum_ShouldBehaveLikeExactly()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).Between(3).And(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMinimumIsGreaterThanMaximum_ShouldThrowArgumentException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).Between(4).And(3);

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithMessage("*'maximum'*greater*'minimum'*").AsWildcard();
			}

			[Fact]
			public async Task WhenMinimumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).Between(-1).And(3);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("*'minimum'*").AsWildcard().And
					.WithParamName("minimum");
			}
		}
	}
}
