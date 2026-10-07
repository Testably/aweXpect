namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class HasLength
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).HasLength(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to 0,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).HasLength(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to <null>,
					             but it had length 3
					             """);
			}

			[Test]
			public async Task WhenExpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).HasLength(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected length must not be negative.*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Test]
			[Arguments("", 1)]
			[Arguments("abc", 4)]
			[Arguments(" a b c ", 6)]
			public async Task WhenLengthDiffers_ShouldFail(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength(length);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has length equal to {length},
					              but it had length {subject.Length}
					              """);
			}

			[Test]
			[Arguments("", 0)]
			[Arguments("abc", 3)]
			[Arguments(" a b c ", 7)]
			public async Task WhenLengthMatches_ShouldSucceed(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength(length);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EqualToTests
		{
			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).HasLength().EqualTo(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to 0,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).HasLength().EqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected length must not be negative.*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Test]
			[Arguments("", 1)]
			[Arguments("abc", 4)]
			[Arguments(" a b c ", 6)]
			public async Task WhenLengthDiffers_ShouldFail(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength().EqualTo(length);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has length equal to {length},
					              but it had length {subject.Length}
					              """);
			}

			[Test]
			[Arguments("", 0)]
			[Arguments("abc", 3)]
			[Arguments(" a b c ", 7)]
			public async Task WhenLengthMatches_ShouldSucceed(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength().EqualTo(length);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "foo";
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has length greater than or equal to <null>,
					             but it had length 3
					             """);
			}

			[Test]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				string subject = "foo";
				int? expected = 2;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldFail()
			{
				string subject = "foo";
				int? expected = 4;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has length greater than or equal to {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}

			[Test]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				string subject = "foo";
				int expected = 3;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "foo";
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has length greater than <null>,
					             but it had length 3
					             """);
			}

			[Test]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				string subject = "foo";
				int? expected = 2;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldFail()
			{
				string subject = "foo";
				int? expected = 4;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has length greater than {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}

			[Test]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				string subject = "foo";
				int expected = 3;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has length greater than {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "foo";
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has length less than or equal to <null>,
					             but it had length 3
					             """);
			}

			[Test]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				string subject = "foo";
				int? expected = 2;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has length less than or equal to {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}

			[Test]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				string subject = "foo";
				int? expected = 4;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				string subject = "foo";
				int expected = 3;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LessThanTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "foo";
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has length less than <null>,
					             but it had length 3
					             """);
			}

			[Test]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				string subject = "foo";
				int? expected = 2;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has length less than {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}

			[Test]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				string subject = "foo";
				int? expected = 4;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				string subject = "foo";
				int expected = 3;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has length less than {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have length equal to 1,
					             but it was <null>
					             """);
			}

			[Test]
			[Arguments("", 1)]
			[Arguments("abc", 4)]
			[Arguments(" a b c ", 6)]
			public async Task WhenLengthDiffers_ShouldSucceed(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(length);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("", 0)]
			[Arguments("abc", 3)]
			[Arguments(" a b c ", 7)]
			public async Task WhenLengthMatches_ShouldFail(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(length);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have length equal to {length},
					              but it had length {length}
					              """);
			}

			[Test]
			public async Task WhenUnexpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The unexpected length must not be negative.*")
					.AsWildcard().And
					.WithParamName("unexpected");
			}
		}

		public sealed class NotGreaterThanTests
		{
			[Test]
			public async Task WhenCombinedWithAnd_ShouldNameBothComparisons()
			{
				string subject = "abcde";

				async Task Act()
					=> await That(subject).HasLength().NotGreaterThan(3).And.HasLength().NotLessThan(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have length greater than 3 and does not have length less than 1,
					             but it had length 5
					             """);
			}

			[Test]
			public async Task WhenCombinedWithOr_AndOnlyTheSecondHolds_ShouldSucceed()
			{
				string subject = "abcde";

				async Task Act()
					=> await That(subject).HasLength().NotGreaterThan(3).Or.HasLength().NotLessThan(4);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).HasLength().NotGreaterThan(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The unexpected length must not be negative.*")
					.AsWildcard().And
					.WithParamName("unexpected");
			}

			[Test]
			public async Task WhenLengthIsGreater_ShouldFail()
			{
				string subject = "abcde";

				async Task Act()
					=> await That(subject).HasLength().NotGreaterThan(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have length greater than 3,
					             but it had length 5
					             """);
			}

			[Test]
			public async Task WhenLengthIsNotGreater_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).HasLength().NotGreaterThan(3);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasLength(null));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLengthDiffers_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasLength(4));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLengthMatches_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasLength(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have length equal to 3,
					             but it had length 3
					             """);
			}

			[Test]
			public async Task WhenLengthMatchesContinuation_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasLength().GreaterThan(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have length greater than 2,
					             but it had length 3
					             """);
			}
		}
	}
}
