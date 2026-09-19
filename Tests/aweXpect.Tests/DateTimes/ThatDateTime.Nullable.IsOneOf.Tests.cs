using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class IsOneOf
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTime? subject = CurrentTime();
					DateTime[] expected = [];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!");
				}

				[Fact]
				public async Task WhenExpectedOnlyContainsNull_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					IEnumerable<DateTime?> expected = [null,];

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
				public async Task WhenKindIsIncompatibleButAnotherValueMatches_ShouldSucceed()
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, DateTimeKind.Utc);
					DateTime?[] expected =
					[
						DateTime.SpecifyKind(CurrentTime()!.Value, DateTimeKind.Local),
						DateTime.SpecifyKind(CurrentTime()!.Value, DateTimeKind.Utc),
					];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Utc)]
				public async Task WhenKindIsIncompatible_ShouldFail(
					DateTimeKind subjectKind, DateTimeKind expectedKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime?[] expected = [DateTime.SpecifyKind(CurrentTime()!.Value, expectedKind),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Unspecified)]
				[InlineData(DateTimeKind.Unspecified, DateTimeKind.Utc)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Unspecified)]
				[InlineData(DateTimeKind.Unspecified, DateTimeKind.Local)]
				public async Task WhenKindIsUnspecified_ShouldSucceed(
					DateTimeKind subjectKind, DateTimeKind expectedKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime?[] expected = [DateTime.SpecifyKind(CurrentTime()!.Value, expectedKind),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTime? subject = CurrentTime();
					DateTime?[] expected = [];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!");
				}

				[Fact]
				public async Task WhenSubjectIsContained_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					IEnumerable<DateTime?> expected = [LaterTime(), subject, EarlierTime(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime[] expected = [LaterTime()!.Value, EarlierTime()!.Value,];

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
					DateTime? subject = null;
					IEnumerable<DateTime?> expected = [CurrentTime(), LaterTime(),];

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
				public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
				{
					DateTime? subject = null;
					IEnumerable<DateTime?> expected = [CurrentTime(), null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(3, 2, true)]
				[InlineData(5, 3, true)]
				[InlineData(2, 2, false)]
				[InlineData(0, 2, false)]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
					int actualDifference, int tolerance, bool expectToThrow)
				{
					DateTime? subject = EarlierTime(actualDifference);
					DateTime?[] expected = [CurrentTime(), LaterTime(),];

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
