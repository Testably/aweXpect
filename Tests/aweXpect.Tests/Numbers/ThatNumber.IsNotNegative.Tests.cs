namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsNotNegative
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(1.0D)]
			[InlineData(0D)]
			public async Task ForDecimal_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(decimal subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0D)]
			public async Task ForDecimal_WhenValueIsLessThanZero_ShouldFail(decimal subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1.0)]
			[InlineData(0)]
			public async Task ForDouble_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0)]
			public async Task ForDouble_WhenValueIsLessThanZero_ShouldFail(double subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForDouble_WhenValueIsNaN_ShouldSucceed()
			{
				double subject = double.NaN;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow().Because("NaN is neither positive nor negative");
			}

			[Fact]
			public async Task ForDouble_WhenValueIsNegativeInfinity_ShouldFail()
			{
				double subject = double.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was -∞
					             """);
			}

			[Fact]
			public async Task ForDouble_WhenValueIsNegativeZero_ShouldSucceed()
			{
				double subject = -0.0;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow().Because("negative zero is equal to zero, which is not negative");
			}

			[Fact]
			public async Task ForDouble_WhenValueIsPositiveInfinity_ShouldSucceed()
			{
				double subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.0F)]
			[InlineData(0)]
			public async Task ForFloat_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(float subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0F)]
			public async Task ForFloat_WhenValueIsLessThanZero_ShouldFail(float subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_WhenValueIsNaN_ShouldSucceed()
			{
				float subject = float.NaN;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow().Because("NaN is neither positive nor negative");
			}

			[Fact]
			public async Task ForFloat_WhenValueIsNegativeInfinity_ShouldFail()
			{
				float subject = float.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was -∞
					             """);
			}

			[Fact]
			public async Task ForFloat_WhenValueIsNegativeZero_ShouldSucceed()
			{
				float subject = -0.0F;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow().Because("negative zero is equal to zero, which is not negative");
			}

			[Fact]
			public async Task ForFloat_WhenValueIsPositiveInfinity_ShouldSucceed()
			{
				float subject = float.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForInt_ShouldSupportChaining()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).IsNotNegative()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForInt_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(int subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForInt_WhenValueIsLessThanZero_ShouldFail(int subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForInt128_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(
				int subjectValue)
			{
				Int128 subject = subjectValue;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(-1)]
			public async Task ForInt128_WhenValueIsLessThanZero_ShouldFail(
				int subjectValue)
			{
				Int128 subject = subjectValue;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForLong_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(long subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForLong_WhenValueIsLessThanZero_ShouldFail(long subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1.0)]
			[InlineData(0.0)]
			public async Task ForNullableDecimal_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(
				double value)
			{
				decimal? subject = new(value);

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0)]
			public async Task ForNullableDecimal_WhenValueIsLessThanZero_ShouldFail(
				double value)
			{
				decimal? subject = new(value);

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableDecimal_WhenValueIsNull_ShouldFail()
			{
				decimal? subject = null;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData(1.0)]
			[InlineData(0.0)]
			public async Task ForNullableDouble_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(
				double? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0)]
			public async Task ForNullableDouble_WhenValueIsLessThanZero_ShouldFail(
				double? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsNaN_ShouldSucceed()
			{
				double? subject = double.NaN;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow().Because("NaN is neither positive nor negative");
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsNegativeInfinity_ShouldFail()
			{
				double? subject = double.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was -∞
					             """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsNull_ShouldFail()
			{
				double? subject = null;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsPositiveInfinity_ShouldSucceed()
			{
				double? subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.0F)]
			[InlineData(0.0F)]
			public async Task ForNullableFloat_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(
				float? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0F)]
			public async Task ForNullableFloat_WhenValueIsLessThanZero_ShouldFail(float? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsNaN_ShouldSucceed()
			{
				float? subject = float.NaN;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow().Because("NaN is neither positive nor negative");
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsNegativeInfinity_ShouldFail()
			{
				float? subject = float.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was -∞
					             """);
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsNull_ShouldFail()
			{
				float? subject = null;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsPositiveInfinity_ShouldSucceed()
			{
				float? subject = float.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableInt_ShouldSupportChaining()
			{
				int? subject = 1;

				async Task Act()
					=> await That(subject).IsNotNegative()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForNullableInt_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(int? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForNullableInt_WhenValueIsLessThanZero_ShouldFail(int? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail()
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was <null>
					             """);
			}

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForNullableInt128_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(
				int subjectValue)
			{
				Int128? subject = subjectValue;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(-1)]
			public async Task ForNullableInt128_WhenValueIsLessThanZero_ShouldFail(
				int subjectValue)
			{
				Int128? subject = subjectValue;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableInt128_WhenValueIsNull_ShouldFail()
			{
				Int128? subject = null;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was <null>
					             """);
			}
#endif

			[Theory]
			[InlineData(1L)]
			[InlineData(0L)]
			public async Task ForNullableLong_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(long? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1L)]
			public async Task ForNullableLong_WhenValueIsLessThanZero_ShouldFail(long? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail()
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData((sbyte)1)]
			[InlineData((sbyte)0)]
			public async Task ForNullableSbyte_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(
				sbyte? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)-1)]
			public async Task ForNullableSbyte_WhenValueIsLessThanZero_ShouldFail(sbyte? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail()
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData((short)1)]
			[InlineData((short)0)]
			public async Task ForNullableShort_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(
				short? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)-1)]
			public async Task ForNullableShort_WhenValueIsLessThanZero_ShouldFail(short? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail()
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForSbyte_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(sbyte subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForSbyte_WhenValueIsLessThanZero_ShouldFail(sbyte subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForShort_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(short subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForShort_WhenValueIsLessThanZero_ShouldFail(short subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1U)]
			[InlineData(0U)]
			public async Task ForUint_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(uint subject)
			{
				async Task Act()
					=> await That(subject).IsNotNegative();

				await That(Act).DoesNotThrow().Because("an unsigned value is never negative");
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task ForDouble_WhenValueIsNaN_ShouldFail()
			{
				double subject = double.NaN;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.IsNotNegative());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was NaN
					             """);
			}

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForInt_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(int subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.IsNotNegative());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForInt_WhenValueIsLessThanZero_ShouldSucceed(int subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.IsNotNegative());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForNullableInt_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(int? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.IsNotNegative());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForNullableInt_WhenValueIsLessThanZero_ShouldSucceed(int? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.IsNotNegative());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail()
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.IsNotNegative());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was <null>
					             """);
			}
		}
	}
}
