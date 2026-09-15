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
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(1.1m, 2.1m, 3.1m);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0m, 2.0m, 3.0m,]).Within(0.2m).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not match collection [1.0m, 2.0m, 3.0m,] in any order ± 0.2,
						             but it did

						             Collection:
						             [1.1, 2.1, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Fact]
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
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<decimal?> subject = ToAsyncEnumerable<decimal?>(1.1m, null, 2.1m, 3.1m);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0m, null, 2.0m, 3.0m,]).Within(0.2m);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not match collection [1.0m, null, 2.0m, 3.0m,] in order ± 0.2,
						             but it did

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Fact]
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
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.1, double.NaN, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, double.NaN, 2.0, 3.0,]).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not match collection [1.0, double.NaN, 2.0, 3.0,] in any order ± 0.2,
						             but it did

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [1.0, NaN, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.1, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, 2.0, 3.0,]).Within(0.2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not match collection [1.0, 2.0, 3.0,] in order ± 0.2,
						             but it did

						             Collection:
						             [1.1, 2.1, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Fact]
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
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(1.1, double.NaN, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, double.NaN, 2.0, 3.0,]).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not match collection [1.0, double.NaN, 2.0, 3.0,] in any order ± 0.2,
						             but it did

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [1.0, NaN, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(1.1, null, 2.1, 3.1);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0, null, 2.0, 3.0,]).Within(0.2).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not match collection [1.0, null, 2.0, 3.0,] in any order ± 0.2,
						             but it did

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Fact]
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
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(1.1F, float.NaN, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, float.NaN, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not match collection [1.0F, float.NaN, 2.0F, 3.0F,] in any order ± 0.2,
						             but it did

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [1.0, NaN, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(1.1F, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not match collection [1.0F, 2.0F, 3.0F,] in any order ± 0.2,
						             but it did

						             Collection:
						             [1.1, 2.1, 3.1]

						             Expected:
						             [1.0, 2.0, 3.0]
						             """);
				}

				[Fact]
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
				[Fact]
				public async Task TwoNaNValues_ShouldBeConsideredEqual()
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(1.1F, float.NaN, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, float.NaN, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not match collection [1.0F, float.NaN, 2.0F, 3.0F,] in any order ± 0.2,
						             but it did

						             Collection:
						             [1.1, NaN, 2.1, 3.1]

						             Expected:
						             [1.0, NaN, 2.0, 3.0]
						             """);
				}

				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(1.1F, null, 2.1F, 3.1F);

					async Task Act()
						=> await That(subject).IsNotEqualTo([1.0F, null, 2.0F, 3.0F,]).Within(0.2F).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not match collection [1.0F, null, 2.0F, 3.0F,] in any order ± 0.2,
						             but it did

						             Collection:
						             [1.1, <null>, 2.1, 3.1]

						             Expected:
						             [1.0, <null>, 2.0, 3.0]
						             """);
				}

				[Fact]
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
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					IAsyncEnumerable<DateTime> subject =
						ToAsyncEnumerable(now.AddHours(1), now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTime> expected =
						[now.AddHours(1).AddMinutes(1), now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not match collection expected in any order ± 1:00,
						              but it did

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

				[Fact]
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
				[Fact]
				public async Task WhenEachElementLiesWithinTheTolerance_ShouldFail()
				{
					DateTime now = DateTime.Now;
					IAsyncEnumerable<DateTime?> subject =
						ToAsyncEnumerable<DateTime?>(now.AddHours(1), null, now.AddHours(2), now.AddHours(3));
					IEnumerable<DateTime?> expected =
						[now.AddHours(1).AddMinutes(1), null, now.AddHours(2).AddMinutes(-1), now.AddHours(3),];

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(1.Minutes()).InAnyOrder();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not match collection expected in any order ± 1:00,
						              but it did

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

				[Fact]
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
		}
	}
}
#endif
