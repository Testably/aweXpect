#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateOnly subject = CurrentTime();
				DateOnly[] expected = [];

				object Act()
					=> That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				DateOnly subject = CurrentTime();
				DateOnly[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
			{
				DateOnly subject = CurrentTime();
				IEnumerable<DateOnly?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateOnly subject = CurrentTime();
				DateOnly?[] expected = [];

				object Act()
					=> That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Fact]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				DateOnly subject = CurrentTime();
				DateOnly?[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsContained_ShouldFail()
			{
				DateOnly subject = CurrentTime();
				IEnumerable<DateOnly> expected = [LaterTime(), subject, EarlierTime(),];

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
				DateOnly subject = CurrentTime();
				DateOnly[] expected = [LaterTime(), EarlierTime(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
			{
				DateOnly subject = CurrentTime();
				DateOnly[] unexpected = [CurrentTime(), LaterTime(),];

				object Act()
					=> That(subject).IsNotOneOf(unexpected)
						.Within(23.Hours());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("tolerance").And
					.WithMessage("Tolerance must be a whole number of days").AsPrefix()
					.Because("a date has no time of day, so the remainder is rejected as soon as it is specified instead of when the expectation is awaited");
			}

			[Theory]
			[InlineData(3, 2, false)]
			[InlineData(5, 3, false)]
			[InlineData(2, 2, true)]
			[InlineData(0, 2, true)]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
				int actualDifference, int tolerance, bool expectToThrow)
			{
				DateOnly subject = EarlierTime(actualDifference);
				DateOnly[] expected = [CurrentTime(), LaterTime(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected)
						.Within(tolerance.Days())
						.Because("we want to test the failure");

				string difference = actualDifference == 0
					? ""
					: $" which differs by -{actualDifference} days from the closest value";

				await That(Act).Throws<XunitException>()
					.OnlyIf(expectToThrow)
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(expected)} ± {tolerance} days, because we want to test the failure,
					              but it was {Formatter.Format(subject)}{difference}
					              """);
			}
		}
	}
}
#endif
