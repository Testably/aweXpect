using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Core;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ExecutesIn
	{
		public sealed class WithinTests
		{
			[Test]
			public async Task WhenDelegateIsTooFast_ShouldFail()
			{
				VirtualTimeProvider time = new();
				Action @delegate = () => time.Advance(5.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn(10000.Milliseconds()).Within(1123.Milliseconds())
						.WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in approximately 0:10 ± 0:01.123,
					             but it took only 0:00.005
					             """);
			}

			[Test]
			public async Task WhenDelegateTakesLongEnough_ShouldSucceed()
			{
				VirtualTimeProvider time = new();
				Action @delegate = () => time.Advance(50.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn(50.Milliseconds()).Within(5.Seconds()).WithTimeProvider(time);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateTakesTooLong_ShouldFail()
			{
				VirtualTimeProvider time = new();
				Action @delegate = () => time.Advance(50.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn(10.Milliseconds()).Within(5.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in approximately 0:00.010 ± 0:00.005,
					             but it took 0:00.050
					             """);
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				VirtualTimeProvider time = new();
				Action @delegate = () =>
				{
					time.Advance(500.Milliseconds());
					throw new MyException();
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn(500.Milliseconds()).Within(50.Seconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in approximately 0:00.500 ± 0:50,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a crashed delegate must fail even though its duration was inside the tolerance");
			}

			[Test]
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
