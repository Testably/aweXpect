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

					await That(Act).Throws<XunitException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(expected)} ± 0:0{tolerance}, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}
			}
		}
	}
}
