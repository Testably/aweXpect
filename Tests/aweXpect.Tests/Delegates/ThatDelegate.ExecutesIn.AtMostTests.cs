namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ExecutesIn
	{
		public sealed class AtMostTests
		{
			[Test]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				Action @delegate = () => Task.Delay(50.Milliseconds()).Wait();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenDelegateThrowsAfterExceedingTheLimit_ShouldFail()
			{
				Action @delegate = () =>
				{
					Task.Delay(500.Milliseconds()).Wait();
					throw new MyException();
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.010,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAfterExceedingTheLimit_ShouldFail)}
					              """)
					.Because("the exception is the more relevant cause than the exceeded limit");
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:05,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed in microseconds must not satisfy a timing expectation");
			}
		}
	}
}
