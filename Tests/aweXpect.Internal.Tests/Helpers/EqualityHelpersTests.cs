using System.Globalization;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public sealed class EqualityHelpersTests
{
	public sealed class IsConsideredEqualTo
	{
		public sealed class DoubleTests
		{
			[Test]
			public async Task WhenBothAreNaN_ShouldReturnTrue()
			{
				double value = double.NaN;
				double expected = double.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsTrue();
			}

			[Test]
			[Arguments(0.1, 0.0, 0.1)]
			[Arguments(0.0, 0.1, 0.1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				double value, double expected, double tolerance)
			{
				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNaN_ShouldReturnFalse()
			{
				double value = 0.1;
				double expected = double.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				double value = double.NaN;
				double? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNaN_ShouldReturnFalse()
			{
				double value = double.NaN;
				double expected = 0.1;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}
		}

		public sealed class NullableDoubleTests
		{
			[Test]
			public async Task WhenBothAreNaN_ShouldReturnTrue()
			{
				double? value = double.NaN;
				double expected = double.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				double? value = null;
				double? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsTrue();
			}

			[Test]
			[Arguments(0.1, 0.0, 0.1)]
			[Arguments(0.0, 0.1, 0.1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				double? value, double expected, double tolerance)
			{
				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNaN_ShouldReturnFalse()
			{
				double? value = 0.1;
				double expected = double.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				double? value = double.NaN;
				double? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNaN_ShouldReturnFalse()
			{
				double? value = double.NaN;
				double expected = 0.1;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldReturnFalse()
			{
				double? value = null;
				double? expected = double.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}
		}

		public sealed class FloatTests
		{
			[Test]
			public async Task WhenBothAreNaN_ShouldReturnTrue()
			{
				float value = float.NaN;
				float expected = float.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsTrue();
			}

			[Test]
			[Arguments(0.1F, 0.0F, 0.1F)]
			[Arguments(0.0F, 0.1F, 0.1F)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				float value, float expected, float tolerance)
			{
				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNaN_ShouldReturnFalse()
			{
				float value = 0.1F;
				float expected = float.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				float value = float.NaN;
				float? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNaN_ShouldReturnFalse()
			{
				float value = float.NaN;
				float expected = 0.1F;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}
		}

		public sealed class NullableFloatTests
		{
			[Test]
			public async Task WhenBothAreNaN_ShouldReturnTrue()
			{
				float? value = float.NaN;
				float expected = float.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				float? value = null;
				float? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsTrue();
			}

			[Test]
			[Arguments(0.1F, 0.0F, 0.1F)]
			[Arguments(0.0F, 0.1F, 0.1F)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				float? value, float expected, float tolerance)
			{
				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNaN_ShouldReturnFalse()
			{
				float? value = 0.1F;
				float expected = float.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				float? value = float.NaN;
				float? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNaN_ShouldReturnFalse()
			{
				float? value = float.NaN;
				float expected = 0.1F;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldReturnFalse()
			{
				float? value = null;
				float? expected = float.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}
		}

		public sealed class DecimalTests
		{
			[Test]
			[Arguments("0.1", "0.0", "0.1")]
			[Arguments("0.0", "0.1", "0.1")]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				string valueString, string expectedString, string toleranceString)
			{
				decimal value = decimal.Parse(valueString, CultureInfo.InvariantCulture);
				decimal expected = decimal.Parse(expectedString, CultureInfo.InvariantCulture);
				decimal tolerance = decimal.Parse(toleranceString, CultureInfo.InvariantCulture);

				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				decimal value = decimal.MinValue;
				decimal? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1m);

				await That(result).IsFalse();
			}
		}

		public sealed class NullableDecimalTests
		{
			[Test]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				decimal? value = null;
				decimal? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1m);

				await That(result).IsTrue();
			}

			[Test]
			[Arguments("0.1", "0.0", "0.1")]
			[Arguments("0.0", "0.1", "0.1")]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				string valueString, string expectedString, string toleranceString)
			{
				decimal? value = decimal.Parse(valueString, CultureInfo.InvariantCulture);
				decimal? expected = decimal.Parse(expectedString, CultureInfo.InvariantCulture);
				decimal tolerance = decimal.Parse(toleranceString, CultureInfo.InvariantCulture);

				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				decimal? value = decimal.MinValue;
				decimal? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1m);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldReturnFalse()
			{
				decimal? value = null;
				decimal? expected = decimal.MinValue;

				bool result = value.IsConsideredEqualTo(expected, 0.1m);

				await That(result).IsFalse();
			}
		}

		public sealed class NullableDateTimeTests
		{
			[Test]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				DateTime? value = null;
				DateTime? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.Zero, out bool hasKindDifference);

				await That(result).IsTrue();
				await That(hasKindDifference).IsFalse();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				DateTime? value = DateTime.Now;
				DateTime? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue, out bool hasKindDifference);

				await That(result).IsFalse();
				await That(hasKindDifference).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNullAndSubjectKindIsUnspecified_ShouldReturnFalseWithoutKindDifference()
			{
				DateTime? value = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);
				DateTime? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue, out bool hasKindDifference);

				await That(result).IsFalse();
				await That(hasKindDifference).IsFalse()
					.Because("an unspecified kind is compatible with any kind, so only the missing value fails");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldReturnFalse()
			{
				DateTime? value = null;
				DateTime? expected = DateTime.Now;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue, out bool hasKindDifference);

				await That(result).IsFalse();
				await That(hasKindDifference).IsTrue();
			}
		}

		public sealed class DateTimeOffsetTests
		{
			[Test]
			public async Task WhenDifferenceExceedsTolerance_ShouldReturnFalse()
			{
				DateTimeOffset value = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
				DateTimeOffset expected = value.AddTicks(1);

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.Zero);

				await That(result).IsFalse();
			}

			[Test]
			[Arguments(1)]
			[Arguments(-1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(int seconds)
			{
				DateTimeOffset value = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
				DateTimeOffset expected = value.AddSeconds(seconds);

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.FromSeconds(1));

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				DateTimeOffset value = DateTimeOffset.MinValue;
				DateTimeOffset? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenOffsetsDiffer_ShouldCompareTheInstants()
			{
				DateTimeOffset value = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
				DateTimeOffset expected = new(2026, 1, 1, 14, 0, 0, TimeSpan.FromHours(2));

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.Zero);

				await That(result).IsTrue()
					.Because("both values denote the same instant, like the equality of DateTimeOffset");
			}
		}

		public sealed class NullableDateTimeOffsetTests
		{
			[Test]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				DateTimeOffset? value = null;
				DateTimeOffset? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.Zero);

				await That(result).IsTrue();
			}

			[Test]
			[Arguments(1)]
			[Arguments(-1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(int seconds)
			{
				DateTimeOffset? value = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
				DateTimeOffset? expected = value.Value.AddSeconds(seconds);

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.FromSeconds(1));

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				DateTimeOffset? value = DateTimeOffset.MinValue;
				DateTimeOffset? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldReturnFalse()
			{
				DateTimeOffset? value = null;
				DateTimeOffset? expected = DateTimeOffset.MinValue;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}
		}

		public sealed class TimeSpanTests
		{
			[Test]
			public async Task WhenDifferenceIsOutsideTheRangeOfATimeSpan_ShouldReturnFalse()
			{
				TimeSpan value = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MaxValue;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse()
					.Because("the difference exceeds even the largest tolerance, without overflowing");
			}

			[Test]
			[Arguments(1)]
			[Arguments(-1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(int seconds)
			{
				TimeSpan value = TimeSpan.FromMinutes(1);
				TimeSpan expected = value.Add(TimeSpan.FromSeconds(seconds));

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.FromSeconds(1));

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				TimeSpan value = TimeSpan.Zero;
				TimeSpan? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}
		}

		public sealed class NullableTimeSpanTests
		{
			[Test]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				TimeSpan? value = null;
				TimeSpan? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.Zero);

				await That(result).IsTrue();
			}

			[Test]
			[Arguments(1)]
			[Arguments(-1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(int seconds)
			{
				TimeSpan? value = TimeSpan.FromMinutes(1);
				TimeSpan? expected = value.Value.Add(TimeSpan.FromSeconds(seconds));

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.FromSeconds(1));

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				TimeSpan? value = TimeSpan.Zero;
				TimeSpan? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldReturnFalse()
			{
				TimeSpan? value = null;
				TimeSpan? expected = TimeSpan.Zero;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}
		}

		public sealed class LongTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				long value = 0L;
				long? expected = null;

				bool result = value.IsConsideredEqualTo(expected, long.MaxValue);

				await That(result).IsFalse();
			}
		}

		public sealed class NullableLongTests
		{
			[Test]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				long? value = null;
				long? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0L);

				await That(result).IsTrue();
			}

			[Test]
			[Arguments(1L, 0L)]
			[Arguments(0L, 1L)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(long? value, long? expected)
			{
				bool result = value.IsConsideredEqualTo(expected, 1L);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				long? value = 0L;
				long? expected = null;

				bool result = value.IsConsideredEqualTo(expected, long.MaxValue);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldReturnFalse()
			{
				long? value = null;
				long? expected = 0L;

				bool result = value.IsConsideredEqualTo(expected, long.MaxValue);

				await That(result).IsFalse();
			}
		}

		public sealed class ULongTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				ulong value = 0UL;
				ulong? expected = null;

				bool result = value.IsConsideredEqualTo(expected, ulong.MaxValue);

				await That(result).IsFalse();
			}
		}

		public sealed class NullableULongTests
		{
			[Test]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				ulong? value = null;
				ulong? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0UL);

				await That(result).IsTrue();
			}

			[Test]
			[Arguments(1UL, 0UL)]
			[Arguments(0UL, 1UL)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(ulong? value, ulong? expected)
			{
				bool result = value.IsConsideredEqualTo(expected, 1UL);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				ulong? value = 0UL;
				ulong? expected = null;

				bool result = value.IsConsideredEqualTo(expected, ulong.MaxValue);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldReturnFalse()
			{
				ulong? value = null;
				ulong? expected = 0UL;

				bool result = value.IsConsideredEqualTo(expected, ulong.MaxValue);

				await That(result).IsFalse();
			}
		}

		public sealed class DateTimeTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalseWithoutKindDifference()
			{
				DateTime value = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);
				DateTime? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue, out bool hasKindDifference);

				await That(result).IsFalse();
				await That(hasKindDifference).IsFalse()
					.Because("an unspecified kind is compatible with any kind, so only the missing value fails");
			}
		}
