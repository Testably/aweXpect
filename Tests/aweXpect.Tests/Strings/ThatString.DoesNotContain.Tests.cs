namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class DoesNotContain
	{
		public sealed class Tests
		{
			[Fact]
			public async Task IgnoringCase_ShouldIncludeSettingInExpectationText()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "INVESTIGATOR";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "INVESTIGATOR" ignoring case,
					             but it contained "INVESTIGATOR" once in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             INVESTIGATOR
					             """);
			}

			[Fact]
			public async Task Using_ShouldIncludeComparerInExpectationText()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "InvEstIgAtOr";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "InvEstIgAtOr" using IgnoreCaseForVocalsComparer,
					             but it contained "InvEstIgAtOr" once in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             InvEstIgAtOr
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "some text";
				string unexpected = "";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'unexpected' string cannot be empty.").AsPrefix().And
					.WithParamName("unexpected");
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "some text";
				string? unexpected = null;

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenUnexpectedStringIsContained_ShouldFail()
			{
				string subject = "some text";
				string unexpected = "me";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "me",
					             but it contained "me" once in "some text"
					             
					             Actual:
					             some text
					             
					             Expected:
					             me
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedStringIsNotContained_ShouldSucceed()
			{
				string subject = "some text";
				string unexpected = "not";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AtLeastTests
		{
			[Fact]
			public async Task WhenExpectedStringOccursEnoughTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtLeast(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "in" at least 3 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             in
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtLeast(5);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "text that does not occur";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtLeast(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMinimumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtLeast(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("*'minimum'*").AsWildcard().And
					.WithParamName("minimum");
			}
		}

		public sealed class AtMostTests
		{
			[Fact]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtMost(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "text that does not occur";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtMost(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "text that does not occur" at most once,
					             but it did not contain "text that does not occur" in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             text that does not occur
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringSufficientlyFewTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtMost(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "in" at most 3 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             in
					             """);
			}

			[Fact]
			public async Task WhenMaximumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtMost(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("*'maximum'*").AsWildcard().And
					.WithParamName("maximum");
			}
		}

		public sealed class BetweenTests
		{
			[Fact]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(4).And(9);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(1).And(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursSufficientTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(1).And(4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "in" between 1 and 4 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             in
					             """);
			}

			[Fact]
			public async Task WhenMaximumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(1).And(-3);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("*'maximum'*").AsWildcard().And
					.WithParamName("maximum");
			}

			[Fact]
			public async Task WhenMinimumEqualsMaximum_ShouldBehaveLikeExactly()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(3).And(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "in" exactly 3 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             in
					             """);
			}

			[Fact]
			public async Task WhenMinimumIsGreaterThanMaximum_ShouldThrowArgumentException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(4).And(3);

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithMessage("*'maximum'*greater*'minimum'*").AsWildcard();
			}

			[Fact]
			public async Task WhenMinimumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(-1).And(3);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("*'minimum'*").AsWildcard().And
					.WithParamName("minimum");
			}
		}

		public sealed class ExactlyTests
		{
			[Fact]
			public async Task
				WhenExpectedIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Exactly(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("*'expected'*").AsWildcard().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenExpectedStringOccursCorrectlyOften_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Exactly(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "in" exactly 3 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             in
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Exactly(4);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Exactly(2);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LessThanTests
		{
			[Fact]
			public async Task WhenExpectedStringDoesNotOccurAtAll_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "text that does not occur";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).LessThan(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "text that does not occur" less than once,
					             but it did not contain "text that does not occur" in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             text that does not occur
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringOccursEqualTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).LessThan(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).LessThan(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringSufficientlyFewTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).LessThan(4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "in" less than 4 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             in
					             """);
			}

			[Fact]
			public async Task WhenMaximumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).LessThan(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("*'maximum'*").AsWildcard().And
					.WithParamName("maximum");
			}
		}

		public sealed class MoreThanTests
		{
			[Fact]
			public async Task WhenExpectedStringDoesNotOccurAtAll_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "text that does not occur";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).MoreThan(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursEnoughTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).MoreThan(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not contain "in" more than twice,
					              but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					              
					              Actual:
					              In this text in between the word an investigator should find the word 'IN' multiple times.
					              
					              Expected:
					              in
					              """);
			}

			[Theory]
			[InlineData(3)]
			[InlineData(5)]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldSucceed(int minimum)
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).MoreThan(minimum);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMinimumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).MoreThan(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("*'minimum'*").AsWildcard().And
					.WithParamName("minimum");
			}
		}

		public sealed class OnceTests
		{
			[Fact]
			public async Task WhenExpectedStringOccursExactly1Times_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "investigator";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Once();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "investigator" exactly once,
					             but it contained "investigator" once in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             investigator
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "detective";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Once();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "word";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Once();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
