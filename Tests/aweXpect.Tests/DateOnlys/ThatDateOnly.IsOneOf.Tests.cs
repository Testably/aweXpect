#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed class IsOneOf
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateOnly subject = CurrentTime();
				DateOnly[] expected = [];

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				DateOnly subject = CurrentTime();
				DateOnly[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedOnlyContainsNull_ShouldFail()
			{
				DateOnly subject = CurrentTime();
				IEnumerable<DateOnly?> expected = [null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}

					              Expected values:
					              [<null>]
					              """);
			}

			[Test]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateOnly subject = CurrentTime();
				DateOnly?[] expected = [];

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				DateOnly subject = CurrentTime();
				DateOnly?[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsContained_ShouldSucceed()
			{
				DateOnly subject = CurrentTime();
				IEnumerable<DateOnly> expected = [LaterTime(), subject, EarlierTime(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				DateOnly subject = CurrentTime();
				DateOnly[] expected = [LaterTime(), EarlierTime(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}, which differs by -1 day from the closest value

					              Expected values:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
			{
				DateOnly subject = CurrentTime();
				DateOnly[] expected = [CurrentTime(), LaterTime(),];

				object Act()
					=> That(subject).IsOneOf(expected)
						.Within(23.Hours());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("tolerance").And
					.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
					.Because("a date has no time of day, so the remainder is rejected as soon as it is specified instead of when the expectation is awaited");
			}

			[Test]
			[Arguments(3, 2, true)]
			[Arguments(5, 3, true)]
			[Arguments(2, 2, false)]
			[Arguments(0, 2, false)]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
				int actualDifference, int tolerance, bool expectToThrow)
			{
				DateOnly subject = EarlierTime(actualDifference);
				DateOnly[] expected = [CurrentTime(), LaterTime(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected)
						.Within(tolerance.Days())
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.OnlyIf(expectToThrow)
					.WithMessage($"""
					              Expected that subject
					              is one of expected ± {tolerance} days, because we want to test the failure,
					              but it was {Formatter.Format(subject)}, which differs by -{actualDifference} days from the closest value

					              Expected values:
					              {Formatter.Format(expected)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsContained_ShouldFail()
			{
				DateOnly subject = new(2010, 11, 12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(new DateOnly(2010, 11, 12), new DateOnly(2010, 11, 13)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [2010-11-12, 2010-11-13],
					             but it was 2010-11-12
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNotContained_ShouldSucceed()
			{
				DateOnly subject = new(2010, 11, 12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(new DateOnly(2010, 11, 13), new DateOnly(2010, 11, 14)));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
