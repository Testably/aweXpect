namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class PercentTests
		{
			[Test]
			public async Task ForDouble_WhenInsideTolerance_ShouldFail()
			{
				double subject = 105.0;

				async Task Act()
					=> await That(subject).IsNotEqualTo(100.0).Within(5.0).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 100.0 ± 5.0%,
					             but it was 105.0, which differs by 5.0
					             """);
			}

			[Test]
			public async Task ForDouble_WhenOutsideTolerance_ShouldSucceed()
			{
				double subject = 106.0;

				async Task Act()
					=> await That(subject).IsNotEqualTo(100.0).Within(5.0).Percent();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
