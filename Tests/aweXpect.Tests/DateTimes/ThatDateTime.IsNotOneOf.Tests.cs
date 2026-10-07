using System.Collections.Generic;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAnEarlierAttemptHadAnIncompatibleKind_ShouldDescribeTheLastAttempt()
			{
				DateTime current = CurrentTime(DateTimeKind.Utc);
				DateTime[] unexpected = [current,];
				int calls = 0;
				Func<DateTime> subject = () => calls++ == 0
					? CurrentTime(DateTimeKind.Local)
					: current;

				async Task Act()
					=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
						.IsNotOneOf(unexpected).And.IsOneOf(unexpected).WithTimeSystem(new VirtualTimeSystem());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              eventually is not one of unexpected and is one of unexpected within 0:05,
					              but it was {Formatter.Format(current)}

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """)
					.Because("the kind of an earlier attempt does not describe the last one");
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateTime subject = CurrentTime();
				DateTime[] expected = [];

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
				DateTime subject = CurrentTime();
				DateTime[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
			{
				DateTime subject = CurrentTime();
				IEnumerable<DateTime?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(DateTimeKind.Utc, DateTimeKind.Local)]
			[Arguments(DateTimeKind.Local, DateTimeKind.Utc)]
			public async Task WhenKindIsIncompatible_ShouldSucceed(
				DateTimeKind subjectKind, DateTimeKind unexpectedKind)
			{
				DateTime subject = DateTime.SpecifyKind(CurrentTime(), subjectKind);
				DateTime[] unexpected = [DateTime.SpecifyKind(CurrentTime(), unexpectedKind),];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenKindIsIncompatibleButAnotherValueMatches_ShouldFail()
			{
				DateTime subject = DateTime.SpecifyKind(CurrentTime(), DateTimeKind.Utc);
				DateTime[] unexpected =
				[
					DateTime.SpecifyKind(CurrentTime(), DateTimeKind.Local),
					DateTime.SpecifyKind(CurrentTime(), DateTimeKind.Utc),
				];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected,
					              but it was {Formatter.Format(subject)}

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments(DateTimeKind.Utc, DateTimeKind.Unspecified)]
			[Arguments(DateTimeKind.Unspecified, DateTimeKind.Utc)]
			[Arguments(DateTimeKind.Local, DateTimeKind.Unspecified)]
			[Arguments(DateTimeKind.Unspecified, DateTimeKind.Local)]
			public async Task WhenKindIsUnspecified_ShouldFail(
				DateTimeKind subjectKind, DateTimeKind unexpectedKind)
			{
				DateTime subject = DateTime.SpecifyKind(CurrentTime(), subjectKind);
				DateTime[] unexpected = [DateTime.SpecifyKind(CurrentTime(), unexpectedKind),];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected,
					              but it was {Formatter.Format(subject)}

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateTime subject = CurrentTime();
				DateTime?[] expected = [];

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
				DateTime subject = CurrentTime();
				DateTime?[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenParamsValuesContainTheSubject_ShouldFail()
			{
				DateTime subject = CurrentTime();
				DateTime later = LaterTime();

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
				DateTime subject = CurrentTime();
				IEnumerable<DateTime> expected = [LaterTime(), subject, EarlierTime(),];

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
				DateTime subject = CurrentTime();
				DateTime[] expected = [LaterTime(), EarlierTime(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectOnlyDiffersInKindFromAllValues_ShouldSucceed()
			{
				DateTime subject = CurrentTime(DateTimeKind.Utc);
				DateTime[] expected = [EarlierTime(1, DateTimeKind.Local), CurrentTime(DateTimeKind.Local),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow()
					.Because("a subject that cannot be compared to any alternative is not one of them");
			}

			[Test]
			public async Task WhenSubjectOnlyDiffersInKindFromTheMatchingValue_ShouldSucceed()
			{
				DateTime subject = CurrentTime(DateTimeKind.Utc);
				DateTime[] expected = [CurrentTime(DateTimeKind.Local), LaterTime(1, DateTimeKind.Utc),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow()
					.Because("the only alternative with the same ticks has an incompatible Kind");
			}

			[Test]
			[Arguments(3, 2, false)]
			[Arguments(5, 3, false)]
			[Arguments(2, 2, true)]
			[Arguments(0, 2, true)]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
				int actualDifference, int tolerance, bool expectToThrow)
			{
				DateTime subject = EarlierTime(actualDifference);
				DateTime[] expected = [CurrentTime(), LaterTime(),];

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
