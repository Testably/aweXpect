namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsPositive
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(1.0D)]
			public async Task ForDecimal_WhenValueIsGreaterThanZero_ShouldSucceed(decimal subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0D)]
			[InlineData(0D)]
			public async Task ForDecimal_WhenValueIsLessThanOrEqualToZero_ShouldFail(decimal subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1.0)]
			public async Task ForDouble_WhenValueIsGreaterThanZero_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0)]
			[InlineData(0)]
			public async Task ForDouble_WhenValueIsLessThanOrEqualToZero_ShouldFail(double subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForDouble_WhenValueIsNaN_ShouldFail()
			{
				double subject = double.NaN;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was NaN
					             """);
			}

			[Fact]
			public async Task ForDouble_WhenValueIsNegativeInfinity_ShouldFail()
			{
				double subject = double.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was -∞
					             """);
			}

			[Fact]
			public async Task ForDouble_WhenValueIsPositiveInfinity_ShouldSucceed()
			{
				double subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.0F)]
			public async Task ForFloat_WhenValueIsGreaterThanZero_ShouldSucceed(float subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0F)]
			[InlineData(0)]
			public async Task ForFloat_WhenValueIsLessThanOrEqualToZero_ShouldFail(float subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_WhenValueIsNaN_ShouldFail()
			{
				float subject = float.NaN;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was NaN
					             """);
			}

			[Fact]
			public async Task ForFloat_WhenValueIsNegativeInfinity_ShouldFail()
			{
				float subject = float.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was -∞
					             """);
			}

			[Fact]
			public async Task ForFloat_WhenValueIsPositiveInfinity_ShouldSucceed()
			{
				float subject = float.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1)]
			public async Task ForInt_WhenValueIsGreaterThanZero_ShouldSucceed(int subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			[InlineData(0)]
			public async Task ForInt_WhenValueIsLessThanOrEqualToZero_ShouldFail(int subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1)]
			public async Task ForInt128_WhenValueIsGreaterThanZero_ShouldSucceed(
				int subjectValue)
			{
				Int128 subject = subjectValue;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(-1)]
			[InlineData(0)]
			public async Task ForInt128_WhenValueIsLessThanOrEqualZero_ShouldFail(
				int subjectValue)
			{
				Int128 subject = subjectValue;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

			[Theory]
			[InlineData(1)]
			public async Task ForLong_WhenValueIsGreaterThanZero_ShouldSucceed(long subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			[InlineData(0)]
			public async Task ForLong_WhenValueIsLessThanOrEqualToZero_ShouldFail(long subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1.0)]
			public async Task ForNullableDecimal_WhenValueIsGreaterThanZero_ShouldSucceed(
				double value)
			{
				decimal? subject = new(value);

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0)]
			[InlineData(0.0)]
			public async Task ForNullableDecimal_WhenValueIsLessThanOrEqualToZero_ShouldFail(
				double value)
			{
				decimal? subject = new(value);

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableDecimal_WhenValueIsNull_ShouldFail()
			{
				decimal? subject = null;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData(1.0)]
			public async Task ForNullableDouble_WhenValueIsGreaterThanZero_ShouldSucceed(
				double? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0)]
			[InlineData(0.0)]
			public async Task ForNullableDouble_WhenValueIsLessThanOrEqualToZero_ShouldFail(
				double? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsNaN_ShouldFail()
			{
				double? subject = double.NaN;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was NaN
					             """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsNegativeInfinity_ShouldFail()
			{
				double? subject = double.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was -∞
					             """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsNull_ShouldFail()
			{
				double? subject = null;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueIsPositiveInfinity_ShouldSucceed()
			{
				double? subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.0F)]
			public async Task ForNullableFloat_WhenValueIsGreaterThanZero_ShouldSucceed(float? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1.0F)]
			[InlineData(0.0F)]
			public async Task ForNullableFloat_WhenValueIsLessThanOrEqualToZero_ShouldFail(
				float? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsNaN_ShouldFail()
			{
				float? subject = float.NaN;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was NaN
					             """);
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsNegativeInfinity_ShouldFail()
			{
				float? subject = float.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was -∞
					             """);
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsNull_ShouldFail()
			{
				float? subject = null;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueIsPositiveInfinity_ShouldSucceed()
			{
				float? subject = float.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1)]
			public async Task ForNullableInt_WhenValueIsGreaterThanZero_ShouldSucceed(int? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			[InlineData(0)]
			public async Task ForNullableInt_WhenValueIsLessThanOrEqualToZero_ShouldFail(int? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail()
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was <null>
					             """);
			}
#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1)]
			public async Task ForNullableInt128_WhenValueIsGreaterThanZero_ShouldSucceed(
				int subjectValue)
			{
				Int128 subject = subjectValue;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(-1)]
			[InlineData(0)]
			public async Task ForNullableInt128_WhenValueIsLessThanOrEqualToZero_ShouldFail(
				int subjectValue)
			{
				Int128 subject = subjectValue;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

			[Theory]
			[InlineData(1L)]
			public async Task ForNullableLong_WhenValueIsGreaterThanZero_ShouldSucceed(long? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1L)]
			[InlineData(0L)]
			public async Task ForNullableLong_WhenValueIsLessThanOrEqualToZero_ShouldFail(long? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail()
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData((sbyte)1)]
			public async Task ForNullableSbyte_WhenValueIsGreaterThanZero_ShouldSucceed(sbyte? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)-1)]
			[InlineData((sbyte)0)]
			public async Task ForNullableSbyte_WhenValueIsLessThanOrEqualToZero_ShouldFail(
				sbyte? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail()
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData((short)1)]
			public async Task ForNullableShort_WhenValueIsGreaterThanZero_ShouldSucceed(short? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)-1)]
			[InlineData((short)0)]
			public async Task ForNullableShort_WhenValueIsLessThanOrEqualToZero_ShouldFail(
				short? subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail()
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData(1)]
			public async Task ForSbyte_WhenValueIsGreaterThanZero_ShouldSucceed(sbyte subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			[InlineData(0)]
			public async Task ForSbyte_WhenValueIsLessThanOrEqualToZero_ShouldFail(sbyte subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1)]
			public async Task ForShort_WhenValueIsGreaterThanZero_ShouldSucceed(short subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(-1)]
			[InlineData(0)]
			public async Task ForShort_WhenValueIsLessThanOrEqualToZero_ShouldFail(short subject)
			{
				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
		
		public sealed class NegatedTests
		{
			[Theory]
			[InlineData(-1)]
			[InlineData(0)]
			public async Task ForInt_WhenValueIsLessThanOrEqualToZero_ShouldSucceed(int subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsPositive());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1)]
			public async Task ForInt_WhenValueIsGreaterThanZero_ShouldFail(int subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsPositive());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}
			
			[Theory]
			[InlineData(-1)]
			[InlineData(0)]
			public async Task ForNullableInt_WhenValueIsLessThanOrEqualToZero_ShouldSucceed(
				int? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsPositive());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1)]
			public async Task ForNullableInt_WhenValueIsGreaterThanZero_ShouldFail(int? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsPositive());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail()
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.IsPositive());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not positive,
					             but it was <null>
					             """);
			}
		}
	}
}
