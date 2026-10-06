#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class Within
		{
			public sealed class DecimalTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(1.1m, 2.1m, 3.1m);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0m, 2.0m, 3.0m,]).Within(0.2m).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0m, 2.0m, 3.0m,] ± 0.2 in any order,
						             but it was

						             Collection:
						             [1.1, 2.1, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(1.1m, 2.3m, 3.1m);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0m, 2.0m, 3.0m,]).Within(0.2m);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDecimalTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<decimal?> subject = ToAsyncEnumerable<decimal?>(1.1m, null, 2.1m, 3.1m);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0m, null, 2.0m, 3.0m,]).Within(0.2m);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0m, null, 2.0m, 3.0m,] ± 0.2 in order,
						             but it was

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<decimal?> subject = ToAsyncEnumerable<decimal?>(1.1m, null, 2.3m, 3.1m);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0m, null, 2.0m, 3.0m,]).Within(0.2m).InAnyOrder();

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DoubleTests
			{
				[Test]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.1, double.NaN, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, double.NaN, 2.0, 3.0,]).Within(0.2).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0, double.NaN, 2.0, 3.0,] ± 0.2 in any order,
						             but it was

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [1.0, NaN, 2.0, 3.0]
						             """);
				}

				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.1, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, 2.0, 3.0,]).Within(0.2);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0, 2.0, 3.0,] ± 0.2 in order,
						             but it was

						             Collection:
						             [1.1, 2.1, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Test]
				[Arguments(double.PositiveInfinity, 1.0)]
				[Arguments(double.NegativeInfinity, 1.0)]
				[Arguments(double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldFail(double value, double tolerance)
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(value);

					async Task Act()
						=> await That(subject).IsNotEqualTo([value,]).Within(tolerance);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection [value,] ± {Formatter.Format(tolerance)} in order,
						              but it was

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [{Formatter.Format(value)}]
						              """);
				}

				[Test]
				[Arguments(double.PositiveInfinity, double.NegativeInfinity, 1.0)]
				[Arguments(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity)]
				[Arguments(12.5, double.PositiveInfinity, double.PositiveInfinity)]
				[Arguments(double.PositiveInfinity, 12.5, double.PositiveInfinity)]
				[Arguments(double.NaN, double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldSucceed(double value, double expected, double tolerance)
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(value);

					async Task Act()
						=> await That(subject).IsNotEqualTo([expected,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.1, 2.3, 3.1);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, 2.0, 3.0,]).Within(0.2);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDoubleTests
			{
				[Test]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(1.1, double.NaN, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, double.NaN, 2.0, 3.0,]).Within(0.2).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0, double.NaN, 2.0, 3.0,] ± 0.2 in any order,
						             but it was

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [1.0, NaN, 2.0, 3.0]
						             """);
				}

				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(1.1, null, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, null, 2.0, 3.0,]).Within(0.2).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0, null, 2.0, 3.0,] ± 0.2 in any order,
						             but it was

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Test]
				[Arguments(double.PositiveInfinity, 1.0)]
				[Arguments(double.NegativeInfinity, 1.0)]
				[Arguments(double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldFail(double value, double tolerance)
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(value);

					async Task Act()
						=> await That(subject).IsNotEqualTo([value,]).Within(tolerance);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection [value,] ± {Formatter.Format(tolerance)} in order,
						              but it was

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [{Formatter.Format(value)}]
						              """);
				}

				[Test]
				[Arguments(double.PositiveInfinity, double.NegativeInfinity, 1.0)]
				[Arguments(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity)]
				[Arguments(12.5, double.PositiveInfinity, double.PositiveInfinity)]
				[Arguments(double.PositiveInfinity, 12.5, double.PositiveInfinity)]
				[Arguments(double.NaN, double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldSucceed(double value, double expected, double tolerance)
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(value);

					async Task Act()
						=> await That(subject).IsNotEqualTo([expected,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(1.1, null, 2.3, 3.1);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, null, 2.0, 3.0,]).Within(0.2);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FloatTests
			{
				[Test]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(1.1F, float.NaN, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, float.NaN, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0F, float.NaN, 2.0F, 3.0F,] ± 0.2 in any order,
						             but it was

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [1.0, NaN, 2.0, 3.0]
						             """);
				}

				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(1.1F, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0F, 2.0F, 3.0F,] ± 0.2 in any order,
						             but it was

						             Collection:
						             [1.1, 2.1, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Test]
				[Arguments(float.PositiveInfinity, 1.0F)]
				[Arguments(float.NegativeInfinity, 1.0F)]
				[Arguments(float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldFail(float value, float tolerance)
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(value);

					async Task Act()
						=> await That(subject).IsNotEqualTo([value,]).Within(tolerance);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection [value,] ± {Formatter.Format(tolerance)} in order,
						              but it was

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [{Formatter.Format(value)}]
						              """);
				}

				[Test]
				[Arguments(float.PositiveInfinity, float.NegativeInfinity, 1.0F)]
				[Arguments(float.PositiveInfinity, float.NegativeInfinity, float.PositiveInfinity)]
				[Arguments(12.5F, float.PositiveInfinity, float.PositiveInfinity)]
				[Arguments(float.PositiveInfinity, 12.5F, float.PositiveInfinity)]
				[Arguments(float.NaN, float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldSucceed(float value, float expected, float tolerance)
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(value);

					async Task Act()
						=> await That(subject).IsNotEqualTo([expected,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(1.1F, 2.3F, 3.1F);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, 2.0F, 3.0F,]).Within(0.2F);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableFloatTests
			{
				[Test]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(1.1F, float.NaN, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, float.NaN, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0F, float.NaN, 2.0F, 3.0F,] ± 0.2 in any order,
						             but it was

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [1.0, NaN, 2.0, 3.0]
						             """);
				}

				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(1.1F, null, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, null, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0F, null, 2.0F, 3.0F,] ± 0.2 in any order,
						             but it was

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Test]
				[Arguments(float.PositiveInfinity, 1.0F)]
				[Arguments(float.NegativeInfinity, 1.0F)]
				[Arguments(float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldFail(float value, float tolerance)
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(value);

					async Task Act()
						=> await That(subject).IsNotEqualTo([value,]).Within(tolerance);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection [value,] ± {Formatter.Format(tolerance)} in order,
						              but it was

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [{Formatter.Format(value)}]
						              """);
				}

				[Test]
				[Arguments(float.PositiveInfinity, float.NegativeInfinity, 1.0F)]
				[Arguments(float.PositiveInfinity, float.NegativeInfinity, float.PositiveInfinity)]
				[Arguments(12.5F, float.PositiveInfinity, float.PositiveInfinity)]
				[Arguments(float.PositiveInfinity, 12.5F, float.PositiveInfinity)]
				[Arguments(float.NaN, float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldSucceed(float value, float expected, float tolerance)
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(value);

					async Task Act()
						=> await That(subject).IsNotEqualTo([expected,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(1.1F, null, 2.3F, 3.1F);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, null, 2.0F, 3.0F,]).Within(0.2F);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DateTimeTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					IAsyncEnumerable<DateTime> subject =
						ToAsyncEnumerable(now.AddHours(1), now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTime> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection expected ± 1:00 in any order,
						              but it was

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

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					IAsyncEnumerable<DateTime> subject =
						ToAsyncEnumerable(now.AddHours(1), now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTime> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateTimeTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					IAsyncEnumerable<DateTime?> subject =
						ToAsyncEnumerable<DateTime?>(now.AddHours(1), null, now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTime?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection expected ± 1:00 in any order,
						              but it was

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

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					IAsyncEnumerable<DateTime?> subject =
						ToAsyncEnumerable<DateTime?>(now.AddHours(1), null, now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTime?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DateTimeOffsetTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IAsyncEnumerable<DateTimeOffset> subject =
						ToAsyncEnumerable(now.AddHours(1), now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTimeOffset> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection expected ± 1:00 in any order,
						              but it was

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

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IAsyncEnumerable<DateTimeOffset> subject =
						ToAsyncEnumerable(now.AddHours(1), now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTimeOffset> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateTimeOffsetTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IAsyncEnumerable<DateTimeOffset?> subject =
						ToAsyncEnumerable<DateTimeOffset?>(now.AddHours(1), null, now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTimeOffset?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection expected ± 1:00 in any order,
						              but it was

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

				[Test]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IAsyncEnumerable<DateTimeOffset?> subject =
						ToAsyncEnumerable<DateTimeOffset?>(now.AddHours(1), now.AddHours(2));
					IEnumerable<DateTimeOffset> expected = [now.AddHours(1), now.AddHours(2).AddMinutes(-2),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IAsyncEnumerable<DateTimeOffset?> subject =
						ToAsyncEnumerable<DateTimeOffset?>(now.AddHours(1), null, now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTimeOffset?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TimeSpanTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<TimeSpan> subject = ToAsyncEnumerable<TimeSpan>(1.Hours(), 2.Hours(), 3.Hours());
					IEnumerable<TimeSpan> expected = [61.Minutes(), 119.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected ± 1:00 in any order,
						             but it was

						             Collection:
						             [
						               1:00:00,
						               2:00:00,
						               3:00:00
						             ]

						             Expected:
						             [
						               1:01:00,
						               1:59:00,
						               3:00:00
						             ]
						             """);
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<TimeSpan> subject = ToAsyncEnumerable<TimeSpan>(1.Hours(), 2.Hours(), 3.Hours());
					IEnumerable<TimeSpan> expected = [61.Minutes(), 118.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<TimeSpan?> subject =
						ToAsyncEnumerable<TimeSpan?>(1.Hours(), null, 2.Hours(), 3.Hours());
					IEnumerable<TimeSpan?> expected = [61.Minutes(), null, 119.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected ± 1:00 in any order,
						             but it was

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
						               1:59:00,
						               3:00:00
						             ]
						             """);
				}

				[Test]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					IAsyncEnumerable<TimeSpan?> subject = ToAsyncEnumerable<TimeSpan?>(1.Hours(), 2.Hours());
					IEnumerable<TimeSpan> expected = [1.Hours(), 118.Minutes(),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<TimeSpan?> subject =
						ToAsyncEnumerable<TimeSpan?>(1.Hours(), null, 2.Hours(), 3.Hours());
					IEnumerable<TimeSpan?> expected = [61.Minutes(), null, 118.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DateOnlyTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly> unexpected = [new(2024, 1, 1), new(2024, 1, 12), new(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection unexpected ± 1 day in order,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly> unexpected = [new(2024, 1, 1), new(2024, 1, 13), new(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateOnlyTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly?> unexpected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection unexpected ± 1 day in order,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly?> unexpected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TimeOnlyTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly> unexpected = [new(13, 0), new(14, 1), new(15, 0),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection unexpected ± 1:00 in order,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly> unexpected = [new(13, 0), new(14, 2), new(15, 0),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableTimeOnlyTests
			{
				[Test]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly?> unexpected = [new TimeOnly(13, 0), null, new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection unexpected ± 1:00 in order,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly?> unexpected = [new TimeOnly(13, 0), null, new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
