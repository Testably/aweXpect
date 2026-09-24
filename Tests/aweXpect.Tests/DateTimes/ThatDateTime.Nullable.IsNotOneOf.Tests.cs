using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class IsNotOneOf
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime?[] values = [LaterTime(), subject, EarlierTime(),];
					IEnumerable<DateTime?> expected = Factory.GetSingleUseEnumerable(values);

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(values)},
						              but it was {Formatter.Format(subject)}
						              """)
						.Because("the empty check must not consume the values needed for the comparison and the message");
				}

				[Fact]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTime? subject = CurrentTime();
					DateTime[] expected = [];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!");
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTime? subject = CurrentTime();
					DateTime[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The unexpected cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					IEnumerable<DateTime?> expected = [null,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenKindIsIncompatibleButAnotherValueMatches_ShouldFail()
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, DateTimeKind.Utc);
					DateTime?[] unexpected =
					[
						DateTime.SpecifyKind(CurrentTime()!.Value, DateTimeKind.Local),
						DateTime.SpecifyKind(CurrentTime()!.Value, DateTimeKind.Utc),
					];

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Utc)]
				public async Task WhenKindIsIncompatible_ShouldSucceed(
					DateTimeKind subjectKind, DateTimeKind unexpectedKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime?[] unexpected = [DateTime.SpecifyKind(CurrentTime()!.Value, unexpectedKind),];

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Unspecified)]
				[InlineData(DateTimeKind.Unspecified, DateTimeKind.Utc)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Unspecified)]
				[InlineData(DateTimeKind.Unspecified, DateTimeKind.Local)]
				public async Task WhenKindIsUnspecified_ShouldFail(
					DateTimeKind subjectKind, DateTimeKind unexpectedKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime?[] unexpected = [DateTime.SpecifyKind(CurrentTime()!.Value, unexpectedKind),];

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTime? subject = CurrentTime();
					DateTime?[] expected = [];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!");
				}

				[Fact]
				public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTime? subject = CurrentTime();
					DateTime?[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The unexpected cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenSubjectIsContained_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					IEnumerable<DateTime?> expected = [LaterTime(), subject, EarlierTime(),];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsDifferent_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					DateTime[] expected = [LaterTime()!.Value, EarlierTime()!.Value,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldSucceed()
				{
					DateTime? subject = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(CurrentTime(), LaterTime());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNullAndExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTime? subject = null;
					DateTime[] expected = [];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!")
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTime? subject = null;
					DateTime[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The unexpected cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTime? subject = null;
					DateTime?[] expected = [];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!")
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTime? subject = null;
					DateTime?[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The unexpected cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndUnexpectedCanOnlyBeEnumeratedOnce_ShouldFail()
				{
					DateTime? subject = null;
					DateTime?[] values = [CurrentTime(), null,];
					IEnumerable<DateTime?> expected = Factory.GetSingleUseEnumerable(values);

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(values)},
						              but it was <null>
						              """)
						.Because("the empty check must not consume the values needed for the comparison and the message");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
				{
					DateTime? subject = null;
					IEnumerable<DateTime?> expected = [CurrentTime(), null,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenSubjectOnlyDiffersInKindFromAllValues_ShouldSucceed()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Utc);
					DateTime?[] unexpected = [EarlierTime(1, DateTimeKind.Local), CurrentTime(DateTimeKind.Local),];

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected);

					await That(Act).DoesNotThrow()
						.Because("a subject that cannot be compared to any alternative is not one of them");
				}

				[Fact]
				public async Task WhenSubjectOnlyDiffersInKindFromTheMatchingValue_ShouldSucceed()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Utc);
					DateTime?[] unexpected = [CurrentTime(DateTimeKind.Local), LaterTime(1, DateTimeKind.Utc),];

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected);

					await That(Act).DoesNotThrow()
						.Because("the only alternative with the same ticks has an incompatible Kind");
				}

				[Theory]
				[InlineData(3, 2, false)]
				[InlineData(5, 3, false)]
				[InlineData(2, 2, true)]
				[InlineData(0, 2, true)]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
					int actualDifference, int tolerance, bool expectToThrow)
				{
					DateTime? subject = EarlierTime(actualDifference);
					DateTime?[] expected = [CurrentTime(), LaterTime(),];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected)
							.Within(tolerance.Seconds())
							.Because("we want to test the failure");

					string difference = actualDifference == 0
						? ""
						: $" which differs by -0:0{actualDifference} from the closest value";

					await That(Act).Throws<XunitException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(expected)} ± 0:0{tolerance}, because we want to test the failure,
						              but it was {Formatter.Format(subject)}{difference}
						              """);
				}
			}
		}
	}
}
