using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed partial class Nullable
	{
		public sealed class IsOneOf
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset?[] values = [LaterTime(), EarlierTime(),];
					IEnumerable<DateTimeOffset?> expected = Factory.GetSingleUseEnumerable(values);

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is one of expected,
						              but it was {Formatter.Format(subject)}, which differs by -0:01 from the closest value

						              Expected values:
						              {Formatter.Format(values)}
						              """)
						.Because("the empty check must not consume the values needed for the comparison and the message");
				}

				[Test]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset[] expected = [];

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
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenExpectedOnlyContainsNull_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					IEnumerable<DateTimeOffset?> expected = [null,];

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
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset?[] expected = [];

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
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset?[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenSubjectIsContained_ShouldSucceed()
				{
					DateTimeOffset? subject = CurrentTime();
					IEnumerable<DateTimeOffset?> expected = [LaterTime(), subject, EarlierTime(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset[] expected = [LaterTime()!.Value, EarlierTime()!.Value,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is one of expected,
						              but it was {Formatter.Format(subject)}, which differs by -0:01 from the closest value

						              Expected values:
						              {Formatter.Format(expected)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					IEnumerable<DateTimeOffset?> expected = [CurrentTime(), LaterTime(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is one of expected,
						              but it was <null>

						              Expected values:
						              {Formatter.Format(expected)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNullAndExpectedCanOnlyBeEnumeratedOnce_ShouldSucceed()
				{
					DateTimeOffset? subject = null;
					IEnumerable<DateTimeOffset?> expected =
						Factory.GetSingleUseEnumerable<DateTimeOffset?>(CurrentTime(), null);

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow()
						.Because("the empty check must not consume the value needed for the comparison");
				}

				[Test]
				public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
				{
					DateTimeOffset? subject = null;
					IEnumerable<DateTimeOffset?> expected = [CurrentTime(), null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNullAndExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTimeOffset? subject = null;
					DateTimeOffset[] expected = [];

					object Act()
						=> That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Test]
				public async Task WhenSubjectIsNullAndExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTimeOffset? subject = null;
					DateTimeOffset[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Test]
				public async Task WhenSubjectIsNullAndNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTimeOffset? subject = null;
					DateTimeOffset?[] expected = [];

					object Act()
						=> That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Test]
				public async Task WhenSubjectIsNullAndNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTimeOffset? subject = null;
					DateTimeOffset?[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Test]
				[Arguments(3, 2, true)]
				[Arguments(5, 3, true)]
				[Arguments(2, 2, false)]
				[Arguments(0, 2, false)]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
					int actualDifference, int tolerance, bool expectToThrow)
				{
					DateTimeOffset? subject = EarlierTime(actualDifference);
					DateTimeOffset?[] expected = [CurrentTime(), LaterTime(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected)
							.Within(tolerance.Seconds())
							.Because("we want to test the failure");

					await That(Act).Throws<FailException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)} ± 0:0{tolerance}, because we want to test the failure,
						              but it was {Formatter.Format(subject)}, which differs by -0:0{actualDifference} from the closest value
						              """);
				}
			}
		}
	}
}
