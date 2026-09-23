using System.Globalization;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public sealed class EqualityHelpersTests
{
	public sealed class IsConsideredEqualTo
	{
		public sealed class DoubleTests
		{
			[Fact]
			public async Task WhenBothAreNaN_ShouldReturnTrue()
			{
				double value = double.NaN;
				double expected = double.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsTrue();
			}

			[Theory]
			[InlineData(0.1, 0.0, 0.1)]
			[InlineData(0.0, 0.1, 0.1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				double value, double expected, double tolerance)
			{
				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Fact]
			public async Task WhenExpectedIsNaN_ShouldReturnFalse()
			{
				double value = 0.1;
				double expected = double.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				double value = double.NaN;
				double? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}

			[Fact]
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
			[Fact]
			public async Task WhenBothAreNaN_ShouldReturnTrue()
			{
				double? value = double.NaN;
				double expected = double.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsTrue();
			}

			[Fact]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				double? value = null;
				double? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsTrue();
			}

			[Theory]
			[InlineData(0.1, 0.0, 0.1)]
			[InlineData(0.0, 0.1, 0.1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				double? value, double expected, double tolerance)
			{
				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Fact]
			public async Task WhenExpectedIsNaN_ShouldReturnFalse()
			{
				double? value = 0.1;
				double expected = double.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				double? value = double.NaN;
				double? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}

			[Fact]
			public async Task WhenSubjectIsNaN_ShouldReturnFalse()
			{
				double? value = double.NaN;
				double expected = 0.1;

				bool result = value.IsConsideredEqualTo(expected, 0.1);

				await That(result).IsFalse();
			}

			[Fact]
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
			[Fact]
			public async Task WhenBothAreNaN_ShouldReturnTrue()
			{
				float value = float.NaN;
				float expected = float.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsTrue();
			}

			[Theory]
			[InlineData(0.1F, 0.0F, 0.1F)]
			[InlineData(0.0F, 0.1F, 0.1F)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				float value, float expected, float tolerance)
			{
				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Fact]
			public async Task WhenExpectedIsNaN_ShouldReturnFalse()
			{
				float value = 0.1F;
				float expected = float.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				float value = float.NaN;
				float? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}

			[Fact]
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
			[Fact]
			public async Task WhenBothAreNaN_ShouldReturnTrue()
			{
				float? value = float.NaN;
				float expected = float.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsTrue();
			}

			[Fact]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				float? value = null;
				float? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsTrue();
			}

			[Theory]
			[InlineData(0.1F, 0.0F, 0.1F)]
			[InlineData(0.0F, 0.1F, 0.1F)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				float? value, float expected, float tolerance)
			{
				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Fact]
			public async Task WhenExpectedIsNaN_ShouldReturnFalse()
			{
				float? value = 0.1F;
				float expected = float.NaN;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				float? value = float.NaN;
				float? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}

			[Fact]
			public async Task WhenSubjectIsNaN_ShouldReturnFalse()
			{
				float? value = float.NaN;
				float expected = 0.1F;

				bool result = value.IsConsideredEqualTo(expected, 0.1F);

				await That(result).IsFalse();
			}

			[Fact]
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
			[Theory]
			[InlineData("0.1", "0.0", "0.1")]
			[InlineData("0.0", "0.1", "0.1")]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				string valueString, string expectedString, string toleranceString)
			{
				decimal value = decimal.Parse(valueString, CultureInfo.InvariantCulture);
				decimal expected = decimal.Parse(expectedString, CultureInfo.InvariantCulture);
				decimal tolerance = decimal.Parse(toleranceString, CultureInfo.InvariantCulture);

				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Fact]
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
			[Fact]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				decimal? value = null;
				decimal? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1m);

				await That(result).IsTrue();
			}

			[Theory]
			[InlineData("0.1", "0.0", "0.1")]
			[InlineData("0.0", "0.1", "0.1")]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(
				string valueString, string expectedString, string toleranceString)
			{
				decimal? value = decimal.Parse(valueString, CultureInfo.InvariantCulture);
				decimal? expected = decimal.Parse(expectedString, CultureInfo.InvariantCulture);
				decimal tolerance = decimal.Parse(toleranceString, CultureInfo.InvariantCulture);

				bool result = value.IsConsideredEqualTo(expected, tolerance);

				await That(result).IsTrue();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				decimal? value = decimal.MinValue;
				decimal? expected = null;

				bool result = value.IsConsideredEqualTo(expected, 0.1m);

				await That(result).IsFalse();
			}

			[Fact]
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
			[Fact]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				DateTime? value = null;
				DateTime? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.Zero, out bool hasKindDifference);

				await That(result).IsTrue();
				await That(hasKindDifference).IsFalse();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				DateTime? value = DateTime.Now;
				DateTime? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue, out bool hasKindDifference);

				await That(result).IsFalse();
				await That(hasKindDifference).IsTrue();
			}

			[Fact]
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
			[Fact]
			public async Task WhenDifferenceExceedsTolerance_ShouldReturnFalse()
			{
				DateTimeOffset value = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
				DateTimeOffset expected = value.AddTicks(1);

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.Zero);

				await That(result).IsFalse();
			}

			[Theory]
			[InlineData(1)]
			[InlineData(-1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(int seconds)
			{
				DateTimeOffset value = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
				DateTimeOffset expected = value.AddSeconds(seconds);

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.FromSeconds(1));

				await That(result).IsTrue();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				DateTimeOffset value = DateTimeOffset.MinValue;
				DateTimeOffset? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}

			[Fact]
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
			[Fact]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				DateTimeOffset? value = null;
				DateTimeOffset? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.Zero);

				await That(result).IsTrue();
			}

			[Theory]
			[InlineData(1)]
			[InlineData(-1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(int seconds)
			{
				DateTimeOffset? value = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
				DateTimeOffset? expected = value.Value.AddSeconds(seconds);

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.FromSeconds(1));

				await That(result).IsTrue();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				DateTimeOffset? value = DateTimeOffset.MinValue;
				DateTimeOffset? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}

			[Fact]
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
			[Fact]
			public async Task WhenDifferenceIsOutsideTheRangeOfATimeSpan_ShouldReturnFalse()
			{
				TimeSpan value = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MaxValue;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse()
					.Because("the difference exceeds even the largest tolerance, without overflowing");
			}

			[Theory]
			[InlineData(1)]
			[InlineData(-1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(int seconds)
			{
				TimeSpan value = TimeSpan.FromMinutes(1);
				TimeSpan expected = value.Add(TimeSpan.FromSeconds(seconds));

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.FromSeconds(1));

				await That(result).IsTrue();
			}

			[Fact]
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
			[Fact]
			public async Task WhenBothAreNull_ShouldReturnTrue()
			{
				TimeSpan? value = null;
				TimeSpan? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.Zero);

				await That(result).IsTrue();
			}

			[Theory]
			[InlineData(1)]
			[InlineData(-1)]
			public async Task WhenDifferenceIsTolerance_ShouldReturnTrue(int seconds)
			{
				TimeSpan? value = TimeSpan.FromMinutes(1);
				TimeSpan? expected = value.Value.Add(TimeSpan.FromSeconds(seconds));

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.FromSeconds(1));

				await That(result).IsTrue();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldReturnFalse()
			{
				TimeSpan? value = TimeSpan.Zero;
				TimeSpan? expected = null;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldReturnFalse()
			{
				TimeSpan? value = null;
				TimeSpan? expected = TimeSpan.Zero;

				bool result = value.IsConsideredEqualTo(expected, TimeSpan.MaxValue);

				await That(result).IsFalse();
			}
		}
	}
}
