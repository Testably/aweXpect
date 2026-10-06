using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset[] expected = [];

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
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
			{
				DateTimeOffset subject = CurrentTime();
				IEnumerable<DateTimeOffset?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset?[] expected = [];

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
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset?[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsContained_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				IEnumerable<DateTimeOffset> expected = [LaterTime(), subject, EarlierTime(),];

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
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset[] expected = [LaterTime(), EarlierTime(),];

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
				DateTimeOffset subject = EarlierTime(actualDifference);
				DateTimeOffset[] expected = [CurrentTime(), LaterTime(),];

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
	}
}