#if NET8_0_OR_GREATER

		public sealed class DateOnlyTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				DateOnly value = DateOnly.MinValue;
				DateOnly? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}
		}

		public sealed class NullableDateOnlyTests
		{
			[Test]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				DateOnly? value = null;
				DateOnly? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.Zero);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue()
			{
				DateOnly? value = new DateOnly(2026, 1, 1);
				DateOnly? expected = new DateOnly(2026, 1, 2);

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.FromDays(1));

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				DateOnly? value = DateOnly.MinValue;
				DateOnly? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldReturnFalse()
			{
				DateOnly? value = null;
				DateOnly? expected = DateOnly.MinValue;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}
		}

		public sealed class TimeOnlyTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				TimeOnly value = TimeOnly.MinValue;
				TimeOnly? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}
		}

		public sealed class NullableTimeOnlyTests
		{
			[Test]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				TimeOnly? value = null;
				TimeOnly? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.Zero);

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue()
			{
				TimeOnly? value = new TimeOnly(12, 0);
				TimeOnly? expected = new TimeOnly(12, 1);

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.FromMinutes(1));

				await That(result).IsTrue();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				TimeOnly? value = TimeOnly.MinValue;
				TimeOnly? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldReturnFalse()
			{
				TimeOnly? value = null;
				TimeOnly? expected = TimeOnly.MinValue;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}
		}
#endif
	}
}
