#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed partial class Nullable
	{
		public sealed class IsNotOneOf
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly?[] values = [LaterTime(), subject, EarlierTime(),];
					IEnumerable<TimeOnly?> expected = Factory.GetSingleUseEnumerable(values);

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(values)},
						              but it was {Formatter.Format(subject)}
						              """)
						.Because("the empty check must not consume the values needed for the comparison and the message");
				}

				[Fact]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly[] expected = [];

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
					TimeOnly? subject = CurrentTime();
					TimeOnly[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
				{
					TimeOnly? subject = CurrentTime();
					IEnumerable<TimeOnly?> expected = [null,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly?[] expected = [];

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
					TimeOnly? subject = CurrentTime();
					TimeOnly?[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenSubjectIsContained_ShouldFail()
				{
					TimeOnly? subject = CurrentTime();
					IEnumerable<TimeOnly?> expected = [LaterTime(), subject, EarlierTime(),];

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
					TimeOnly? subject = CurrentTime();
					TimeOnly[] expected = [LaterTime()!.Value, EarlierTime()!.Value,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldSucceed()
				{
					TimeOnly? subject = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(CurrentTime(), LaterTime());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNullAndExpectedIsEmpty_ShouldThrowArgumentException()
				{
					TimeOnly? subject = null;
					TimeOnly[] expected = [];

					object Act()
						=> That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndExpectedIsNull_ShouldThrowArgumentNullException()
				{
					TimeOnly? subject = null;
					TimeOnly[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					TimeOnly? subject = null;
					TimeOnly?[] expected = [];

					object Act()
						=> That(subject).IsNotOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
						.Because("missing expected values are an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					TimeOnly? subject = null;
					TimeOnly?[]? expected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix()
						.Because("a null list of expected values is an argument error, independent of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndUnexpectedCanOnlyBeEnumeratedOnce_ShouldFail()
				{
					TimeOnly? subject = null;
					TimeOnly?[] values = [CurrentTime(), null,];
					IEnumerable<TimeOnly?> expected = Factory.GetSingleUseEnumerable(values);

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(values)},
						              but it was <null>
						              """)
						.Because("the empty check must not consume the values needed for the comparison and the message");
				}

				[Fact]
				public async Task WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
				{
					TimeOnly? subject = null;
					IEnumerable<TimeOnly?> expected = [CurrentTime(), null,];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Theory]
				[InlineData(3, 2, false)]
				[InlineData(5, 3, false)]
				[InlineData(2, 2, true)]
				[InlineData(0, 2, true)]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
					int actualDifference, int tolerance, bool expectToThrow)
				{
					TimeOnly? subject = EarlierTime(actualDifference);
					TimeOnly?[] expected = [CurrentTime(), LaterTime(),];

					async Task Act()
						=> await That(subject).IsNotOneOf(expected)
							.Within(tolerance.Seconds())
							.Because("we want to test the failure");

					string difference = actualDifference == 0
						? ""
						: $", which differs by -0:0{actualDifference} from the closest value";

					await That(Act).Throws<XunitException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(expected)} ± 0:0{tolerance}, because we want to test the failure,
						              but it was {Formatter.Format(subject)}{difference}
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesWrapAroundMidnight_ShouldFail()
				{
					TimeOnly? subject = TimeOnly.MinValue;
					TimeOnly?[] unexpected = [new TimeOnly(12, 0), new TimeOnly(23, 59),];

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected)
							.Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(unexpected)} ± 1:00,
						              but it was 00:00:00.0000000, which differs by 1:00 from the closest value
						              """)
						.Because("equality uses the shortest distance around the clock face");
				}
			}
		}
	}
}
#endif
