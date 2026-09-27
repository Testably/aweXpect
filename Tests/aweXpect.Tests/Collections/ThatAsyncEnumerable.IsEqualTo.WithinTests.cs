#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class IsEqualTo
	{
		public sealed class Within
		{
			public sealed class DecimalTests
			{
				[Theory]
				[InlineData(false)]
				[InlineData(true)]
				public async Task WhenCombinedWithUsing_ShouldThrowInvalidOperationException(bool negated)
				{
					IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(1.1m, 2.1m, 3.1m);

					async Task Act()
					{
						if (negated)
						{
							await That(subject).IsNotEqualTo([1.0m, 2.0m, 3.0m,]).Within(0.2m)
								.Using(new AllEqualComparer());
						}
						else
						{
							await That(subject).IsEqualTo([1.0m, 2.0m, 3.0m,]).Within(0.2m)
								.Using(new AllEqualComparer());
						}
					}

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with Within.")
						.Because("the comparer would silently replace the tolerance");
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(1.1m, 2.1m, 3.1m);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0m, 2.0m, 3.0m,]).Within(0.2m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(1.1m, 2.3m, 3.1m);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0m, 2.0m, 3.0m,]).Within(0.2m).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection [1.0m, 2.0m, 3.0m,] ± 0.2 in any order,
						             but it
						               contained item 2.3 at index 1 that was not expected and
						               lacked 1 of 3 expected items: 2.0

						             Collection:
						             [1.1, 2.3, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenExpectedIsAMultiLineExpression_ShouldTrimTheCommonWhiteSpace()
				{
					IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(1.1m, 2.3m, 3.1m);

					async Task Act()
						=> await That(subject).IsEqualTo([
							1.0m,
							2.0m,
							3.0m,
						]).Within(0.2m).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection [
						             	1.0m,
						             	2.0m,
						             	3.0m,
						             ] ± 0.2 in any order,
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
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<decimal?> subject = ToAsyncEnumerable<decimal?>(1.1m, null, 2.1m, 3.1m);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0m, null, 2.0m, 3.0m,]).Within(0.2m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<decimal?> subject = ToAsyncEnumerable<decimal?>(1.1m, null, 2.3m, 3.1m);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0m, null, 2.0m, 3.0m,]).Within(0.2m).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection [1.0m, null, 2.0m, 3.0m,] ± 0.2 in any order,
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
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.1, double.NaN, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0, double.NaN, 2.0, 3.0,]).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.1, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0, 2.0, 3.0,]).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.1, 2.3, 3.1);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0, 2.0, 3.0,]).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection [1.0, 2.0, 3.0,] ± 0.2 in any order,
						             but it
						               contained item 2.3 at index 1 that was not expected and
						               lacked 1 of 3 expected items: 2.0

						             Collection:
						             [1.1, 2.3, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Theory]
				[InlineData(double.PositiveInfinity, 1.0)]
				[InlineData(double.NegativeInfinity, 1.0)]
				[InlineData(double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldSucceed(double value, double tolerance)
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(value);

					async Task Act()
						=> await That(subject).IsEqualTo([value,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(double.PositiveInfinity, double.NegativeInfinity, 1.0)]
				[InlineData(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity)]
				[InlineData(12.5, double.PositiveInfinity, double.PositiveInfinity)]
				[InlineData(double.PositiveInfinity, 12.5, double.PositiveInfinity)]
				[InlineData(double.NaN, double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldFail(double value, double expected, double tolerance)
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(value);

					async Task Act()
						=> await That(subject).IsEqualTo([expected,]).Within(tolerance);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection [expected,] ± {Formatter.Format(tolerance)} in order,
						              but it contained item {Formatter.Format(value)} at index 0 instead of {Formatter.Format(expected)}

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [{Formatter.Format(expected)}]
						              """);
				}
			}

			public sealed class NullableDoubleTests
			{
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(1.1, double.NaN, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0, double.NaN, 2.0, 3.0,]).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(1.1, null, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0, null, 2.0, 3.0,]).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(1.1, null, 2.3, 3.1);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0, null, 2.0, 3.0,]).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection [1.0, null, 2.0, 3.0,] ± 0.2 in any order,
						             but it
						               contained item 2.3 at index 2 that was not expected and
						               lacked 1 of 4 expected items: 2.0

						             Collection:
						             [1.1, <null>, 2.3, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Theory]
				[InlineData(double.PositiveInfinity, 1.0)]
				[InlineData(double.NegativeInfinity, 1.0)]
				[InlineData(double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldSucceed(double value, double tolerance)
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(value);

					async Task Act()
						=> await That(subject).IsEqualTo([value,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(double.PositiveInfinity, double.NegativeInfinity, 1.0)]
				[InlineData(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity)]
				[InlineData(12.5, double.PositiveInfinity, double.PositiveInfinity)]
				[InlineData(double.PositiveInfinity, 12.5, double.PositiveInfinity)]
				[InlineData(double.NaN, double.PositiveInfinity, double.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldFail(double value, double expected, double tolerance)
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(value);

					async Task Act()
						=> await That(subject).IsEqualTo([expected,]).Within(tolerance);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection [expected,] ± {Formatter.Format(tolerance)} in order,
						              but it contained item {Formatter.Format(value)} at index 0 instead of {Formatter.Format(expected)}

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [
						                {Formatter.Format(expected)}
						              ]
						              """);
				}
			}

			public sealed class FloatTests
			{
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(1.1F, float.NaN, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0F, float.NaN, 2.0F, 3.0F,]).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(1.1F, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0F, 2.0F, 3.0F,]).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(1.1F, 2.3F, 3.1F);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0F, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection [1.0F, 2.0F, 3.0F,] ± 0.2 in any order,
						             but it
						               contained item 2.3 at index 1 that was not expected and
						               lacked 1 of 3 expected items: 2.0

						             Collection:
						             [1.1, 2.3, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Theory]
				[InlineData(float.PositiveInfinity, 1.0F)]
				[InlineData(float.NegativeInfinity, 1.0F)]
				[InlineData(float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldSucceed(float value, float tolerance)
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(value);

					async Task Act()
						=> await That(subject).IsEqualTo([value,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(float.PositiveInfinity, float.NegativeInfinity, 1.0F)]
				[InlineData(float.PositiveInfinity, float.NegativeInfinity, float.PositiveInfinity)]
				[InlineData(12.5F, float.PositiveInfinity, float.PositiveInfinity)]
				[InlineData(float.PositiveInfinity, 12.5F, float.PositiveInfinity)]
				[InlineData(float.NaN, float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldFail(float value, float expected, float tolerance)
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(value);

					async Task Act()
						=> await That(subject).IsEqualTo([expected,]).Within(tolerance);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection [expected,] ± {Formatter.Format(tolerance)} in order,
						              but it contained item {Formatter.Format(value)} at index 0 instead of {Formatter.Format(expected)}

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [{Formatter.Format(expected)}]
						              """);
				}
			}

			public sealed class NullableFloatTests
			{
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(1.1F, float.NaN, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0F, float.NaN, 2.0F, 3.0F,]).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(1.1F, null, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0F, null, 2.0F, 3.0F,]).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(1.1F, null, 2.3F, 3.1F);

					async Task Act()
						=> await That(subject).IsEqualTo([1.0F, null, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection [1.0F, null, 2.0F, 3.0F,] ± 0.2 in any order,
						             but it
						               contained item 2.3 at index 2 that was not expected and
						               lacked 1 of 4 expected items: 2.0

						             Collection:
						             [1.1, <null>, 2.3, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Theory]
				[InlineData(float.PositiveInfinity, 1.0F)]
				[InlineData(float.NegativeInfinity, 1.0F)]
				[InlineData(float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesAreEqual_ShouldSucceed(float value, float tolerance)
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(value);

					async Task Act()
						=> await That(subject).IsEqualTo([value,]).Within(tolerance);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(float.PositiveInfinity, float.NegativeInfinity, 1.0F)]
				[InlineData(float.PositiveInfinity, float.NegativeInfinity, float.PositiveInfinity)]
				[InlineData(12.5F, float.PositiveInfinity, float.PositiveInfinity)]
				[InlineData(float.PositiveInfinity, 12.5F, float.PositiveInfinity)]
				[InlineData(float.NaN, float.PositiveInfinity, float.PositiveInfinity)]
				public async Task WhenNonFiniteValuesDiffer_ShouldFail(float value, float expected, float tolerance)
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(value);

					async Task Act()
						=> await That(subject).IsEqualTo([expected,]).Within(tolerance);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to collection [expected,] ± {Formatter.Format(tolerance)} in order,
						              but it contained item {Formatter.Format(value)} at index 0 instead of {Formatter.Format(expected)}

						              Collection:
						              [{Formatter.Format(value)}]

						              Expected:
						              [
						                {Formatter.Format(expected)}
						              ]
						              """);
				}
			}

			public sealed class DateTimeTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					IAsyncEnumerable<DateTime> subject =
						ToAsyncEnumerable(now.AddHours(1), now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTime> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					IAsyncEnumerable<DateTime> subject =
						ToAsyncEnumerable(now.AddHours(1), now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTime> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					IAsyncEnumerable<DateTime?> subject =
						ToAsyncEnumerable<DateTime?>(now.AddHours(1), null, now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTime?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					IAsyncEnumerable<DateTime?> subject =
						ToAsyncEnumerable<DateTime?>(now.AddHours(1), null, now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTime?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IAsyncEnumerable<DateTimeOffset> subject =
						ToAsyncEnumerable(now.AddHours(1), now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTimeOffset> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IAsyncEnumerable<DateTimeOffset> subject =
						ToAsyncEnumerable(now.AddHours(1), now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTimeOffset> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IAsyncEnumerable<DateTimeOffset?> subject =
						ToAsyncEnumerable<DateTimeOffset?>(now.AddHours(1), null, now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTimeOffset?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IAsyncEnumerable<DateTimeOffset?> subject =
						ToAsyncEnumerable<DateTimeOffset?>(now.AddHours(1), now.AddHours(2));
					IEnumerable<DateTimeOffset> expected = [now.AddHours(1), now.AddHours(2).AddMinutes(-1),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					IAsyncEnumerable<DateTimeOffset?> subject =
						ToAsyncEnumerable<DateTimeOffset?>(now.AddHours(1), null, now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTimeOffset?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<TimeSpan> subject = ToAsyncEnumerable<TimeSpan>(1.Hours(), 2.Hours(), 3.Hours());
					IEnumerable<TimeSpan> expected = [61.Minutes(), 119.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<TimeSpan> subject = ToAsyncEnumerable<TimeSpan>(1.Hours(), 2.Hours(), 3.Hours());
					IEnumerable<TimeSpan> expected = [61.Minutes(), 118.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenTheDefaultToleranceIsSet_ShouldApplyAndMentionIt()
				{
					IAsyncEnumerable<TimeSpan> subject = ToAsyncEnumerable<TimeSpan>(1.Hours(), 2.Hours(), 3.Hours());
					IEnumerable<TimeSpan> expected = [61.Minutes(), 118.Minutes(), 3.Hours(),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).IsEqualTo(expected).InAnyOrder();
					}

					await That(Act).Throws<XunitException>()
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
						             """)
						.Because("the first item matches only within the default tolerance, which is part of the expectation");
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					IAsyncEnumerable<TimeSpan?> subject =
						ToAsyncEnumerable<TimeSpan?>(1.Hours(), null, 2.Hours(), 3.Hours());
					IEnumerable<TimeSpan?> expected = [61.Minutes(), null, 119.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					IAsyncEnumerable<TimeSpan?> subject = ToAsyncEnumerable<TimeSpan?>(1.Hours(), 2.Hours());
					IEnumerable<TimeSpan> expected = [1.Hours(), 119.Minutes(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<TimeSpan?> subject =
						ToAsyncEnumerable<TimeSpan?>(1.Hours(), null, 2.Hours(), 3.Hours());
					IEnumerable<TimeSpan?> expected = [61.Minutes(), null, 118.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly> expected = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly> expected = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Days());

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly> expected = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Days());
						await That(subject).IsEqualTo(expected);
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Fact]
				public async Task WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly> expected = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Days());
						await That(subject).IsEqualTo(expected);
					}

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly> expected = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					object Act()
						=> That(subject).IsEqualTo(expected).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("Tolerance must be a whole number of days").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}

			public sealed class NullableDateOnlyTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Days());

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					object Act()
						=> That(subject).IsEqualTo(expected).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("Tolerance must be a whole number of days").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}

			public sealed class TimeOnlyTests
			{
				[Fact]
				public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly[] values = [new TimeOnly(22, 0), new TimeOnly(23, 59, 30), new TimeOnly(2, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly> expected = [new TimeOnly(22, 0), new TimeOnly(0, 0, 30), new TimeOnly(2, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly> expected = [new TimeOnly(13, 0), new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly> expected = [new TimeOnly(13, 0), new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly> expected = [new TimeOnly(13, 0), new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).IsEqualTo(expected);
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Fact]
				public async Task WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly> expected = [new TimeOnly(13, 0), new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).IsEqualTo(expected);
					}

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly?[] values = [new TimeOnly(22, 0), null, new TimeOnly(23, 59, 30), new TimeOnly(2, 0),];
					IAsyncEnumerable<TimeOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly?> expected = [new TimeOnly(22, 0), null, new TimeOnly(0, 0, 30), new TimeOnly(2, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly?> expected = [new TimeOnly(13, 0), null, new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly?> expected = [new TimeOnly(13, 0), null, new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
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
