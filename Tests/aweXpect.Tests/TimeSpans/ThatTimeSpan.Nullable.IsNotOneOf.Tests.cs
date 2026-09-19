using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
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
					TimeSpan? subject = CurrentTime();
					TimeSpan[] expected = [];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!");
				}

				[Fact]
				public async Task WhenExpectedOnlyContainsAnOverflowingValue_ShouldSucceed()
				{
					TimeSpan? subject = TimeSpan.MinValue;
					TimeSpan?[] expected = [TimeSpan.MaxValue,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).DoesNotThrow()
						.Because("a difference that exceeds the range of a time span must pass instead of overflow");
				}

				[Fact]
				public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
				{
					TimeSpan? subject = CurrentTime();
					IEnumerable<TimeSpan?> expected = [null,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan?[] expected = [];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!");
				}

				[Fact]
				public async Task WhenSubjectIsContained_ShouldFail()
				{
					TimeSpan? subject = CurrentTime();
					IEnumerable<TimeSpan?> expected = [LaterTime(), subject, EarlierTime(),];

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
				public async Task WhenSubjectIsContainedAfterAnOverflowingValue_ShouldFail()
				{
					TimeSpan? subject = TimeSpan.MinValue;
					TimeSpan?[] expected = [TimeSpan.MaxValue, TimeSpan.MinValue,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(expected)},
						              but it was the minimum time span
						              """)
						.Because("an earlier candidate that is far away must not hide a matching later candidate");
				}

				[Fact]
				public async Task WhenSubjectIsDifferent_ShouldSucceed()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan[] expected = [LaterTime()!.Value, EarlierTime()!.Value,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldSucceed()
				{
					TimeSpan? subject = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(CurrentTime(), LaterTime());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
				{
					TimeSpan? subject = null;
					IEnumerable<TimeSpan?> expected = [CurrentTime(), null,];

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
					TimeSpan? subject = EarlierTime(actualDifference);
					TimeSpan?[] expected = [CurrentTime(), LaterTime(),];

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
