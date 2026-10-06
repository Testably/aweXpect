#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed partial class Nullable
	{
		public sealed class IsNotOneOf
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly?[] values = [LaterTime(), subject, EarlierTime(),];
					IEnumerable<DateOnly?> expected = Factory.GetSingleUseEnumerable(values);

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of expected,
						              but it was {Formatter.Format(subject)}

						              Unexpected values:
						              {Formatter.Format(values)}
						              """)
						.Because("the empty check must not consume the values needed for the comparison and the message");
				}

				[Test]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateOnly? subject = CurrentTime();
					DateOnly[] expected = [];

					object Act()
						=> That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
						.Because("an empty set is rejected when the expectation is built, before it is awaited");
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateOnly? subject = CurrentTime();
					DateOnly[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					IEnumerable<DateOnly?> expected = [null,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateOnly? subject = CurrentTime();
					DateOnly?[] expected = [];

					object Act()
						=> That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
						.Because("an empty set is rejected when the expectation is built, before it is awaited");
				}

				[Test]
				public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateOnly? subject = CurrentTime();
					DateOnly?[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenSubjectIsContained_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					IEnumerable<DateOnly?> expected = [LaterTime(), subject, EarlierTime(),];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of expected,
						              but it was {Formatter.Format(subject)}

						              Unexpected values:
						              {Formatter.Format(expected)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsDifferent_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly[] expected = [LaterTime()!.Value, EarlierTime()!.Value,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldSucceed()
				{
					DateOnly? subject = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(CurrentTime(), LaterTime());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNullAndExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateOnly? subject = null;
					DateOnly[] expected = [];

					object Act()
						=> That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Test]
				public async Task WhenSubjectIsNullAndExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateOnly? subject = null;
					DateOnly[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Test]
				public async Task WhenSubjectIsNullAndNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateOnly? subject = null;
					DateOnly?[] expected = [];

					object Act()
						=> That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Test]
				public async Task WhenSubjectIsNullAndNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateOnly? subject = null;
					DateOnly?[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Test]
				public async Task WhenSubjectIsNullAndUnexpectedCanOnlyBeEnumeratedOnce_ShouldFail()
				{
					DateOnly? subject = null;
					DateOnly?[] values = [CurrentTime(), null,];
					IEnumerable<DateOnly?> expected = Factory.GetSingleUseEnumerable(values);

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of expected,
						              but it was <null>

						              Unexpected values:
						              {Formatter.Format(values)}
						              """)
						.Because("the empty check must not consume the values needed for the comparison and the message");
				}

				[Test]
				public async Task WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
				{
					DateOnly? subject = null;
					IEnumerable<DateOnly?> expected = [CurrentTime(), null,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of expected,
						              but it was <null>

						              Unexpected values:
						              {Formatter.Format(expected)}
						              """);
				}

				[Test]
				public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();
					DateOnly?[] unexpected = [CurrentTime(), LaterTime(),];

					object Act()
						=> That(subject).IsNotOneOf(unexpected)
							.Within(23.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified instead of when the expectation is awaited");
				}

				[Test]
				[Arguments(3, 2, false)]
				[Arguments(5, 3, false)]
				[Arguments(2, 2, true)]
				[Arguments(0, 2, true)]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
					int actualDifference, int tolerance, bool expectToThrow)
				{
					DateOnly? subject = EarlierTime(actualDifference);
					DateOnly?[] expected = [CurrentTime(), LaterTime(),];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected)
							.Within(tolerance.Days())
							.Because("we want to test the failure");

					string difference = actualDifference == 0
						? ""
						: $", which differs by -{actualDifference} days from the closest value";

					await That(Act).Throws<FailException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(expected)} ± {tolerance} days, because we want to test the failure,
						              but it was {Formatter.Format(subject)}{difference}
						              """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenSubjectIsContained_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotOneOf(new DateOnly(2010, 11, 12), new DateOnly(2010, 11, 13)));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNotContained_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotOneOf(new DateOnly(2010, 11, 13), new DateOnly(2010, 11, 14)));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is one of [2010-11-13, 2010-11-14],
						             but it was 2010-11-12, which differs by -1 day from the closest value
						             """);
				}
			}
		}
	}
}
#endif
