#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;

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
				[Theory]
				[InlineData(false)]
				[InlineData(true)]
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

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<decimal> subject = [1.1m, 2.1m, 3.1m,];
					IEnumerable<decimal> expected = [1.0m, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<decimal> subject = [1.1m, 2.3m, 3.1m,];
					IEnumerable<decimal> expected = [1.0m, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2m).InAnyOrder();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<decimal?> subject = [1.1m, null, 2.1m, 3.1m,];
					IEnumerable<decimal?> expected = [1.0m, null, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					ImmutableArray<decimal?> subject = [1.1m, 2.1m, 3.1m,];
					IEnumerable<decimal> expected = [1.0m, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<decimal?> subject = [1.1m, null, 2.3m, 3.1m,];
					IEnumerable<decimal?> expected = [1.0m, null, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2m).InAnyOrder();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					ImmutableArray<double> subject = [1.1, double.NaN, 2.1, 3.1,];
					IEnumerable<double> expected = [1.0, double.NaN, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<double> subject = [1.1, 2.1, 3.1,];
					IEnumerable<double> expected = [1.0, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<double> subject = [1.1, 2.3, 3.1,];
					IEnumerable<double> expected = [1.0, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					ImmutableArray<double?> subject = [1.1, double.NaN, 2.1, 3.1,];
					IEnumerable<double?> expected = [1.0, double.NaN, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<double?> subject = [1.1, null, 2.1, 3.1,];
					IEnumerable<double?> expected = [1.0, null, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					ImmutableArray<double?> subject = [1.1, 2.1, 3.1,];
					IEnumerable<double> expected = [1.0, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<double?> subject = [1.1, null, 2.3, 3.1,];
					IEnumerable<double?> expected = [1.0, null, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					ImmutableArray<float> subject = [1.1F, float.NaN, 2.1F, 3.1F,];
					IEnumerable<float> expected = [1.0F, float.NaN, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<float> subject = [1.1F, 2.1F, 3.1F,];
					IEnumerable<float> expected = [1.0F, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<float> subject = [1.1F, 2.3F, 3.1F,];
					IEnumerable<float> expected = [1.0F, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					ImmutableArray<float?> subject = [1.1F, float.NaN, 2.1F, 3.1F,];
					IEnumerable<float?> expected = [1.0F, float.NaN, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<float?> subject = [1.1F, null, 2.1F, 3.1F,];
					IEnumerable<float?> expected = [1.0F, null, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					ImmutableArray<float?> subject = [1.1F, 2.1F, 3.1F,];
					IEnumerable<float> expected = [1.0F, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<float?> subject = [1.1F, null, 2.3F, 3.1F,];
					IEnumerable<float?> expected = [1.0F, null, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
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
				[Fact]
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

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					ImmutableArray<DateTime> subject =
						[now.AddHours(1), now.AddHours(2), now.AddHours(3),];
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
					ImmutableArray<DateTime?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTime?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					ImmutableArray<DateTime?> subject = [now.AddHours(1), now.AddHours(2),];
					IEnumerable<DateTime> expected = [now.AddHours(1), now.AddHours(2).AddMinutes(-1),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					ImmutableArray<DateTime?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
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
					ImmutableArray<DateTimeOffset> subject =
						[now.AddHours(1), now.AddHours(2), now.AddHours(3),];
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
					ImmutableArray<DateTimeOffset> subject =
						[now.AddHours(1), now.AddHours(2), now.AddHours(3),];
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
					ImmutableArray<DateTimeOffset?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
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
					ImmutableArray<DateTimeOffset?> subject = [now.AddHours(1), now.AddHours(2),];
					IEnumerable<DateTimeOffset> expected = [now.AddHours(1), now.AddHours(2).AddMinutes(-1),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset now = DateTimeOffset.Now;
					ImmutableArray<DateTimeOffset?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
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
					ImmutableArray<TimeSpan> subject = [1.Hours(), 2.Hours(), 3.Hours(),];
					IEnumerable<TimeSpan> expected = [61.Minutes(), 119.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<TimeSpan> subject = [1.Hours(), 2.Hours(), 3.Hours(),];
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
			}

			public sealed class NullableTimeSpanTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldSucceed()
				{
					ImmutableArray<TimeSpan?> subject = [1.Hours(), null, 2.Hours(), 3.Hours(),];
					IEnumerable<TimeSpan?> expected = [61.Minutes(), null, 119.Minutes(), 3.Hours(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					ImmutableArray<TimeSpan?> subject = [1.Hours(), 2.Hours(),];
					IEnumerable<TimeSpan> expected = [1.Hours(), 119.Minutes(),];

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldFail()
				{
					ImmutableArray<TimeSpan?> subject = [1.Hours(), null, 2.Hours(), 3.Hours(),];
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
		}
	}
}
#endif
