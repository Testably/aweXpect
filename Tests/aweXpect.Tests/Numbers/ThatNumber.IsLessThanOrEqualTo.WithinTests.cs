namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsLessThanOrEqualTo
	{
		public sealed class WithinTests
		{
			[Test]
			[Arguments((byte)4, (byte)5)]
			[Arguments((byte)5, (byte)5)]
			[Arguments((byte)6, (byte)5)]
			public async Task ForByte_WhenInsideTolerance_ShouldSucceed(
				byte subject, byte expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((byte)7, (byte)5, ", which differs by 2")]
			[Arguments((byte)10, (byte)5, ", which differs by 5")]
			public async Task ForByte_WhenOutsideTolerance_ShouldFail(
				byte subject, byte expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)} ± 1,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(12.5, 12.5)]
			[Arguments(12.6, 12.5)]
			public async Task ForDecimal_WhenInsideTolerance_ShouldSucceed(
				double subjectValue, double expectedValue)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(13.0, 12.5, ", which differs by 0.5")]
			public async Task ForDecimal_WhenOutsideTolerance_ShouldFail(
				double subjectValue, double expectedValue, string expectedDifference)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(new decimal(0.1));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)} ± 0.1,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal subject, decimal expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			public async Task ForDouble_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double subject = 5.0;
				double expected = double.NaN;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(1.0);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("no tolerance can bring a value within reach of NaN");
			}

			[Test]
			[Arguments(12.5, 12.5)]
			[Arguments(12.6, 12.5)]
			public async Task ForDouble_WhenInsideTolerance_ShouldSucceed(
				double subject, double expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(13.0, 12.5, ", which differs by 0.5")]
			public async Task ForDouble_WhenOutsideTolerance_ShouldFail(
				double subject, double expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(0.1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)} ± 0.1,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			public async Task ForDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5).IsLessThanOrEqualTo(13.0).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}

			[Test]
			[Arguments(12.5f, 12.5f)]
			[Arguments(12.6f, 12.5f)]
			public async Task ForFloat_WhenInsideTolerance_ShouldSucceed(
				float subject, float expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(0.11f);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(13.0f, 12.5f, ", which differs by 0.5")]
			public async Task ForFloat_WhenOutsideTolerance_ShouldFail(
				float subject, float expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(0.1f);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)} ± 0.1,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForFloat_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float subject, float expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(-0.1f);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(4, 5)]
			[Arguments(5, 5)]
			[Arguments(6, 5)]
			public async Task ForInt_WhenInsideTolerance_ShouldSucceed(int subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(7, 5, ", which differs by 2")]
			[Arguments(10, 5, ", which differs by 5")]
			public async Task ForInt_WhenOutsideTolerance_ShouldFail(int subject, int expected,
				string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)} ± 1,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(5L, 5L)]
			[Arguments(6L, 5L)]
			public async Task ForLong_WhenInsideTolerance_ShouldSucceed(long subject, long expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(1L);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(7L, 5L, ", which differs by 2")]
			public async Task ForLong_WhenOutsideTolerance_ShouldFail(long subject, long expected,
				string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(1L);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)} ± 1,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(5, 5)]
			[Arguments(6, 5)]
			public async Task ForNullableInt_WhenInsideTolerance_ShouldSucceed(
				int? subject, int? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableInt_WhenSubjectIsNull_ShouldFail()
			{
				int? subject = null;
				int? expected = 5;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than or equal to 5 ± 1,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task ForSByte_WhenDifferenceWouldOverflow_ShouldFailWithoutThrowing()
			{
				sbyte subject = sbyte.MaxValue;
				sbyte expected = sbyte.MinValue;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within((sbyte)1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than or equal to -128 ± 1,
					             but it was 127, which differs by 255
					             """)
					.Because("the difference must not overflow the range of sbyte");
			}

			[Test]
			public async Task WhenToleranceIsNotSet_ShouldNotAllowLargerValues()
			{
				int subject = 6;
				int expected = 5;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than or equal to 5,
					             but it was 6, which differs by 1
					             """);
			}

			[Test]
			public async Task WhenToleranceIsZero_ShouldNotAllowLargerValues()
			{
				int subject = 6;
				int expected = 5;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected).Within(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than or equal to 5 ± 0,
					             but it was 6, which differs by 1
					             """);
			}
		}
	}
}
