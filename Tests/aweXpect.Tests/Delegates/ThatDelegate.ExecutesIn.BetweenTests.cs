using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;

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
				VirtualTimeSystem time = new();
				Action @delegate = () => time.Advance(5.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(5123.Milliseconds()).And(6000.Milliseconds())
						.WithTimeSystem(time);

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
				VirtualTimeSystem time = new();
				Action @delegate = () => time.Advance(50.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(10.Milliseconds()).And(5000.Milliseconds())
						.WithTimeSystem(time);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateTakesTooLong_ShouldFail()
			{
				VirtualTimeSystem time = new();
				Action @delegate = () => time.Advance(50.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(5.Milliseconds()).And(10.Milliseconds())
						.WithTimeSystem(time);

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
				VirtualTimeSystem time = new();
				Action @delegate = () =>
				{
					time.Advance(500.Milliseconds());
					throw new MyException();
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(5.Milliseconds()).And(50.Seconds())
						.WithTimeSystem(time);

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
