namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class DoesNotContain
	{
		public sealed class Tests
		{
			[Test]
			public async Task IgnoringCase_ShouldIncludeSettingInExpectationText()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "INVESTIGATOR";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "INVESTIGATOR" ignoring case,
					             but it contained "INVESTIGATOR" once in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			public async Task Using_ShouldIncludeComparerInExpectationText()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "InvEstIgAtOr";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "InvEstIgAtOr" using IgnoreCaseForVocalsComparer,
					             but it contained "InvEstIgAtOr" once in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			public async Task WhenEvaluatedForSeveralItems_ShouldNotShareTheNegation()
			{
				string[] subject = ["b", "a",];

				async Task Act()
					=> await That(subject).All().ComplyWith(it => it.DoesNotContain("a"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "a" for all items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               "a"
					             ]

					             Collection:
					             [
					               "b",
					               "a"
					             ]
					             """)
					.Because("the items share one quantifier, so the negation of one item must not leak into the next");
			}

			[Test]
			public async Task WhenPatternIsEmptyAfterTheIndentationIsIgnored_ShouldThrowArgumentException()
			{
				string subject = "some text";
				string unexpected = " ";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AsRegex().IgnoringIndentation();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'unexpected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("unexpected")
					.Because("the negated expectation receives the pattern as 'unexpected'");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContain("p").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "p" as wildcard,
					             but it was <null>
					             """);
			}

			[Test]
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

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "some text";
				string? unexpected = null;

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments("ab ", true)]
			[Arguments("a b", false)]
			public async Task WhenUnexpectedIsWhiteSpaceAndTrailingWhiteSpaceIsIgnored_ShouldOnlyFindItInsideTheSubject(
				string subject, bool expectSuccess)
			{
				async Task Act()
					=> await That(subject).DoesNotContain(" ").IgnoringTrailingWhiteSpace();

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain " " ignoring trailing whitespace,
					              but it contained " " once in {Formatter.Format(subject)}
					              """)
					.Because("the whitespace at the end of the subject is ignored, but not the one inside it");
			}

			[Test]
			public async Task WhenUnexpectedStringIsContained_ShouldFail()
			{
				string subject = "some text";
				string unexpected = "me";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "me",
					             but it contained "me" once in "some text"
					             """);
			}

			[Test]
			public async Task WhenUnexpectedStringIsNotContained_ShouldSucceed()
			{
				string subject = "some text";
				string unexpected = "not";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("ab ", true)]
			[Arguments("a b", false)]
			public async Task
				WhenUnexpectedWildcardIsWhiteSpaceAndTrailingWhiteSpaceIsIgnored_ShouldOnlyFindItInsideTheSubject(
					string subject, bool expectSuccess)
			{
				async Task Act()
					=> await That(subject).DoesNotContain(" ").AsWildcard().IgnoringTrailingWhiteSpace();

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              does not contain " " as wildcard ignoring trailing whitespace,
					              but it contained " " once in {Formatter.Format(subject)}
					              """)
					.Because("the whitespace at the end of the subject is ignored, but not the one inside it");
			}
		}

		public sealed class AtLeastTests
		{
			[Test]
			public async Task WhenExpectedStringOccursEnoughTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtLeast(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" fewer than 3 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtLeast(5);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedStringOccursShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "text that does not occur";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtLeast(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMinimumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtLeast(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("The minimum must not be negative.").AsPrefix().And
					.WithParamName("minimum");
			}
		}

		public sealed class AtMostTests
		{
			[Test]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtMost(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedStringOccursShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "text that does not occur";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtMost(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "text that does not occur" more than once,
					             but it did not contain "text that does not occur" in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			public async Task WhenExpectedStringSufficientlyFewTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtMost(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" more than 3 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			public async Task WhenMaximumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).AtMost(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("The maximum must not be negative.").AsPrefix().And
					.WithParamName("maximum");
			}
		}

		public sealed class BetweenTests
		{
			[Test]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(4).And(9);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(1).And(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedStringOccursSufficientTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(1).And(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "in" between 1 and 4 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			public async Task WhenMaximumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(1).And(-3);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("The maximum must not be negative.").AsPrefix().And
					.WithParamName("maximum");
			}

			[Test]
			public async Task WhenMinimumEqualsMaximum_ShouldBehaveLikeExactly()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(3).And(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "in" exactly 3 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			public async Task WhenMinimumIsGreaterThanMaximum_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(4).And(3);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Test]
			public async Task WhenMinimumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Between(-1).And(3);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("The minimum must not be negative.").AsPrefix().And
					.WithParamName("minimum");
			}
		}

		public sealed class ExactlyTests
		{
			[Test]
			public async Task
				WhenExpectedIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Exactly(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("The expected count must not be negative.").AsPrefix().And
					.WithParamName("expected");
			}

			[Test]
			public async Task WhenExpectedStringOccursCorrectlyOften_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Exactly(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "in" exactly 3 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Exactly(4);

				await That(Act).DoesNotThrow();
			}

			[Test]
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
			[Test]
			public async Task WhenExpectedStringDoesNotOccurAtAll_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "text that does not occur";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).LessThan(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "text that does not occur" at least once,
					             but it did not contain "text that does not occur" in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			public async Task WhenExpectedStringOccursEqualTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).LessThan(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedStringOccursMoreTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).LessThan(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedStringSufficientlyFewTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).LessThan(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" at least 4 times,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			public async Task WhenMaximumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).LessThan(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("The maximum must not be negative.").AsPrefix().And
					.WithParamName("maximum");
			}
		}

		public sealed class MoreThanTests
		{
			[Test]
			public async Task WhenExpectedStringDoesNotOccurAtAll_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "text that does not occur";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).MoreThan(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedStringOccursEnoughTimes_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).MoreThan(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" at most twice,
					             but it contained "in" 3 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			[Arguments(3)]
			[Arguments(5)]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldSucceed(int minimum)
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).MoreThan(minimum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMinimumIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "in";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).MoreThan(-1);

				await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
					.WithMessage("The minimum must not be negative.").AsPrefix().And
					.WithParamName("minimum");
			}

			[Test]
			public async Task WhenMinimumIsZero_ShouldReadLikeWithoutQuantifier()
			{
				string subject = "some text";
				string unexpected = "me";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).MoreThan(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "me",
					             but it contained "me" once in "some text"
					             """)
					.Because("more than zero occurrences are the same as any occurrence");
			}
		}

		public sealed class OnceTests
		{
			[Test]
			public async Task WhenExpectedStringOccursExactly1Times_ShouldFail()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "investigator";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Once();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "investigator" exactly once,
					             but it contained "investigator" once in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Test]
			public async Task WhenExpectedStringOccursFewerTimes_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string unexpected = "detective";

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected).Once();

				await That(Act).DoesNotThrow();
			}

			[Test]
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

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenUnexpectedStringIsMissing_ShouldFailWithThePositiveExpectation()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotContain("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "foo" at least once,
					             *
					             """).AsWildcard();
			}
		}
	}
}
