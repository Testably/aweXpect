using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan[] expected = [];

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
				TimeSpan subject = CurrentTime();
				TimeSpan[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedOnlyContainsAnOverflowingValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan[] expected = [TimeSpan.MaxValue,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow()
					.Because("a difference that exceeds the range of a time span must pass instead of overflow");
			}

			[Test]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				IEnumerable<TimeSpan?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan?[] expected = [];

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
				TimeSpan subject = CurrentTime();
				TimeSpan?[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsContained_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				IEnumerable<TimeSpan> expected = [LaterTime(), subject, EarlierTime(),];

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
			public async Task WhenSubjectIsContainedAfterAnOverflowingValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan[] expected = [TimeSpan.MaxValue, TimeSpan.MinValue,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not one of expected,
					             but it was TimeSpan.MinValue

					             Unexpected values:
					             [TimeSpan.MaxValue, TimeSpan.MinValue]
					             """)
					.Because("an earlier candidate that is far away must not hide a matching later candidate");
			}

			[Test]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan[] expected = [LaterTime(), EarlierTime(),];

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
				TimeSpan subject = EarlierTime(actualDifference);
				TimeSpan[] expected = [CurrentTime(), LaterTime(),];

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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsContained_ShouldSucceed()
			{
				TimeSpan subject = 5.Seconds();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotOneOf(5.Seconds(), 6.Seconds()));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNotContained_ShouldFail()
			{
				TimeSpan subject = 5.Seconds();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotOneOf(6.Seconds(), 7.Seconds()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is one of [0:06, 0:07],
					             but it was 0:05, which differs by -0:01 from the closest value
					             """);
			}
		}
	}
}
