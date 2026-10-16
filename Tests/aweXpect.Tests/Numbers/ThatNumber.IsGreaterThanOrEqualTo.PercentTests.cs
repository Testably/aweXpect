namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsGreaterThanOrEqualTo
	{
		public sealed class PercentTests
		{
			[Test]
			public async Task ForDouble_WhenInsideTolerance_ShouldSucceed()
			{
				double subject = 190.0;

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(200.0).Within(5.0).Percent();

				await That(Act).DoesNotThrow()
					.Because("5% of 200 shifts the inclusive bound to 190");
			}

			[Test]
			public async Task ForDouble_WhenOutsideTolerance_ShouldFail()
			{
				double subject = 189.0;

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(200.0).Within(5.0).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than or equal to 200.0 ± 5.0%,
					             but it was 189.0, which differs by -11.0
					             """);
			}
		}
	}
}
