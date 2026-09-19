using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateTime subject = CurrentTime();
				DateTime[] expected = [];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
			{
				DateTime subject = CurrentTime();
				IEnumerable<DateTime?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateTime subject = CurrentTime();
				DateTime?[] expected = [];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task WhenSubjectIsContained_ShouldFail()
			{
				DateTime subject = CurrentTime();
				IEnumerable<DateTime> expected = [LaterTime(), subject, EarlierTime(),];

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
				DateTime subject = CurrentTime();
				DateTime[] expected = [LaterTime(), EarlierTime(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectOnlyDiffersInKindFromAllValues_ShouldSucceed()
			{
				DateTime subject = CurrentTime(DateTimeKind.Utc);
				DateTime[] expected = [EarlierTime(1, DateTimeKind.Local), CurrentTime(DateTimeKind.Local),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow()
					.Because("a subject that cannot be compared to any alternative is not one of them");
			}

			[Fact]
			public async Task WhenSubjectOnlyDiffersInKindFromTheMatchingValue_ShouldSucceed()
			{
				DateTime subject = CurrentTime(DateTimeKind.Utc);
				DateTime[] expected = [CurrentTime(DateTimeKind.Local), LaterTime(1, DateTimeKind.Utc),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

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
				DateTime subject = EarlierTime(actualDifference);
				DateTime[] expected = [CurrentTime(), LaterTime(),];

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
