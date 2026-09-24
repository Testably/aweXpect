using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class Within
		{
			public sealed class DecimalTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IEnumerable<decimal> subject = [1.1m, 2.1m, 3.1m,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0m, 2.0m, 3.0m,]).Within(0.2m).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0m, 2.0m, 3.0m,] in any order ± 0.2,
						             but it was

						             Collection:
						             [1.1, 2.1, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IEnumerable<decimal> subject = [1.1m, 2.3m, 3.1m,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0m, 2.0m, 3.0m,]).Within(0.2m);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDecimalTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IEnumerable<decimal?> subject = [1.1m, null, 2.1m, 3.1m,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0m, null, 2.0m, 3.0m,]).Within(0.2m);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0m, null, 2.0m, 3.0m,] in order ± 0.2,
						             but it was

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IEnumerable<decimal?> subject = [1.1m, null, 2.3m, 3.1m,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0m, null, 2.0m, 3.0m,]).Within(0.2m).InAnyOrder();

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DoubleTests
			{
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IEnumerable<double> subject = [1.1, double.NaN, 2.1, 3.1,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, double.NaN, 2.0, 3.0,]).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0, double.NaN, 2.0, 3.0,] in any order ± 0.2,
						             but it was

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [1.0, NaN, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IEnumerable<double> subject = [1.1, 2.1, 3.1,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, 2.0, 3.0,]).Within(0.2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0, 2.0, 3.0,] in order ± 0.2,
						             but it was

						             Collection:
						             [1.1, 2.1, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IEnumerable<double> subject = [1.1, 2.3, 3.1,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, 2.0, 3.0,]).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(double.PositiveInfinity, 1.0)]
				[InlineData(double.NegativeInfinity, 1.0)]
				[InlineData(double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldFail(double value, double tolerance)
				{
					IEnumerable<double> subject = [value,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([value,]).Within(tolerance);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection [value,] in order ± {Formatter.Format(tolerance)},
						              but it was

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [{Formatter.Format(value)}]
						              """);
				}

				[Theory]
				[InlineData(double.PositiveInfinity, double.NegativeInfinity, 1.0)]
				[InlineData(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity)]
				[InlineData(12.5, double.PositiveInfinity, double.PositiveInfinity)]
				[InlineData(double.PositiveInfinity, 12.5, double.PositiveInfinity)]
				[InlineData(double.NaN, double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldSucceed(double value, double expected, double tolerance)
				{
					IEnumerable<double> subject = [value,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([expected,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDoubleTests
			{
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IEnumerable<double?> subject = [1.1, double.NaN, 2.1, 3.1,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, double.NaN, 2.0, 3.0,]).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0, double.NaN, 2.0, 3.0,] in any order ± 0.2,
						             but it was

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [
						               1.0,
						               NaN,
						               2.0,
						               3.0
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IEnumerable<double?> subject = [1.1, null, 2.1, 3.1,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, null, 2.0, 3.0,]).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0, null, 2.0, 3.0,] in any order ± 0.2,
						             but it was

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IEnumerable<double?> subject = [1.1, null, 2.3, 3.1,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, null, 2.0, 3.0,]).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(double.PositiveInfinity, 1.0)]
				[InlineData(double.NegativeInfinity, 1.0)]
				[InlineData(double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldFail(double value, double tolerance)
				{
					IEnumerable<double?> subject = [value,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([value,]).Within(tolerance);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection [value,] in order ± {Formatter.Format(tolerance)},
						              but it was

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [
						                {Formatter.Format(value)}
						              ]
						              """);
				}

				[Theory]
				[InlineData(double.PositiveInfinity, double.NegativeInfinity, 1.0)]
				[InlineData(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity)]
				[InlineData(12.5, double.PositiveInfinity, double.PositiveInfinity)]
				[InlineData(double.PositiveInfinity, 12.5, double.PositiveInfinity)]
				[InlineData(double.NaN, double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldSucceed(double value, double expected, double tolerance)
				{
					IEnumerable<double?> subject = [value,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([expected,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FloatTests
			{
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IEnumerable<float> subject = [1.1F, float.NaN, 2.1F, 3.1F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, float.NaN, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0F, float.NaN, 2.0F, 3.0F,] in any order ± 0.2,
						             but it was

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [1.0, NaN, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IEnumerable<float> subject = [1.1F, 2.1F, 3.1F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0F, 2.0F, 3.0F,] in any order ± 0.2,
						             but it was

						             Collection:
						             [1.1, 2.1, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IEnumerable<float> subject = [1.1F, 2.3F, 3.1F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, 2.0F, 3.0F,]).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(float.PositiveInfinity, 1.0F)]
				[InlineData(float.NegativeInfinity, 1.0F)]
				[InlineData(float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldFail(float value, float tolerance)
				{
					IEnumerable<float> subject = [value,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([value,]).Within(tolerance);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection [value,] in order ± {Formatter.Format(tolerance)},
						              but it was

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [{Formatter.Format(value)}]
						              """);
				}

				[Theory]
				[InlineData(float.PositiveInfinity, float.NegativeInfinity, 1.0F)]
				[InlineData(float.PositiveInfinity, float.NegativeInfinity, float.PositiveInfinity)]
				[InlineData(12.5F, float.PositiveInfinity, float.PositiveInfinity)]
				[InlineData(float.PositiveInfinity, 12.5F, float.PositiveInfinity)]
				[InlineData(float.NaN, float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldSucceed(float value, float expected, float tolerance)
				{
					IEnumerable<float> subject = [value,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([expected,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableFloatTests
			{
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IEnumerable<float?> subject = [1.1F, float.NaN, 2.1F, 3.1F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, float.NaN, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0F, float.NaN, 2.0F, 3.0F,] in any order ± 0.2,
						             but it was

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [
						               1.0,
						               NaN,
						               2.0,
						               3.0
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IEnumerable<float?> subject = [1.1F, null, 2.1F, 3.1F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, null, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection [1.0F, null, 2.0F, 3.0F,] in any order ± 0.2,
						             but it was

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IEnumerable<float?> subject = [1.1F, null, 2.3F, 3.1F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, null, 2.0F, 3.0F,]).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(float.PositiveInfinity, 1.0F)]
				[InlineData(float.NegativeInfinity, 1.0F)]
				[InlineData(float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldFail(float value, float tolerance)
				{
					IEnumerable<float?> subject = [value,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([value,]).Within(tolerance);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection [value,] in order ± {Formatter.Format(tolerance)},
						              but it was

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [
						                {Formatter.Format(value)}
						              ]
						              """);
				}

				[Theory]
				[InlineData(float.PositiveInfinity, float.NegativeInfinity, 1.0F)]
				[InlineData(float.PositiveInfinity, float.NegativeInfinity, float.PositiveInfinity)]
				[InlineData(12.5F, float.PositiveInfinity, float.PositiveInfinity)]
				[InlineData(float.PositiveInfinity, 12.5F, float.PositiveInfinity)]
				[InlineData(float.NaN, float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldSucceed(float value, float expected, float tolerance)
				{
					IEnumerable<float?> subject = [value,];

					async Task Act()
						=> await That(subject).IsNotEqualTo([expected,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DateTimeTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					IEnumerable<DateTime> subject =
						[now.AddHours(1), now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTime> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection expected in any order ± 1:00,
						              but it was

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					IEnumerable<DateTime> subject =
						[now.AddHours(1), now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTime> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateTimeTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					IEnumerable<DateTime?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTime?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection expected in any order ± 1:00,
						              but it was

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					IEnumerable<DateTime?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTime?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DateTimeOffsetTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IEnumerable<DateTimeOffset> subject =
						[now.AddHours(1), now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTimeOffset> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection expected in any order ± 1:00,
						              but it was

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IEnumerable<DateTimeOffset> subject =
						[now.AddHours(1), now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTimeOffset> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateTimeOffsetTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IEnumerable<DateTimeOffset?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTimeOffset?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to collection expected in any order ± 1:00,
						              but it was

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IEnumerable<DateTimeOffset?> subject = [now.AddHours(1), now.AddHours(2),];
					IEnumerable<DateTimeOffset> expected = [now.AddHours(1), now.AddHours(2).AddMinutes(-2),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IEnumerable<DateTimeOffset?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTimeOffset?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TimeSpanTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IEnumerable<TimeSpan> subject = [1.Hours(), 2.Hours(), 3.Hours(),];
					IEnumerable<TimeSpan> expected = [61.Minutes(), 119.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in any order ± 1:00,
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

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IEnumerable<TimeSpan> subject = [1.Hours(), 2.Hours(), 3.Hours(),];
					IEnumerable<TimeSpan> expected = [61.Minutes(), 118.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IEnumerable<TimeSpan?> subject = [1.Hours(), null, 2.Hours(), 3.Hours(),];
					IEnumerable<TimeSpan?> expected = [61.Minutes(), null, 119.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in any order ± 1:00,
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

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					IEnumerable<TimeSpan?> subject = [1.Hours(), 2.Hours(),];
					IEnumerable<TimeSpan> expected = [1.Hours(), 118.Minutes(),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					IEnumerable<TimeSpan?> subject = [1.Hours(), null, 2.Hours(), 3.Hours(),];
					IEnumerable<TimeSpan?> expected = [61.Minutes(), null, 118.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
