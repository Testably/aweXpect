namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public partial class Contains
	{
		public sealed class TwiceTests
		{
			[Fact]
			public async Task WhenExpectedStringOccursExactly1Times_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "word";

				async Task Act()
					=> await That(subject).Contains(expected).Twice();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "investigator";

				async Task Act()
					=> await That(subject).Contains(expected).Twice();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "investigator" exactly twice,
					             but it contained "investigator" once in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).Twice();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" exactly twice,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}
		}
	}
}
