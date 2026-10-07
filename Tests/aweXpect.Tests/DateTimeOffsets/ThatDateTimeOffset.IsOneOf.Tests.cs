using System.Collections.Generic;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed class IsOneOf
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset[] expected = [];

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
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedOnlyContainsNull_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				IEnumerable<DateTimeOffset?> expected = [null,];

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
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset?[] expected = [];

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
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset?[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenParamsValuesDoNotContainTheSubject_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset later = LaterTime();
				DateTimeOffset earlier = EarlierTime();

				async Task Act()
					=> await That(subject).IsOneOf(later, earlier);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(new[] { later, earlier, })},
					              but it was {Formatter.Format(subject)}, which differs by -0:01 from the closest value
					              """);
			}

			[Test]
			public async Task WhenSubjectIsContained_ShouldSucceed()
			{
				DateTimeOffset subject = CurrentTime();
				IEnumerable<DateTimeOffset> expected = [LaterTime(), subject, EarlierTime(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset[] expected = [LaterTime(), EarlierTime(),];

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
			public async Task WhenTheDefaultToleranceIsSet_ShouldMentionIt()
			{
				DateTimeOffset subject = EarlierTime(3);
				DateTimeOffset[] expected = [CurrentTime(), LaterTime(),];

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(2.Seconds());
					await That(subject).IsOneOf(expected);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected ± 0:02,
					              but it was {Formatter.Format(subject)}, which differs by -0:03 from the closest value

					              Expected values:
					              {Formatter.Format(expected)}
					              """)
					.Because("the applied default tolerance is part of the expectation");
			}

			[Test]
			[Arguments(3, 2, true)]
			[Arguments(5, 3, true)]
			[Arguments(2, 2, false)]
			[Arguments(0, 2, false)]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
				int actualDifference, int tolerance, bool expectToThrow)
			{
				DateTimeOffset subject = EarlierTime(actualDifference);
				DateTimeOffset[] expected = [CurrentTime(), LaterTime(),];

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
