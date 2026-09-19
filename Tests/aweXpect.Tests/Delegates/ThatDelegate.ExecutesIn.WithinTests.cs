namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ExecutesIn
	{
		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenDelegateIsTooFast_ShouldFail()
			{
				Action @delegate = () => Task.Delay(5.Milliseconds()).Wait();

				async Task Act()
					=> await That(@delegate).ExecutesIn(10000.Milliseconds()).Within(1123.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in approximately 0:10 ± 0:01.123,
					             but it took only 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDelegateTakesLongEnough_ShouldSucceed()
			{
				Action @delegate = () => Task.Delay(50.Milliseconds()).Wait();

				async Task Act()
					=> await That(@delegate).ExecutesIn(50.Milliseconds()).Within(5.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDelegateTakesTooLong_ShouldFail()
			{
				Action @delegate = () => Task.Delay(50.Milliseconds()).Wait();

				async Task Act()
					=> await That(@delegate).ExecutesIn(10.Milliseconds()).Within(5.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in approximately 0:00.010 ± 0:00.005,
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
					=> await That(@delegate).ExecutesIn(500.Milliseconds()).Within(50.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in approximately 0:00.500 ± 0:50,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a crashed delegate must fail even though its duration was inside the tolerance");
			}

			[Fact]
			public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).ExecutesIn(50.Milliseconds()).Within(-1.Milliseconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be negative").AsPrefix();
			}
		}
	}
}
