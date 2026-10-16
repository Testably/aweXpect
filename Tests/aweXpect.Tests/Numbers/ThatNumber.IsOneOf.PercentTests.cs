namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsOneOf
	{
		public sealed class PercentTests
		{
			[Test]
			public async Task ForDouble_WhenInsideToleranceOfOneValue_ShouldSucceed()
			{
				double subject = 209.0;

				async Task Act()
					=> await That(subject).IsOneOf(100.0, 200.0).Within(5.0).Percent();

				await That(Act).DoesNotThrow()
					.Because("5% of 200 is a tolerance of 10");
			}

			[Test]
			public async Task ForDouble_WhenOutsideTolerance_ShouldFail()
			{
				double subject = 106.0;

				async Task Act()
					=> await That(subject).IsOneOf(100.0, 200.0).Within(5.0).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is one of [100.0, 200.0] ± 5.0%,
					             but it was 106.0, which differs by 6.0 from the closest value
					             """)
					.Because("5% of 100 is only a tolerance of 5");
			}
		}
	}
}
