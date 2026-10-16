namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsLessThanOrEqualTo
	{
		public sealed class PercentTests
		{
			[Test]
			public async Task ForDouble_WhenInsideTolerance_ShouldSucceed()
			{
				double subject = 210.0;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(200.0).Within(5.0).Percent();

				await That(Act).DoesNotThrow()
					.Because("5% of 200 shifts the inclusive bound to 210");
			}

			[Test]
			public async Task ForDouble_WhenOutsideTolerance_ShouldFail()
			{
				double subject = 211.0;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(200.0).Within(5.0).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than or equal to 200.0 ± 5.0%,
					             but it was 211.0, which differs by 11.0
					             """);
			}
		}
	}
}
