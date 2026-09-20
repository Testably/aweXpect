using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
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
					TimeSpan? subject = CurrentTime();
					TimeSpan[] expected = [];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!");
				}

				[Fact]
				public async Task WhenExpectedOnlyContainsAnOverflowingValue_ShouldFail()
				{
					TimeSpan? subject = TimeSpan.MinValue;
					TimeSpan?[] expected = [TimeSpan.MaxValue,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)},
						              but it was the minimum time span
						              """)
						.Because("a difference that exceeds the range of a time span must fail instead of overflow");
				}

				[Fact]
				public async Task WhenExpectedOnlyContainsNull_ShouldFail()
				{
					TimeSpan? subject = CurrentTime();
					IEnumerable<TimeSpan?> expected = [null,];

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
					TimeSpan? subject = CurrentTime();
					TimeSpan?[] expected = [];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!");
				}

				[Fact]
				public async Task WhenSubjectIsContained_ShouldSucceed()
				{
					TimeSpan? subject = CurrentTime();
					IEnumerable<TimeSpan?> expected = [LaterTime(), subject, EarlierTime(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsContainedAfterAnOverflowingValue_ShouldSucceed()
				{
					TimeSpan? subject = TimeSpan.MinValue;
					TimeSpan?[] expected = [TimeSpan.MaxValue, TimeSpan.MinValue,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow()
						.Because("an earlier candidate that is far away must not hide a matching later candidate");
				}

				[Fact]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan[] expected = [LaterTime()!.Value, EarlierTime()!.Value,];

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
					TimeSpan? subject = null;
					IEnumerable<TimeSpan?> expected = [CurrentTime(), LaterTime(),];

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
					TimeSpan? subject = null;
					IEnumerable<TimeSpan?> expected = [CurrentTime(), null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNullAndExpectedIsEmpty_ShouldThrowArgumentException()
				{
					TimeSpan? subject = null;
					TimeSpan[] expected = [];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!")
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					TimeSpan? subject = null;
					TimeSpan?[] expected = [];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("You have to provide at least one expected value!")
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Theory]
				[InlineData(3, 2, true)]
				[InlineData(5, 3, true)]
				[InlineData(2, 2, false)]
				[InlineData(0, 2, false)]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
					int actualDifference, int tolerance, bool expectToThrow)
				{
					TimeSpan? subject = EarlierTime(actualDifference);
					TimeSpan?[] expected = [CurrentTime(), LaterTime(),];

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
