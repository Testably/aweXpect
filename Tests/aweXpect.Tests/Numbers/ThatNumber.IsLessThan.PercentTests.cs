namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsLessThan
	{
		public sealed class PercentTests
		{
			[Test]
			public async Task ForDouble_WhenInsideTolerance_ShouldSucceed()
			{
				double subject = 209.0;

				async Task Act()
					=> await That(subject).IsLessThan(200.0).Within(5.0).Percent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(210.0, ", which differs by 10.0")]
			[Arguments(211.0, ", which differs by 11.0")]
			public async Task ForDouble_WhenOutsideTolerance_ShouldFail(double subject, string differsBy)
			{
				async Task Act()
					=> await That(subject).IsLessThan(200.0).Within(5.0).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than 200.0 ± 5.0%,
					              but it was {Formatter.Format(subject)}{differsBy}
					              """)
					.Because("5% of 200 shifts the strict bound to 210, which stays excluded");
			}
		}
	}
}
