#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed class IsOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly[] expected = [];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task WhenExpectedOnlyContainsNull_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				IEnumerable<TimeOnly?> expected = [null,];

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
				TimeOnly subject = CurrentTime();
				TimeOnly?[] expected = [];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task WhenSubjectIsContained_ShouldSucceed()
			{
				TimeOnly subject = CurrentTime();
				IEnumerable<TimeOnly> expected = [LaterTime(), subject, EarlierTime(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly[] expected = [LaterTime(), EarlierTime(),];

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
			[InlineData(3, 2, true)]
			[InlineData(5, 3, true)]
			[InlineData(2, 2, false)]
			[InlineData(0, 2, false)]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
				int actualDifference, int tolerance, bool expectToThrow)
			{
				TimeOnly subject = EarlierTime(actualDifference);
				TimeOnly[] expected = [CurrentTime(), LaterTime(),];

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

			[Fact]
			public async Task Within_WhenValuesWrapAroundMidnight_ShouldSucceed()
			{
				TimeOnly subject = TimeOnly.MinValue;
				TimeOnly[] expected = [new(12, 0), new(23, 59),];

				async Task Act()
					=> await That(subject).IsOneOf(expected)
						.Within(1.Minutes());

				await That(Act).DoesNotThrow()
					.Because("equality uses the shortest distance around the clock face");
			}
		}
	}
}
#endif
