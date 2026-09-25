using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class IsOneOf
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime?[] values = [LaterTime(), EarlierTime(),];
					IEnumerable<DateTime?> expected = Factory.GetSingleUseEnumerable(values);

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(values)},
						              but it was {Formatter.Format(subject)} which differs by -0:01 from the closest value
						              """)
						.Because("the empty check must not consume the values needed for the comparison and the message");
				}

				[Fact]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTime? subject = CurrentTime();
					DateTime[] expected = [];

					object Act()
						=> That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
						.Because("an empty set is rejected when the expectation is built, before it is awaited");
				}

				[Fact]
				public async Task WhenExpectedIsInfiniteAndContainsTheSubject_ShouldSucceed()
				{
					DateTime start = CurrentTime()!.Value;
					DateTime? subject = start.AddDays(8);
					IEnumerable<DateTime?> expected = Factory.GetFibonacciNumbers<DateTime?>(i => start.AddDays(i));

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow()
						.Because("the values are only enumerated until the subject is found");
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTime? subject = CurrentTime();
					DateTime[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenExpectedOnlyContainsNull_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					IEnumerable<DateTime?> expected = [null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of [<null>],
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData(DateTimeKind.Unspecified, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Unspecified, DateTimeKind.Utc)]
				[InlineData(DateTimeKind.Unspecified, DateTimeKind.Unspecified)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Unspecified)]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Unspecified)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Utc)]
				public async Task WhenKindsAreCompatible_ShouldSucceed(DateTimeKind subjectKind,
					DateTimeKind expectedKind)
				{
					DateTime? subject = CurrentTime(subjectKind);
					DateTime?[] expected = [EarlierTime(1, expectedKind), CurrentTime(expectedKind),];

					async Task Act()
						=> await That(subject).IsOneOf(expected)
							.Because("an Unspecified Kind matches any other Kind and equal Kinds are comparable");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenKindIsIncompatibleButAnotherValueMatches_ShouldSucceed()
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, DateTimeKind.Utc);
					DateTime?[] expected =
					[
						DateTime.SpecifyKind(CurrentTime()!.Value, DateTimeKind.Local),
						DateTime.SpecifyKind(CurrentTime()!.Value, DateTimeKind.Utc),
					];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Utc)]
				public async Task WhenKindIsIncompatible_ShouldFail(
					DateTimeKind subjectKind, DateTimeKind expectedKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime?[] expected = [DateTime.SpecifyKind(CurrentTime()!.Value, expectedKind),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)},
						              but it had kind {subjectKind}, which cannot be compared with {expectedKind}
						              """);
				}

				[Theory]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Unspecified)]
				[InlineData(DateTimeKind.Unspecified, DateTimeKind.Utc)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Unspecified)]
				[InlineData(DateTimeKind.Unspecified, DateTimeKind.Local)]
				public async Task WhenKindIsUnspecified_ShouldSucceed(
					DateTimeKind subjectKind, DateTimeKind expectedKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime?[] expected = [DateTime.SpecifyKind(CurrentTime()!.Value, expectedKind),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTime? subject = CurrentTime();
					DateTime?[] expected = [];

					object Act()
						=> That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
						.Because("an empty set is rejected when the expectation is built, before it is awaited");
				}

				[Fact]
				public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTime? subject = CurrentTime();
					DateTime?[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenSubjectIsContained_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					IEnumerable<DateTime?> expected = [LaterTime(), subject, EarlierTime(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime[] expected = [LaterTime()!.Value, EarlierTime()!.Value,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)} which differs by -0:01 from the closest value
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					IEnumerable<DateTime?> expected = [CurrentTime(), LaterTime(),];

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
				public async Task WhenSubjectIsNullAndExpectedCanOnlyBeEnumeratedOnce_ShouldSucceed()
				{
					DateTime? subject = null;
					IEnumerable<DateTime?> expected = Factory.GetSingleUseEnumerable<DateTime?>(CurrentTime(), null);

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow()
						.Because("the empty check must not consume the value needed for the comparison");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
				{
					DateTime? subject = null;
					IEnumerable<DateTime?> expected = [CurrentTime(), null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNullAndExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTime? subject = null;
					DateTime[] expected = [];

					object Act()
						=> That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTime? subject = null;
					DateTime[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					DateTime? subject = null;
					DateTime?[] expected = [];

					object Act()
						=> That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					DateTime? subject = null;
					DateTime?[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectOnlyDiffersInKindFromAllValues_ShouldFail()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Utc);
					DateTime?[] expected = [EarlierTime(1, DateTimeKind.Local), CurrentTime(DateTimeKind.Local),];

					async Task Act()
						=> await That(subject).IsOneOf(expected)
							.Because("no alternative can be compared to the subject");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)}, because no alternative can be compared to the subject,
						              but it had kind Utc, which cannot be compared with Local
						              """);
				}

				[Fact]
				public async Task WhenSubjectOnlyDiffersInKindFromSomeValues_ShouldSucceed()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Utc);
					DateTime?[] expected = [CurrentTime(DateTimeKind.Local), CurrentTime(DateTimeKind.Utc),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow()
						.Because(
							"an alternative with an incompatible Kind is skipped instead of failing the expectation");
				}

				[Fact]
				public async Task WhenSubjectOnlyDiffersInKindFromTheOnlyNonNullValue_ShouldFail()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Utc);
					DateTime?[] expected = [null, CurrentTime(DateTimeKind.Local),];

					async Task Act()
						=> await That(subject).IsOneOf(expected)
							.Because("a null alternative is neither comparable nor a Kind mismatch");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)}, because a null alternative is neither comparable nor a Kind mismatch,
						              but it had kind Utc, which cannot be compared with Local
						              """);
				}

				[Fact]
				public async Task Within_WhenSubjectOnlyDiffersInKind_ShouldFail()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Utc);
					DateTime?[] expected = [CurrentTime(DateTimeKind.Local),];

					async Task Act()
						=> await That(subject).IsOneOf(expected)
							.Within(3.Seconds())
							.Because("a tolerance cannot bridge incompatible Kinds");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)} ± 0:03, because a tolerance cannot bridge incompatible Kinds,
						              but it had kind Utc, which cannot be compared with Local
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
					DateTime? subject = EarlierTime(actualDifference);
					DateTime?[] expected = [CurrentTime(), LaterTime(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected)
							.Within(tolerance.Seconds())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)} ± 0:0{tolerance}, because we want to test the failure,
						              but it was {Formatter.Format(subject)} which differs by -0:0{actualDifference} from the closest value
						              """);
				}
			}
		}
	}
}
