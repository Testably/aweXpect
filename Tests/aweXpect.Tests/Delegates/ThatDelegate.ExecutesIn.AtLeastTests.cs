namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ExecutesIn
	{
		public sealed class AtLeastTests
		{
			[Fact]
			public async Task WhenDelegateIsTooFast_ShouldFail()
			{
				Action @delegate = () => Task.Delay(5.Milliseconds()).Wait();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtLeast(5123.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at least 0:05.123,
					             but it took only 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateTakesLongEnough_ShouldSucceed()
			{
				Action @delegate = () => Task.Delay(50.Milliseconds()).Wait();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtLeast(10.Milliseconds());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
