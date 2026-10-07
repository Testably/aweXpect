#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly[] expected = [];

				object Act()
					=> That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
			{
				TimeOnly subject = CurrentTime();
				IEnumerable<TimeOnly?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly?[] expected = [];

				object Act()
					=> That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly?[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenParamsValuesContainTheSubject_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly later = LaterTime();

				async Task Act()
					=> await That(subject).IsNotOneOf(later, subject);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(new[] { later, subject, })},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsContained_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				IEnumerable<TimeOnly> expected = [LaterTime(), subject, EarlierTime(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of expected,
					              but it was {Formatter.Format(subject)}

					              Unexpected values:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly[] expected = [LaterTime(), EarlierTime(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(3, 2, false)]
			[Arguments(5, 3, false)]
			[Arguments(2, 2, true)]
			[Arguments(0, 2, true)]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
				int actualDifference, int tolerance, bool expectToThrow)
			{
				TimeOnly subject = EarlierTime(actualDifference);
				TimeOnly[] expected = [CurrentTime(), LaterTime(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected)
						.Within(tolerance.Seconds())
						.Because("we want to test the failure");

				string difference = actualDifference == 0
					? ""
					: $", which differs by -0:0{actualDifference} from the closest value";

				await That(Act).Throws<FailException>()
					.OnlyIf(expectToThrow)
					.WithMessage($"""
					              Expected that subject
					              is not one of expected ± 0:02, because we want to test the failure,
					              but it was {Formatter.Format(subject)}{difference}

					              Unexpected values:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task Within_WhenValuesWrapAroundMidnight_ShouldFail()
			{
				TimeOnly subject = TimeOnly.MinValue;
				TimeOnly[] unexpected = [new(12, 0), new(23, 59),];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected)
						.Within(1.Minutes());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not one of unexpected ± 1:00,
					             but it was 00:00:00.0000000, which differs by 1:00 from the closest value

					             Unexpected values:
					             [12:00:00.0000000, 23:59:00.0000000]
					             """)
					.Because("equality uses the shortest distance around the clock face");
			}
		}
	}
}
#endif
