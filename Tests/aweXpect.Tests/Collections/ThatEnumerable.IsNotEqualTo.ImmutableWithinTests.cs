#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class ImmutableWithin
		{
			public sealed class DecimalTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					ImmutableArray<decimal> subject = [1.1m, 2.1m, 3.1m,];
					IEnumerable<decimal> expected = [1.0m, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2m).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in any order ± 0.2,
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
					ImmutableArray<decimal> subject = [1.1m, 2.3m, 3.1m,];
					IEnumerable<decimal> expected = [1.0m, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2m);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDecimalTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					ImmutableArray<decimal?> subject = [1.1m, null, 2.1m, 3.1m,];
					IEnumerable<decimal?> expected = [1.0m, null, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2m);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in order ± 0.2,
						             but it was

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					ImmutableArray<decimal?> subject = [1.1m, 2.3m, 3.1m,];
					IEnumerable<decimal> expected = [1.0m, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					ImmutableArray<decimal?> subject = [1.1m, null, 2.3m, 3.1m,];
					IEnumerable<decimal?> expected = [1.0m, null, 2.0m, 3.0m,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2m).InAnyOrder();

					await That(Act).DoesNotThrow();
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
						=> await That(subject).IsNotEqualTo(expected).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in any order ± 0.2,
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
					ImmutableArray<double> subject = [1.1, 2.1, 3.1,];
					IEnumerable<double> expected = [1.0, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in order ± 0.2,
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
					ImmutableArray<double> subject = [1.1, 2.3, 3.1,];
					IEnumerable<double> expected = [1.0, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDoubleTests
			{
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					ImmutableArray<double?> subject = [1.1, double.NaN, 2.1, 3.1,];
					IEnumerable<double> expected = [1.0, double.NaN, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in any order ± 0.2,
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
					ImmutableArray<double?> subject = [1.1, null, 2.1, 3.1,];
					IEnumerable<double?> expected = [1.0, null, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in any order ± 0.2,
						             but it was

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					ImmutableArray<double?> subject = [1.1, 2.3, 3.1,];
					IEnumerable<double> expected = [1.0, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					ImmutableArray<double?> subject = [1.1, null, 2.3, 3.1,];
					IEnumerable<double?> expected = [1.0, null, 2.0, 3.0,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2);

					await That(Act).DoesNotThrow();
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
						=> await That(subject).IsNotEqualTo(expected).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in any order ± 0.2,
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
					ImmutableArray<float> subject = [1.1F, 2.1F, 3.1F,];
					IEnumerable<float> expected = [1.0F, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in any order ± 0.2,
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
					ImmutableArray<float> subject = [1.1F, 2.3F, 3.1F,];
					IEnumerable<float> expected = [1.0F, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableFloatTests
			{
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					ImmutableArray<float?> subject = [1.1F, float.NaN, 2.1F, 3.1F,];
					IEnumerable<float> expected = [1.0F, float.NaN, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in any order ± 0.2,
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
					ImmutableArray<float?> subject = [1.1F, null, 2.1F, 3.1F,];
					IEnumerable<float?> expected = [1.0F, null, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to collection expected in any order ± 0.2,
						             but it was

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					ImmutableArray<float?> subject = [1.1F, 2.3F, 3.1F,];
					IEnumerable<float> expected = [1.0F, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					ImmutableArray<float?> subject = [1.1F, null, 2.3F, 3.1F,];
					IEnumerable<float?> expected = [1.0F, null, 2.0F, 3.0F,];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(0.2F);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DateTimeTests
			{
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					ImmutableArray<DateTime> subject =
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
					ImmutableArray<DateTime> subject =
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
					ImmutableArray<DateTime?> subject =
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
				public async Task WhenExpectedItemsAreNotNullable_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					ImmutableArray<DateTime?> subject = [now.AddHours(1), now.AddHours(2),];
					IEnumerable<DateTime> expected = [now.AddHours(1), now.AddHours(2).AddMinutes(-2),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOneElementLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime now = DateTime.Now;
					ImmutableArray<DateTime?> subject =
						[now.AddHours(1), null, now.AddHours(2), now.AddHours(3),];
					IEnumerable<DateTime?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-2), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
