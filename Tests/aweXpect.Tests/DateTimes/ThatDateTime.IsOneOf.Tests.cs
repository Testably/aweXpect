using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed class IsOneOf
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAnEarlierAttemptHadAnIncompatibleKind_ShouldDescribeTheLastAttempt()
			{
				DateTime later = LaterTime(1, DateTimeKind.Utc);
				DateTime[] expected = [CurrentTime(DateTimeKind.Utc),];
				int calls = 0;
				Func<DateTime> subject = () => calls++ == 0
					? CurrentTime(DateTimeKind.Local)
					: later;

				async Task Act()
					=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
						.IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              eventually is one of expected within 0:05,
					              but it was {Formatter.Format(later)}, which differs by 0:01 from the closest value

					              Expected values:
					              {Formatter.Format(expected)}
					              """)
					.Because("the kind of an earlier attempt does not describe the last one");
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateTime subject = CurrentTime();
				DateTime[] expected = [];

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				DateTime subject = CurrentTime();
				DateTime[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedOnlyContainsNull_ShouldFail()
			{
				DateTime subject = CurrentTime();
				IEnumerable<DateTime?> expected = [null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}

					              Expected values:
					              [<null>]
					              """);
			}

			[Test]
			[Arguments(DateTimeKind.Utc, DateTimeKind.Local)]
			[Arguments(DateTimeKind.Local, DateTimeKind.Utc)]
			public async Task WhenKindIsIncompatible_ShouldFail(
				DateTimeKind subjectKind, DateTimeKind expectedKind)
			{
				DateTime subject = DateTime.SpecifyKind(CurrentTime(), subjectKind);
				DateTime[] expected = [DateTime.SpecifyKind(CurrentTime(), expectedKind),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it had kind {subjectKind}, which cannot be compared with {expectedKind}

					              Expected values:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task WhenKindIsIncompatibleButAnotherValueMatches_ShouldSucceed()
			{
				DateTime subject = DateTime.SpecifyKind(CurrentTime(), DateTimeKind.Utc);
				DateTime[] expected =
				[
					DateTime.SpecifyKind(CurrentTime(), DateTimeKind.Local),
					DateTime.SpecifyKind(CurrentTime(), DateTimeKind.Utc),
				];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(DateTimeKind.Utc, DateTimeKind.Unspecified)]
			[Arguments(DateTimeKind.Unspecified, DateTimeKind.Utc)]
			[Arguments(DateTimeKind.Local, DateTimeKind.Unspecified)]
			[Arguments(DateTimeKind.Unspecified, DateTimeKind.Local)]
			public async Task WhenKindIsUnspecified_ShouldSucceed(
				DateTimeKind subjectKind, DateTimeKind expectedKind)
			{
				DateTime subject = DateTime.SpecifyKind(CurrentTime(), subjectKind);
				DateTime[] expected = [DateTime.SpecifyKind(CurrentTime(), expectedKind),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(DateTimeKind.Unspecified, DateTimeKind.Local)]
			[Arguments(DateTimeKind.Unspecified, DateTimeKind.Utc)]
			[Arguments(DateTimeKind.Unspecified, DateTimeKind.Unspecified)]
			[Arguments(DateTimeKind.Local, DateTimeKind.Unspecified)]
			[Arguments(DateTimeKind.Utc, DateTimeKind.Unspecified)]
			[Arguments(DateTimeKind.Local, DateTimeKind.Local)]
			[Arguments(DateTimeKind.Utc, DateTimeKind.Utc)]
			public async Task WhenKindsAreCompatible_ShouldSucceed(DateTimeKind subjectKind, DateTimeKind expectedKind)
			{
				DateTime subject = CurrentTime(subjectKind);
				DateTime[] expected = [EarlierTime(1, expectedKind), CurrentTime(expectedKind),];

				async Task Act()
					=> await That(subject).IsOneOf(expected)
						.Because("an Unspecified Kind matches any other Kind and equal Kinds are comparable");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateTime subject = CurrentTime();
				DateTime?[] expected = [];

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Test]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				DateTime subject = CurrentTime();
				DateTime?[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsContained_ShouldSucceed()
			{
				DateTime subject = CurrentTime();
				IEnumerable<DateTime> expected = [LaterTime(), subject, EarlierTime(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				DateTime subject = CurrentTime();
				DateTime[] expected = [LaterTime(), EarlierTime(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}, which differs by -0:01 from the closest value

					              Expected values:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task WhenSubjectOnlyDiffersInKindFromAllValues_ShouldFail()
			{
				DateTime subject = CurrentTime(DateTimeKind.Utc);
				DateTime[] expected = [EarlierTime(1, DateTimeKind.Local), CurrentTime(DateTimeKind.Local),];

				async Task Act()
					=> await That(subject).IsOneOf(expected)
						.Because("no alternative can be compared to the subject");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected, because no alternative can be compared to the subject,
					              but it had kind Utc, which cannot be compared with Local

					              Expected values:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task WhenSubjectOnlyDiffersInKindFromSomeValues_ShouldSucceed()
			{
				DateTime subject = CurrentTime(DateTimeKind.Utc);
				DateTime[] expected = [CurrentTime(DateTimeKind.Local), CurrentTime(DateTimeKind.Utc),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow()
					.Because("an alternative with an incompatible Kind is skipped instead of failing the expectation");
			}

			[Test]
			public async Task WhenSubjectOnlyDiffersInValueFromTheComparableValue_ShouldFail()
			{
				DateTime subject = CurrentTime(DateTimeKind.Utc);
				DateTime[] expected = [CurrentTime(DateTimeKind.Local), LaterTime(1, DateTimeKind.Utc),];

				async Task Act()
					=> await That(subject).IsOneOf(expected)
						.Because("at least one alternative was comparable");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected, because at least one alternative was comparable,
					              but it was {Formatter.Format(subject)}, which differs by -0:01 from the closest value

					              Expected values:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task Within_WhenSubjectOnlyDiffersInKind_ShouldFail()
			{
				DateTime subject = CurrentTime(DateTimeKind.Utc);
				DateTime[] expected = [CurrentTime(DateTimeKind.Local),];

				async Task Act()
					=> await That(subject).IsOneOf(expected)
						.Within(3.Seconds())
						.Because("a tolerance cannot bridge incompatible Kinds");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected ± 0:03, because a tolerance cannot bridge incompatible Kinds,
					              but it had kind Utc, which cannot be compared with Local

					              Expected values:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			[Arguments(3, 2, true)]
			[Arguments(5, 3, true)]
			[Arguments(2, 2, false)]
			[Arguments(0, 2, false)]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
				int actualDifference, int tolerance, bool expectToThrow)
			{
				DateTime subject = EarlierTime(actualDifference);
				DateTime[] expected = [CurrentTime(), LaterTime(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected)
						.Within(tolerance.Seconds())
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.OnlyIf(expectToThrow)
					.WithMessage($"""
					              Expected that subject
					              is one of expected ± 0:0{tolerance}, because we want to test the failure,
					              but it was {Formatter.Format(subject)}, which differs by -0:0{actualDifference} from the closest value

					              Expected values:
					              {Formatter.Format(expected)}
					              """);
			}
		}
	}
}
