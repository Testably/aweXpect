namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsBetween
	{
		public sealed class WithinTests
		{
			[Test]
			[Arguments((byte)1, (byte)2, (byte)8)]
			[Arguments((byte)2, (byte)2, (byte)8)]
			[Arguments((byte)8, (byte)2, (byte)8)]
			[Arguments((byte)9, (byte)2, (byte)8)]
			public async Task ForByte_WhenInsideToleranceWidenedRange_ShouldSucceed(
				byte subject, byte minimum, byte maximum)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((byte)0, (byte)2, (byte)8, "-2 from the minimum")]
			[Arguments((byte)10, (byte)2, (byte)8, "2 from the maximum")]
			public async Task ForByte_WhenOutsideToleranceWidenedRange_ShouldFail(
				byte subject, byte minimum, byte maximum, string difference)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage(
						$"""
						 Expected that subject
						 is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 1,
						 but it was {Formatter.Format(subject)}, which differs by {difference}
						 """);
			}

			[Test]
			[Arguments(11.9, 12.0, 14.0)]
			[Arguments(14.1, 12.0, 14.0)]
			public async Task ForDecimal_WhenInsideTolerance_ShouldSucceed(
				double subjectValue, double minimumValue, double maximumValue)
			{
				decimal subject = new(subjectValue);
				decimal minimum = new(minimumValue);
				decimal maximum = new(maximumValue);

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum)
						.Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(11.0, 12.0, 14.0)]
			public async Task ForDecimal_WhenOutsideTolerance_ShouldFail(
				double subjectValue, double minimumValue, double maximumValue)
			{
				decimal subject = new(subjectValue);
				decimal minimum = new(minimumValue);
				decimal maximum = new(maximumValue);

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum)
						.Within(new decimal(0.1));

				await That(Act).Throws<FailException>()
					.WithMessage(
						$"""
						 Expected that subject
						 is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0.1,
						 but it was {Formatter.Format(subject)}, which differs by -1.0 from the minimum
						 """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal subject)
			{
				decimal minimum = new(0);
				decimal maximum = new(10);

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(11.9, 12.0, 14.0)]
			[Arguments(14.1, 12.0, 14.0)]
			public async Task ForDouble_WhenInsideTolerance_ShouldSucceed(
				double subject, double minimum, double maximum)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForDouble_WhenMinimumIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double subject = 13.0;
				double minimum = double.NaN;
				double maximum = 14.0;

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("minimum").And
					.WithMessage("The minimum must not be NaN.").AsPrefix();
			}

			[Test]
			[Arguments(11.0, 12.0, 14.0)]
			public async Task ForDouble_WhenOutsideTolerance_ShouldFail(
				double subject, double minimum, double maximum)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(0.1);

				await That(Act).Throws<FailException>()
					.WithMessage(
						$"""
						 Expected that subject
						 is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0.1,
						 but it was {Formatter.Format(subject)}, which differs by -1.0 from the minimum
						 """);
			}

			[Test]
			[Arguments(14.1, 12.0, 14.0)]
			public async Task ForDouble_WhenOutsideToleranceOnUpperBound_ShouldFail(
				double subject, double minimum, double maximum)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(0.05);

				await That(Act).Throws<FailException>()
					.WithMessage(
						$"""
						 Expected that subject
						 is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0.05,
						 but it was {Formatter.Format(subject)}, which differs by 0.0999999999999996 from the maximum
						 """);
			}

			[Test]
			public async Task ForDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5).IsBetween(12.0).And(13.0).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}

			[Test]
			[Arguments(11.9f, 12.0f, 14.0f)]
			[Arguments(14.1f, 12.0f, 14.0f)]
			public async Task ForFloat_WhenInsideTolerance_ShouldSucceed(
				float subject, float minimum, float maximum)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(0.11f);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(11.0f, 12.0f, 14.0f, "-1.0 from the minimum")]
			[Arguments(15.0f, 12.0f, 14.0f, "1.0 from the maximum")]
			public async Task ForFloat_WhenOutsideToleranceWidenedRange_ShouldFail(
				float subject, float minimum, float maximum, string difference)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(0.1f);

				await That(Act).Throws<FailException>()
					.WithMessage(
						$"""
						 Expected that subject
						 is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0.1,
						 but it was {Formatter.Format(subject)}, which differs by {difference}
						 """);
			}

			[Test]
			[Arguments(1, 2, 8)]
			[Arguments(2, 2, 8)]
			[Arguments(8, 2, 8)]
			[Arguments(9, 2, 8)]
			public async Task ForInt_WhenInsideToleranceWidenedRange_ShouldSucceed(
				int subject, int minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(0, 2, 8, "-2 from the minimum")]
			[Arguments(10, 2, 8, "2 from the maximum")]
			public async Task ForInt_WhenOutsideToleranceWidenedRange_ShouldFail(
				int subject, int minimum, int maximum, string difference)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage(
						$"""
						 Expected that subject
						 is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 1,
						 but it was {Formatter.Format(subject)}, which differs by {difference}
						 """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int subject)
			{
				async Task Act()
					=> await That(subject).IsBetween(0).And(10).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(1L, 2L, 8L)]
			[Arguments(9L, 2L, 8L)]
			public async Task ForLong_WhenInsideToleranceWidenedRange_ShouldSucceed(
				long subject, long minimum, long maximum)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(1L);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(0L, 2L, 8L, "-2 from the minimum")]
			[Arguments(10L, 2L, 8L, "2 from the maximum")]
			public async Task ForLong_WhenOutsideToleranceWidenedRange_ShouldFail(
				long subject, long minimum, long maximum, string difference)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(1L);

				await That(Act).Throws<FailException>()
					.WithMessage(
						$"""
						 Expected that subject
						 is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 1,
						 but it was {Formatter.Format(subject)}, which differs by {difference}
						 """);
			}

			[Test]
			[Arguments(1, 2, 8)]
			[Arguments(9, 2, 8)]
			public async Task ForNullableInt_WhenInsideToleranceWidenedRange_ShouldSucceed(
				int? subject, int? minimum, int? maximum)
			{
				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableInt_WhenSubjectIsNull_ShouldFail()
			{
				int? subject = null;
				int? minimum = 2;
				int? maximum = 8;

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is between 2 and 8 ± 1,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task ForSByte_WhenDifferenceWouldOverflow_ShouldFailWithoutThrowing()
			{
				sbyte subject = sbyte.MinValue;
				sbyte minimum = 0;
				sbyte maximum = sbyte.MaxValue;

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum).Within((sbyte)1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is between 0 and 127 ± 1,
					             but it was -128, which differs by -128 from the minimum
					             """)
					.Because("the difference must not overflow the range of sbyte");
			}

			[Test]
			[Arguments(1, 2, 8, "-1 from the minimum")]
			[Arguments(9, 2, 8, "1 from the maximum")]
			public async Task IsNotBetween_ForInt_WhenInsideToleranceWidenedRange_ShouldFail(
				int subject, int minimum, int maximum, string difference)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage(
						$"""
						 Expected that subject
						 is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 1,
						 but it was {Formatter.Format(subject)}, which differs by {difference}
						 """);
			}

			[Test]
			[Arguments(0, 2, 8)]
			[Arguments(10, 2, 8)]
			public async Task IsNotBetween_ForInt_WhenOutsideToleranceWidenedRange_ShouldSucceed(
				int subject, int minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum).Within(1);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
