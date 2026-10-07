#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEqualTo
	{
		public sealed class ImmutableWithin
		{
			public sealed class DecimalTests
			{
				[Test]
				[Arguments(false)]
				[Arguments(true)]
				public async Task WhenCombinedWithUsing_ShouldThrowInvalidOperationException(bool negated)
				{
					ImmutableArray<decimal> subject = [1.1m, 2.1m, 3.1m,];
					IEnumerable<decimal> expected = [1.0m, 2.0m, 3.0m,];

					async Task Act()
					{
						if (negated)
						{
							await That(subject).IsNotEqualTo(expected).Within(0.2m).Using(new AllEqualComparer());
						}
						else
						{
							await That(subject).IsEqualTo(expected).Within(0.2m).Using(new AllEqualComparer());
						}
					}

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with Within.")
						.Because("the comparer would silently replace the tolerance");
				}

				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<decimal> subject = [1.1m, 2.1m, 3.1m,];
					IEnumerable<decimal> expected = [1.0m, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2m);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<decimal> subject = [1.1m, 2.3m, 3.1m,];
					IEnumerable<decimal> expected = [1.0m, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2m).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection expected ± 0.2 in any order,
						             but it
						               contained item 2.3 at index 1 that was not expected and
						               lacked 1 of 3 expected items: 2.0

						             Collection:
						             [1.1, 2.3, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}
			}

			public sealed class NullableDecimalTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<decimal?> subject = [1.1m, null, 2.1m, 3.1m,];
					IEnumerable<decimal?> expected = [1.0m, null, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2m);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					ImmutableArray<decimal?> subject = [1.1m, 2.1m, 3.1m,];
					IEnumerable<decimal> expected = [1.0m, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2m);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<decimal?> subject = [1.1m, null, 2.3m, 3.1m,];
					IEnumerable<decimal?> expected = [1.0m, null, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2m).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection expected ± 0.2 in any order,
						             but it
						               contained item 2.3 at index 2 that was not expected and
						               lacked 1 of 4 expected items: 2.0

						             Collection:
						             [1.1, <null>, 2.3, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}
			}

			public sealed class DoubleTests
			{
				[Test]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					ImmutableArray<double> subject = [1.1, double.NaN, 2.1, 3.1,];
					IEnumerable<double> expected = [1.0, double.NaN, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<double> subject = [1.1, 2.1, 3.1,];
					IEnumerable<double> expected = [1.0, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<double> subject = [1.1, 2.3, 3.1,];
					IEnumerable<double> expected = [1.0, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection expected ± 0.2 in any order,
						             but it
						               contained item 2.3 at index 1 that was not expected and
						               lacked 1 of 3 expected items: 2.0

						             Collection:
						             [1.1, 2.3, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}
			}

			public sealed class NullableDoubleTests
			{
				[Test]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					ImmutableArray<double?> subject = [1.1, double.NaN, 2.1, 3.1,];
					IEnumerable<double?> expected = [1.0, double.NaN, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<double?> subject = [1.1, null, 2.1, 3.1,];
					IEnumerable<double?> expected = [1.0, null, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					ImmutableArray<double?> subject = [1.1, 2.1, 3.1,];
					IEnumerable<double> expected = [1.0, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<double?> subject = [1.1, null, 2.3, 3.1,];
					IEnumerable<double?> expected = [1.0, null, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection expected ± 0.2 in any order,
						             but it
						               contained item 2.3 at index 2 that was not expected and
						               lacked 1 of 4 expected items: 2.0

						             Collection:
						             [1.1, <null>, 2.3, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}
			}

			public sealed class FloatTests
			{
				[Test]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					ImmutableArray<float> subject = [1.1F, float.NaN, 2.1F, 3.1F,];
					IEnumerable<float> expected = [1.0F, float.NaN, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<float> subject = [1.1F, 2.1F, 3.1F,];
					IEnumerable<float> expected = [1.0F, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<float> subject = [1.1F, 2.3F, 3.1F,];
					IEnumerable<float> expected = [1.0F, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection expected ± 0.2 in any order,
						             but it
						               contained item 2.3 at index 1 that was not expected and
						               lacked 1 of 3 expected items: 2.0

						             Collection:
						             [1.1, 2.3, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}
			}

			public sealed class NullableFloatTests
			{
				[Test]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					ImmutableArray<float?> subject = [1.1F, float.NaN, 2.1F, 3.1F,];
					IEnumerable<float?> expected = [1.0F, float.NaN, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<float?> subject = [1.1F, null, 2.1F, 3.1F,];
					IEnumerable<float?> expected = [1.0F, null, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					ImmutableArray<float?> subject = [1.1F, 2.1F, 3.1F,];
					IEnumerable<float> expected = [1.0F, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<float?> subject = [1.1F, null, 2.3F, 3.1F,];
					IEnumerable<float?> expected = [1.0F, null, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection expected ± 0.2 in any order,
						             but it
						               contained item 2.3 at index 2 that was not expected and
						               lacked 1 of 4 expected items: 2.0

						             Collection:
						             [1.1, <null>, 2.3, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}
			}

			public sealed class DateTimeTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					ImmutableArray<DateTime> subject =
						[now.AddHours(1), now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTime> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					ImmutableArray<DateTime> subject =
						[now.AddHours(1), now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTime> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection expected ± 1:00 in any order,
						              but it
						                contained item {Formatter.Format(now.AddHours(2))} at index 1 that was not expected and
						                lacked 1 of 3 expected items: {Formatter.Format(now.AddHours(2).AddMinutes(-2))}

						              Collection:
						              [
						                {Formatter.Format(now.AddHours(1))},
						                {Formatter.Format(now.AddHours(2))},
						                {Formatter.Format(now.AddHours(3))}
						              ]

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableDateTimeTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					ImmutableArray<DateTime?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTime?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					ImmutableArray<DateTime?> subject = [now.AddHours(1), now.AddHours(2),];
					IEnumerable<DateTime> expected = [now.AddHours(1), now.AddHours(2).AddMinutes(-1),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					ImmutableArray<DateTime?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTime?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection expected ± 1:00 in any order,
						              but it
						                contained item {Formatter.Format(now.AddHours(2))} at index 2 that was not expected and
						                lacked 1 of 4 expected items: {Formatter.Format(now.AddHours(2).AddMinutes(-2))}

						              Collection:
						              [
						                {Formatter.Format(now.AddHours(1))},
						                <null>,
						                {Formatter.Format(now.AddHours(2))},
						                {Formatter.Format(now.AddHours(3))}
						              ]

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class DateTimeOffsetTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					ImmutableArray<DateTimeOffset> subject =
						[now.AddHours(1), now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTimeOffset> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					ImmutableArray<DateTimeOffset> subject =
						[now.AddHours(1), now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTimeOffset> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection expected ± 1:00 in any order,
						              but it
						                contained item {Formatter.Format(now.AddHours(2))} at index 1 that was not expected and
						                lacked 1 of 3 expected items: {Formatter.Format(now.AddHours(2).AddMinutes(-2))}

						              Collection:
						              [
						                {Formatter.Format(now.AddHours(1))},
						                {Formatter.Format(now.AddHours(2))},
						                {Formatter.Format(now.AddHours(3))}
						              ]

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableDateTimeOffsetTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					ImmutableArray<DateTimeOffset?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTimeOffset?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					ImmutableArray<DateTimeOffset?> subject = [now.AddHours(1), now.AddHours(2),];
					IEnumerable<DateTimeOffset> expected = [now.AddHours(1), now.AddHours(2).AddMinutes(-1),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					ImmutableArray<DateTimeOffset?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTimeOffset?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection expected ± 1:00 in any order,
						              but it
						                contained item {Formatter.Format(now.AddHours(2))} at index 2 that was not expected and
						                lacked 1 of 4 expected items: {Formatter.Format(now.AddHours(2).AddMinutes(-2))}

						              Collection:
						              [
						                {Formatter.Format(now.AddHours(1))},
						                <null>,
						                {Formatter.Format(now.AddHours(2))},
						                {Formatter.Format(now.AddHours(3))}
						              ]

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class TimeSpanTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<TimeSpan> subject = [1.Hours(), 2.Hours(), 3.Hours(),];
					IEnumerable<TimeSpan> expected = [61.Minutes(), 119.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<TimeSpan> subject = [1.Hours(), 2.Hours(), 3.Hours(),];
					IEnumerable<TimeSpan> expected = [61.Minutes(), 118.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection expected ± 1:00 in any order,
						             but it
						               contained item 2:00:00 at index 1 that was not expected and
						               lacked 1 of 3 expected items: 1:58:00

						             Collection:
						             [
						               1:00:00,
						               2:00:00,
						               3:00:00
						             ]

						             Expected:
						             [
						               1:01:00,
						               1:58:00,
						               3:00:00
						             ]
						             """);
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<TimeSpan?> subject = [1.Hours(), null, 2.Hours(), 3.Hours(),];
					IEnumerable<TimeSpan?> expected = [61.Minutes(), null, 119.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					ImmutableArray<TimeSpan?> subject = [1.Hours(), 2.Hours(),];
					IEnumerable<TimeSpan> expected = [1.Hours(), 119.Minutes(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<TimeSpan?> subject = [1.Hours(), null, 2.Hours(), 3.Hours(),];
					IEnumerable<TimeSpan?> expected = [61.Minutes(), null, 118.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection expected ± 1:00 in any order,
						             but it
						               contained item 2:00:00 at index 2 that was not expected and
						               lacked 1 of 4 expected items: 1:58:00

						             Collection:
						             [
						               1:00:00,
						               <null>,
						               2:00:00,
						               3:00:00
						             ]

						             Expected:
						             [
						               1:01:00,
						               <null>,
						               1:58:00,
						               3:00:00
						             ]
						             """);
				}
			}

			public sealed class DateOnlyTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					IEnumerable<DateOnly> expected = [new(2024, 1, 1), new(2024, 1, 12), new(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					IEnumerable<DateOnly> expected = [new(2024, 1, 1), new(2024, 1, 13), new(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection expected ± 1 day in order,
						              but it contained item {Formatter.Format(new DateOnly(2024, 1, 11))} at index 1 instead of {Formatter.Format(new DateOnly(2024, 1, 13))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					IEnumerable<DateOnly> expected = [new(2024, 1, 1), new(2024, 1, 12), new(2024, 1, 21),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Days());
						await That(subject).IsEqualTo(expected);
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Test]
				public async Task WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					IEnumerable<DateOnly> expected = [new(2024, 1, 1), new(2024, 1, 13), new(2024, 1, 21),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Days());
						await That(subject).IsEqualTo(expected);
					}

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection expected ± 1 day in order,
						              but it contained item {Formatter.Format(new DateOnly(2024, 1, 11))} at index 1 instead of {Formatter.Format(new DateOnly(2024, 1, 13))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """)
						.Because("the applied default tolerance is part of the expectation");
				}

				[Test]
				public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					IEnumerable<DateOnly> expected = [new(2024, 1, 1), new(2024, 1, 12), new(2024, 1, 21),];

					object Act()
						=> That(subject).IsEqualTo(expected).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}

			public sealed class NullableDateOnlyTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection expected ± 1 day in order,
						              but it contained item {Formatter.Format(new DateOnly(2024, 1, 11))} at index 2 instead of {Formatter.Format(new DateOnly(2024, 1, 13))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					object Act()
						=> That(subject).IsEqualTo(expected).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}

			public sealed class TimeOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly[] values = [new(22, 0), new(23, 59, 30), new(2, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					IEnumerable<TimeOnly> expected = [new(22, 0), new(0, 0, 30), new(2, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					IEnumerable<TimeOnly> expected = [new(13, 0), new(14, 1), new(15, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					IEnumerable<TimeOnly> expected = [new(13, 0), new(14, 2), new(15, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection expected ± 1:00 in order,
						              but it contained item {Formatter.Format(new TimeOnly(14, 0))} at index 1 instead of {Formatter.Format(new TimeOnly(14, 2))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					IEnumerable<TimeOnly> expected = [new(13, 0), new(14, 1), new(15, 0),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).IsEqualTo(expected);
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Test]
				public async Task WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					IEnumerable<TimeOnly> expected = [new(13, 0), new(14, 2), new(15, 0),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).IsEqualTo(expected);
					}

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection expected ± 1:00 in order,
						              but it contained item {Formatter.Format(new TimeOnly(14, 0))} at index 1 instead of {Formatter.Format(new TimeOnly(14, 2))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """)
						.Because("the applied default tolerance is part of the expectation");
				}
			}

			public sealed class NullableTimeOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly?[] values = [new TimeOnly(22, 0), null, new TimeOnly(23, 59, 30), new TimeOnly(2, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];
					IEnumerable<TimeOnly?> expected = [new TimeOnly(22, 0), null, new TimeOnly(0, 0, 30), new TimeOnly(2, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];
					IEnumerable<TimeOnly?> expected = [new TimeOnly(13, 0), null, new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];
					IEnumerable<TimeOnly?> expected = [new TimeOnly(13, 0), null, new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection expected ± 1:00 in order,
						              but it contained item {Formatter.Format(new TimeOnly(14, 0))} at index 2 instead of {Formatter.Format(new TimeOnly(14, 2))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}
			}
		}
	}
}
#endif
