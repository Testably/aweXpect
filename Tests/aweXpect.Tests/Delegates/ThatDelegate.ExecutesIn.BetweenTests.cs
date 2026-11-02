using aweXpect.Core;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ExecutesIn
	{
		public sealed class BetweenTests
		{
			[Test]
			public async Task WhenDelegateIsTooFast_ShouldFail()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Action @delegate = () => timeSystem.TimeProvider.AdvanceBy(5.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(5123.Milliseconds()).And(6000.Milliseconds())
						.WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in between 0:05.123 and 0:06,
					             but it took only 0:*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenDelegateTakesLongEnough_ShouldSucceed()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Action @delegate = () => timeSystem.TimeProvider.AdvanceBy(50.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(10.Milliseconds()).And(5000.Milliseconds())
						.WithTimeProvider(time);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateTakesTooLong_ShouldFail()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Action @delegate = () => timeSystem.TimeProvider.AdvanceBy(50.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(5.Milliseconds()).And(10.Milliseconds())
						.WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in between 0:00.005 and 0:00.010,
					             but it took 0:*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Action @delegate = () =>
				{
					timeSystem.TimeProvider.AdvanceBy(500.Milliseconds());
					throw new MyException();
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(5.Milliseconds()).And(50.Seconds())
						.WithTimeProvider(time);

				await That(Act).Throws<FailException>()
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
