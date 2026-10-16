namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsBetween
	{
		public sealed class PercentTests
		{
			[Test]
			[Arguments(95.0)]
			[Arguments(210.0)]
			public async Task ForDouble_WhenInsideToleranceOfABound_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).IsBetween(100.0).And(200.0).Within(5.0).Percent();

				await That(Act).DoesNotThrow()
					.Because("5% widens the minimum by 5 and the maximum by 10");
			}

			[Test]
			[Arguments(94.0, "-6.0 from the minimum")]
			[Arguments(211.0, "11.0 from the maximum")]
			public async Task ForDouble_WhenOutsideToleranceOfABound_ShouldFail(double subject, string differsBy)
			{
				async Task Act()
					=> await That(subject).IsBetween(100.0).And(200.0).Within(5.0).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is between 100.0 and 200.0 ± 5.0%,
					              but it was {Formatter.Format(subject)}, which differs by {differsBy}
					              """);
			}
		}
	}
}
