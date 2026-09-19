namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ExecutesIn
	{
		public sealed class BetweenTests
		{
			[Fact]
			public async Task WhenDelegateIsTooFast_ShouldFail()
			{
				Action @delegate = () => Task.Delay(5.Milliseconds()).Wait();

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(5123.Milliseconds()).And(6000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in between 0:05.123 and 0:06,
					             but it took only 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateTakesLongEnough_ShouldSucceed()
			{
				Action @delegate = () => Task.Delay(50.Milliseconds()).Wait();

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(10.Milliseconds()).And(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesTooLong_ShouldFail()
			{
				Action @delegate = () => Task.Delay(50.Milliseconds()).Wait();

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(5.Milliseconds()).And(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in between 0:00.005 and 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Action @delegate = () =>
				{
					Task.Delay(500.Milliseconds()).Wait();
					throw new MyException();
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(5.Milliseconds()).And(50.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in between 0:00.005 and 0:50,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a crashed delegate must fail even though its duration was inside the range");
			}
		}
	}
}
