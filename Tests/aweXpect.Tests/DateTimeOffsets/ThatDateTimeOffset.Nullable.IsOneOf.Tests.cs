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
				[Fact]
				public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset?[] values = [LaterTime(), EarlierTime(),];
					IEnumerable<DateTimeOffset?> expected = Factory.GetSingleUseEnumerable(values);

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(values)},
						              but it was {Formatter.Format(subject)}
						              """)
						.Because("the empty check must not consume the values needed for the comparison and the message");
				}

				[Fact]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset[] expected = [];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!");
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The expected cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenExpectedOnlyContainsNull_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					IEnumerable<DateTimeOffset?> expected = [null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of [<null>],
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset?[] expected = [];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!");
				}

				[Fact]
				public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset?[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The expected cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenSubjectIsContained_ShouldSucceed()
				{
					DateTimeOffset? subject = CurrentTime();
					IEnumerable<DateTimeOffset?> expected = [LaterTime(), subject, EarlierTime(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset[] expected = [LaterTime()!.Value, EarlierTime()!.Value,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					IEnumerable<DateTimeOffset?> expected = [CurrentTime(), LaterTime(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Fact]
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

				[Fact]
				public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
				{
					DateTimeOffset? subject = null;
					IEnumerable<DateTimeOffset?> expected = [CurrentTime(), null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNullAndExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTimeOffset? subject = null;
					DateTimeOffset[] expected = [];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!")
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTimeOffset? subject = null;
					DateTimeOffset[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The expected cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTimeOffset? subject = null;
					DateTimeOffset?[] expected = [];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!")
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTimeOffset? subject = null;
					DateTimeOffset?[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The expected cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Theory]
				[InlineData(3, 2, true)]
				[InlineData(5, 3, true)]
				[InlineData(2, 2, false)]
				[InlineData(0, 2, false)]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
					int actualDifference, int tolerance, bool expectToThrow)
				{
					DateTimeOffset? subject = EarlierTime(actualDifference);
					DateTimeOffset?[] expected = [CurrentTime(), LaterTime(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected)
							.Within(tolerance.Seconds())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)} ± 0:0{tolerance}, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}
			}
		}
	}
}
