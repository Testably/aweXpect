using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Fact]
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

			[Fact]
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

			[Fact]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
			{
				DateTimeOffset subject = CurrentTime();
				IEnumerable<DateTimeOffset?> expected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

			[Fact]
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

			[Fact]
			public async Task WhenSubjectIsContained_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				IEnumerable<DateTimeOffset> expected = [LaterTime(), subject, EarlierTime(),];

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
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset[] expected = [LaterTime(), EarlierTime(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(3, 2, false)]
			[InlineData(5, 3, false)]
			[InlineData(2, 2, true)]
			[InlineData(0, 2, true)]
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
					: $" which differs by -0:0{actualDifference} from the closest value";

				await That(Act).Throws<XunitException>()
					.OnlyIf(expectToThrow)
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(expected)} ± 0:0{tolerance}, because we want to test the failure,
					              but it was {Formatter.Format(subject)}{difference}
					              """);
			}
		}
	}
}
