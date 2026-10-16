namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsEqualTo
	{
		public sealed class PercentTests
		{
			[Test]
			[Arguments(95.0)]
			[Arguments(105.0)]
			public async Task ForDecimal_WhenInsideTolerance_ShouldSucceed(double subjectValue)
			{
				decimal subject = new(subjectValue);

				async Task Act()
					=> await That(subject).IsEqualTo(100m).Within(5m).Percent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForDecimal_WhenOutsideTolerance_ShouldFail()
			{
				decimal subject = 106m;

				async Task Act()
					=> await That(subject).IsEqualTo(100m).Within(5m).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 100.0 ± 5.0%,
					             but it was 106.0, which differs by 6.0
					             """);
			}

			[Test]
			[Arguments(-105.0)]
			[Arguments(-95.0)]
			public async Task ForDouble_WhenExpectedIsNegative_ShouldUseItsMagnitude(double subject)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(-100.0).Within(5.0).Percent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForDouble_WhenExpectedIsZero_ShouldRequireEquality()
			{
				double subject = 0.001;

				async Task Act()
					=> await That(subject).IsEqualTo(0.0).Within(5.0).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0.0 ± 5.0%,
					             but it was 0.001, which differs by 0.001
					             """)
					.Because("a percentage of zero is a tolerance of zero");
			}

			[Test]
			[Arguments(95.0)]
			[Arguments(105.0)]
			public async Task ForDouble_WhenInsideTolerance_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(100.0).Within(5.0).Percent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(94.0, "-6.0")]
			[Arguments(106.0, "6.0")]
			public async Task ForDouble_WhenOutsideTolerance_ShouldFail(double subject, string differsBy)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(100.0).Within(5.0).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to 100.0 ± 5.0%,
					              but it was {Formatter.Format(subject)}, which differs by {differsBy}
					              """);
			}

			[Test]
			[Arguments(95f)]
			[Arguments(105f)]
			public async Task ForFloat_WhenInsideTolerance_ShouldSucceed(float subject)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(100f).Within(5f).Percent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForFloat_WhenOutsideTolerance_ShouldFail()
			{
				float subject = 106f;

				async Task Act()
					=> await That(subject).IsEqualTo(100f).Within(5f).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 100.0 ± 5.0%,
					             but it was 106.0, which differs by 6.0
					             """);
			}

			[Test]
			public async Task ForNullableDouble_WhenInsideTolerance_ShouldSucceed()
			{
				double? subject = 105.0;

				async Task Act()
					=> await That(subject).IsEqualTo(100.0).Within(5.0).Percent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableDouble_WhenOutsideTolerance_ShouldFail()
			{
				double? subject = 106.0;

				async Task Act()
					=> await That(subject).IsEqualTo(100.0).Within(5.0).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 100.0 ± 5.0%,
					             but it was 106.0, which differs by 6.0
					             """);
			}

			[Test]
			public async Task WhenCombinedWithAnotherExpectation_ShouldApplyThePercentageOnlyToTheFirst()
			{
				double subject = 105.0;

				async Task Act()
					=> await That(subject).IsEqualTo(100.0).Within(5.0).Percent()
						.And.IsLessThan(104.0).Within(0.5);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 100.0 ± 5.0% and is less than 104.0 ± 0.5,
					             but it was 105.0, which differs by 1.0
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForHalf_WhenInsideTolerance_ShouldSucceed()
			{
				Half subject = (Half)105;

				async Task Act()
					=> await That(subject).IsEqualTo((Half)100).Within((Half)5).Percent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForHalf_WhenOutsideTolerance_ShouldFail()
			{
				Half subject = (Half)106;

				async Task Act()
					=> await That(subject).IsEqualTo((Half)100).Within((Half)5).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 100.0 ± 5.0%,
					             but it was 106.0, which differs by 6.0
					             """);
			}
#endif
		}
	}
}
