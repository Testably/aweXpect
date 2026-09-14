namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsNegative
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(1.0D)]
			[InlineData(0D)]
			public async Task ForDecimal_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(decimal subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1.0D)]
			public async Task ForDecimal_WhenValueIsLessThanZero_ShouldSucceed(decimal subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.0)]
			[InlineData(0)]
			public async Task ForDouble_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(double subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1.0)]
			public async Task ForDouble_WhenValueIsLessThanZero_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_WhenValueIsNaN_ShouldFail()
			{
				double subject = double.NaN;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForDouble_WhenValueIsNegativeInfinity_ShouldSucceed()
			{
				double subject = double.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_WhenValueIsPositiveInfinity_ShouldFail()
			{
				double subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was +∞
					             """);
			}

			[Theory]
			[InlineData(1.0F)]
			[InlineData(0)]
			public async Task ForFloat_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(float subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1.0F)]
			public async Task ForFloat_WhenValueIsLessThanZero_ShouldSucceed(float subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForFloat_WhenValueIsNaN_ShouldFail()
			{
				float subject = float.NaN;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_WhenValueIsNegativeInfinity_ShouldSucceed()
			{
				float subject = float.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForFloat_WhenValueIsPositiveInfinity_ShouldFail()
			{
				float subject = float.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was +∞
					             """);
			}

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForInt_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(int subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

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
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForInt128_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(
				int subjectValue)
			{
				Int128 subject = subjectValue;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(-1)]
			public async Task ForInt128_WhenValueIsLessThanZero_ShouldSucceed(
				int subjectValue)
			{
				Int128 subject = subjectValue;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}
#endif

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForLong_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(long subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForLong_WhenValueIsLessThanZero_ShouldSucceed(long subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.0)]
			[InlineData(0.0)]
			public async Task ForNullableDecimal_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(
				double value)
			{
				decimal? subject = new(value);

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1.0)]
			public async Task ForNullableDecimal_WhenValueIsLessThanZero_ShouldSucceed(
				double value)
			{
				decimal? subject = new(value);

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableDecimal_WhenValueIsNull_ShouldFail()
			{
				decimal? subject = null;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData(1.0)]
			[InlineData(0.0)]
			public async Task ForNullableDouble_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(
				double? subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1.0)]
			public async Task ForNullableDouble_WhenValueIsLessThanZero_ShouldSucceed(
				double? subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsNaN_ShouldFail()
			{
				double? subject = double.NaN;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsNegativeInfinity_ShouldSucceed()
			{
				double? subject = double.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsNull_ShouldFail()
			{
				double? subject = null;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsPositiveInfinity_ShouldFail()
			{
				double? subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was +∞
					             """);
			}

			[Theory]
			[InlineData(1.0F)]
			[InlineData(0.0F)]
			public async Task ForNullableFloat_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(
				float? subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1.0F)]
			public async Task ForNullableFloat_WhenValueIsLessThanZero_ShouldSucceed(float? subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsNaN_ShouldFail()
			{
				float? subject = float.NaN;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsNegativeInfinity_ShouldSucceed()
			{
				float? subject = float.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsNull_ShouldFail()
			{
				float? subject = null;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsPositiveInfinity_ShouldFail()
			{
				float? subject = float.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was +∞
					             """);
			}

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForNullableInt_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(
				int? subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

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
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail()
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was <null>
					             """);
			}

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForNullableInt128_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(
				int subjectValue)
			{
				Int128 subject = subjectValue;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif
#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(-1)]
			public async Task ForNullableInt128_WhenValueIsLessThanZero_ShouldSucceed(
				int subjectValue)
			{
				Int128 subject = subjectValue;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}
#endif

			[Theory]
			[InlineData(1L)]
			[InlineData(0L)]
			public async Task ForNullableLong_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(
				long? subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1L)]
			public async Task ForNullableLong_WhenValueIsLessThanZero_ShouldSucceed(long? subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail()
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData((sbyte)1)]
			[InlineData((sbyte)0)]
			public async Task ForNullableSbyte_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(
				sbyte? subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((sbyte)-1)]
			public async Task ForNullableSbyte_WhenValueIsLessThanZero_ShouldSucceed(sbyte? subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail()
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData((short)1)]
			[InlineData((short)0)]
			public async Task ForNullableShort_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(
				short? subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((short)-1)]
			public async Task ForNullableShort_WhenValueIsLessThanZero_ShouldSucceed(short? subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail()
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is negative,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForSbyte_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(sbyte subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForSbyte_WhenValueIsLessThanZero_ShouldSucceed(sbyte subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForShort_WhenValueIsGreaterThanOrEqualToZero_ShouldFail(short subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is negative,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForShort_WhenValueIsLessThanZero_ShouldSucceed(short subject)
			{
				async Task Act()
					=> await That(subject).IsNegative();

				await That(Act).DoesNotThrow();
			}
		}
		
		public sealed class NegatedTests
		{
			[Theory]
			[InlineData(1)]
			[InlineData(0)]
			public async Task ForInt_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(int subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsNegative());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForInt_WhenValueIsLessThanZero_ShouldFail(int subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsNegative());

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
			public async Task ForNullableInt_WhenValueIsGreaterThanOrEqualToZero_ShouldSucceed(
				int? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsNegative());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			public async Task ForNullableInt_WhenValueIsLessThanZero_ShouldFail(int? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsNegative());

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
					=> await That(subject).DoesNotComplyWith(it =>
						it.IsNegative());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not negative,
					             but it was <null>
					             """);
			}
		}
	}
}
